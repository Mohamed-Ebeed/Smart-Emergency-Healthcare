using Microsoft.EntityFrameworkCore;
using SmartEmergency.Application.Common.Interfaces;
using SmartEmergency.Application.Common.Models;
using SmartEmergency.Application.EmergencyRequests.DTOs;
using SmartEmergency.Application.EmergencyRequests.Interfaces;
using SmartEmergency.Application.Notifications.Interfaces;
using SmartEmergency.Application.Patients.Services;
using SmartEmergency.Domain.Entities;
using SmartEmergency.Domain.Enums;
using SmartEmergency.Domain.Rules;

namespace SmartEmergency.Application.EmergencyRequests.Services;

public class EmergencyCoordinationService : IEmergencyCoordinationService
{
    private readonly IAppDbContext _db;
    private readonly INotificationService _notifications;
    private readonly IRealtimeNotifier _realtime;

    public EmergencyCoordinationService(IAppDbContext db, INotificationService notifications, IRealtimeNotifier realtime)
    {
        _db = db;
        _notifications = notifications;
        _realtime = realtime;
    }

    public async Task<PagedResult<EmergencyRequestDto>> SearchAsync(EmergencyRequestFilterDto filter, CancellationToken ct = default)
    {
        ValidationHelper.Validate(filter);

        var query = _db.EmergencyRequests.AsNoTracking().AsQueryable();

        if (filter.Status is { } status)
            query = query.Where(r => r.CurrentStatus == status);

        if (filter.Priority is { } priority)
            query = query.Where(r => r.Priority == priority);

        if (filter.FromDate is { } from)
        {
            var start = from.ToDateTime(TimeOnly.MinValue);
            query = query.Where(r => r.CreatedAt >= start);
        }

        if (filter.ToDate is { } to)
        {
            // Less than the next day, so the whole ToDate day is included
            var end = to.AddDays(1).ToDateTime(TimeOnly.MinValue);
            query = query.Where(r => r.CreatedAt < end);
        }

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(r => new EmergencyRequestDto
            {
                EmergencyRequestId = r.EmergencyRequestId,
                PatientId = r.PatientId,
                EmergencyTypeId = r.EmergencyTypeId,
                EmergencyTypeName = r.EmergencyType != null ? r.EmergencyType.Name : null,
                Description = r.Description,
                Latitude = r.Latitude,
                Longitude = r.Longitude,
                AddressText = r.AddressText,
                Priority = r.Priority,
                CurrentStatus = r.CurrentStatus,
                CreatedAt = r.CreatedAt,
                PickedUpAt = r.PickedUpAt,
                ArrivedAtHospitalAt = r.ArrivedAtHospitalAt,
                ClosedAt = r.ClosedAt
            })
            .ToListAsync(ct);

        return new PagedResult<EmergencyRequestDto>
        {
            Items = items,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize,
            TotalCount = total
        };
    }

    // Uses the existing CaseStatusHistory table, oldest change first
    public async Task<List<CaseStatusHistoryDto>> GetStatusHistoryAsync(long requestId, CancellationToken ct = default)
    {
        var exists = await _db.EmergencyRequests.AnyAsync(r => r.EmergencyRequestId == requestId, ct);
        if (!exists)
            throw new KeyNotFoundException("Emergency request not found.");

        return await _db.CaseStatusHistories.AsNoTracking()
            .Where(h => h.EmergencyRequestId == requestId)
            .OrderBy(h => h.ChangedAt)
            .Select(h => new CaseStatusHistoryDto
            {
                CaseStatusHistoryId = h.CaseStatusHistoryId,
                Status = h.Status,
                ChangedByUserId = h.ChangedByUserId,
                Notes = h.Notes,
                ChangedAt = h.ChangedAt
            })
            .ToListAsync(ct);
    }

    // Operator sends the request to a hospital and waits for its answer
    public async Task<HospitalAssignmentDto> SendToHospitalAsync(long requestId, SendToHospitalDto dto, string operatorUserId, CancellationToken ct = default)
    {
        ValidationHelper.Validate(dto);

        var request = await _db.EmergencyRequests
            .FirstOrDefaultAsync(r => r.EmergencyRequestId == requestId, ct)
            ?? throw new KeyNotFoundException("Emergency request not found.");

        if (request.CurrentStatus != RequestStatus.Confirmed && request.CurrentStatus != RequestStatus.SentToHospital)
            throw new InvalidOperationException("Only confirmed requests can be sent to a hospital.");

        var hospital = await _db.Hospitals.AsNoTracking()
            .FirstOrDefaultAsync(h => h.HospitalId == dto.HospitalId, ct)
            ?? throw new KeyNotFoundException("Hospital not found.");

        if (!hospital.IsActive)
            throw new InvalidOperationException("This hospital is not active.");

        // One open assignment at a time (waiting or already accepted)
        var hasOpen = await _db.EmergencyHospitalAssignments.AnyAsync(a =>
            a.EmergencyRequestId == requestId &&
            (a.AssignmentStatus == AssignmentStatus.Pending || a.AssignmentStatus == AssignmentStatus.Accepted), ct);
        if (hasOpen)
            throw new InvalidOperationException("This request is already waiting for or accepted by a hospital.");

        // Don't send again to a hospital that already rejected it
        var rejectedBefore = await _db.EmergencyHospitalAssignments.AnyAsync(a =>
            a.EmergencyRequestId == requestId && a.HospitalId == dto.HospitalId &&
            a.AssignmentStatus == AssignmentStatus.Rejected, ct);
        if (rejectedBefore)
            throw new InvalidOperationException("This hospital already rejected the request.");

        var assignment = new EmergencyHospitalAssignment
        {
            EmergencyRequestId = requestId,
            HospitalId = hospital.HospitalId,
            AssignedByOperatorId = operatorUserId,
            AssignmentStatus = AssignmentStatus.Pending,
            AssignedAt = DateTime.UtcNow
        };
        _db.EmergencyHospitalAssignments.Add(assignment);

        var statusChanged = request.CurrentStatus != RequestStatus.SentToHospital;
        AddStatusChange(request, RequestStatus.SentToHospital, operatorUserId, $"Sent to {hospital.Name}");

        await _db.SaveChangesAsync(ct);

        if (statusChanged)
            await _notifications.NotifyEmergencyRequestStatusChangedAsync(requestId, ct);

        // The hospital staff see the new request live
        await _realtime.HospitalAssignmentChangedAsync(hospital.HospitalId, requestId,
            assignment.EmergencyHospitalAssignmentId, assignment.AssignmentStatus, ct);

        return await GetHospitalAssignmentAsync(assignment.EmergencyHospitalAssignmentId, ct);
    }

    // Hospital staff accept or reject a request sent to their hospital
    public async Task<HospitalAssignmentDto> RespondToHospitalAssignmentAsync(long assignmentId, HospitalResponseDto dto, string userId, bool isAdmin, CancellationToken ct = default)
    {
        ValidationHelper.Validate(dto);

        var assignment = await _db.EmergencyHospitalAssignments
            .Include(a => a.EmergencyRequest)
            .Include(a => a.Hospital)
            .FirstOrDefaultAsync(a => a.EmergencyHospitalAssignmentId == assignmentId, ct)
            ?? throw new KeyNotFoundException("Hospital assignment not found.");

        // Staff can only answer for their own hospital
        if (!isAdmin)
        {
            var staffHospitalId = await _db.HospitalStaffs
                .Where(s => s.UserId == userId && s.IsActive)
                .Select(s => (int?)s.HospitalId)
                .FirstOrDefaultAsync(ct);

            if (staffHospitalId != assignment.HospitalId)
                throw new UnauthorizedAccessException("You can only respond for your own hospital.");
        }

        if (assignment.AssignmentStatus != AssignmentStatus.Pending)
            throw new InvalidOperationException("This assignment was already answered.");

        if (RequestStatusTransitions.IsFinalStatus(assignment.EmergencyRequest.CurrentStatus))
            throw new InvalidOperationException("The emergency request is already closed.");

        var accepted = dto.Accept!.Value;
        assignment.AssignmentStatus = accepted ? AssignmentStatus.Accepted : AssignmentStatus.Rejected;
        assignment.RespondedAt = DateTime.UtcNow;
        assignment.ResponseNotes = dto.Notes?.Trim();

        // Keep the decision in the request timeline too (status stays SentToHospital)
        _db.CaseStatusHistories.Add(new CaseStatusHistory
        {
            EmergencyRequestId = assignment.EmergencyRequestId,
            Status = assignment.EmergencyRequest.CurrentStatus,
            ChangedByUserId = userId,
            Notes = $"{assignment.Hospital.Name} {(accepted ? "accepted" : "rejected")} the request",
            ChangedAt = DateTime.UtcNow
        });

        await _db.SaveChangesAsync(ct);
        await _notifications.NotifyEmergencyRequestStatusChangedAsync(assignment.EmergencyRequestId, ct);
        await _realtime.HospitalAssignmentChangedAsync(assignment.HospitalId, assignment.EmergencyRequestId,
            assignment.EmergencyHospitalAssignmentId, assignment.AssignmentStatus, ct);

        return await GetHospitalAssignmentAsync(assignmentId, ct);
    }

    public async Task<List<HospitalAssignmentDto>> GetHospitalAssignmentsAsync(long requestId, CancellationToken ct = default)
    {
        var exists = await _db.EmergencyRequests.AnyAsync(r => r.EmergencyRequestId == requestId, ct);
        if (!exists)
            throw new KeyNotFoundException("Emergency request not found.");

        return await _db.EmergencyHospitalAssignments.AsNoTracking()
            .Where(a => a.EmergencyRequestId == requestId)
            .OrderBy(a => a.AssignedAt)
            .Select(ToHospitalAssignmentDto)
            .ToListAsync(ct);
    }

    // Next hospital = active, not tried before for this request,
    // hospitals with free beds first, then the nearest to the patient
    public async Task<NextHospitalDto> GetNextHospitalAsync(long requestId, CancellationToken ct = default)
    {
        var request = await _db.EmergencyRequests.AsNoTracking()
            .Where(r => r.EmergencyRequestId == requestId)
            .Select(r => new { r.Latitude, r.Longitude })
            .FirstOrDefaultAsync(ct)
            ?? throw new KeyNotFoundException("Emergency request not found.");

        var triedHospitalIds = await _db.EmergencyHospitalAssignments
            .Where(a => a.EmergencyRequestId == requestId)
            .Select(a => a.HospitalId)
            .ToListAsync(ct);

        var candidates = await _db.Hospitals.AsNoTracking()
            .Where(h => h.IsActive && !triedHospitalIds.Contains(h.HospitalId))
            .Select(h => new
            {
                h.HospitalId,
                h.Name,
                h.Address,
                h.Latitude,
                h.Longitude,
                AvailableBeds = _db.HospitalBeds.Count(b =>
                    b.Department.HospitalId == h.HospitalId &&
                    b.Department.IsActive &&
                    b.BedStatus == BedStatus.Available)
            })
            .ToListAsync(ct);

        var best = candidates
            .Select(h => new NextHospitalDto
            {
                HospitalId = h.HospitalId,
                Name = h.Name,
                Address = h.Address,
                AvailableBeds = h.AvailableBeds,
                DistanceKm = DistanceKm(request.Latitude, request.Longitude, h.Latitude, h.Longitude)
            })
            .OrderByDescending(h => h.AvailableBeds > 0)
            .ThenBy(h => h.DistanceKm ?? double.MaxValue)
            .ThenBy(h => h.Name)
            .FirstOrDefault();

        return best ?? throw new KeyNotFoundException("No other suitable hospital was found.");
    }

    // Used after a rejection: pick the next hospital and send the request to it
    public async Task<HospitalAssignmentDto> SendToNextHospitalAsync(long requestId, string operatorUserId, CancellationToken ct = default)
    {
        var next = await GetNextHospitalAsync(requestId, ct);
        return await SendToHospitalAsync(requestId, new SendToHospitalDto { HospitalId = next.HospitalId }, operatorUserId, ct);
    }

    // Operator sends an ambulance + paramedic after a hospital accepted the request
    public async Task<AmbulanceAssignmentDto> AssignAmbulanceAsync(long requestId, AssignAmbulanceDto dto, string operatorUserId, CancellationToken ct = default)
    {
        ValidationHelper.Validate(dto);

        var request = await _db.EmergencyRequests
            .FirstOrDefaultAsync(r => r.EmergencyRequestId == requestId, ct)
            ?? throw new KeyNotFoundException("Emergency request not found.");

        var hospitalAccepted = await _db.EmergencyHospitalAssignments.AnyAsync(a =>
            a.EmergencyRequestId == requestId && a.AssignmentStatus == AssignmentStatus.Accepted, ct);
        if (request.CurrentStatus != RequestStatus.SentToHospital || !hospitalAccepted)
            throw new InvalidOperationException("An ambulance can be assigned only after a hospital accepts the request.");

        var ambulance = await _db.Ambulances
            .FirstOrDefaultAsync(a => a.AmbulanceId == dto.AmbulanceId, ct)
            ?? throw new KeyNotFoundException("Ambulance not found.");

        if (ambulance.AmbulanceStatus != AmbulanceStatus.Available)
            throw new InvalidOperationException("This ambulance is not available now.");

        var paramedic = await _db.Paramedics
            .FirstOrDefaultAsync(p => p.ParamedicId == dto.ParamedicId, ct)
            ?? throw new KeyNotFoundException("Paramedic not found.");

        if (!paramedic.IsActive || !paramedic.IsAvailable)
            throw new InvalidOperationException("This paramedic is not available now.");

        // Ambulance and paramedic come from the same hospital
        if (paramedic.HospitalId != ambulance.HospitalId)
            throw new InvalidOperationException("Paramedic and ambulance must belong to the same hospital.");

        var now = DateTime.UtcNow;
        var assignment = new AmbulanceAssignment
        {
            EmergencyRequestId = requestId,
            AmbulanceId = ambulance.AmbulanceId,
            ParamedicId = paramedic.ParamedicId,
            AssignedByUserId = operatorUserId,
            AssignmentStatus = AssignmentStatus.Accepted,
            AssignedAt = now
        };
        _db.AmbulanceAssignments.Add(assignment);

        // Ambulance is busy and the paramedic is on a job now
        ambulance.AmbulanceStatus = AmbulanceStatus.Busy;
        _db.AmbulanceStatusHistories.Add(new AmbulanceStatusHistory
        {
            AmbulanceId = ambulance.AmbulanceId,
            Status = AmbulanceStatus.Busy,
            ChangedAt = now,
            Notes = $"Assigned to emergency request {requestId}"
        });
        paramedic.IsAvailable = false;

        AddStatusChange(request, RequestStatus.AmbulanceAssigned, operatorUserId, $"Ambulance {ambulance.VehicleNumber} assigned");

        await _db.SaveChangesAsync(ct);
        await _notifications.NotifyEmergencyRequestStatusChangedAsync(requestId, ct);
        await _realtime.AmbulanceChangedAsync(ambulance.AmbulanceId, ambulance.AmbulanceStatus,
            ambulance.LastKnownLatitude, ambulance.LastKnownLongitude, requestId, ct);

        return await GetAmbulanceAssignmentAsync(assignment.AmbulanceAssignmentId, ct);
    }

    public async Task<List<AmbulanceAssignmentDto>> GetAmbulanceAssignmentsAsync(long requestId, CancellationToken ct = default)
    {
        var exists = await _db.EmergencyRequests.AnyAsync(r => r.EmergencyRequestId == requestId, ct);
        if (!exists)
            throw new KeyNotFoundException("Emergency request not found.");

        return await _db.AmbulanceAssignments.AsNoTracking()
            .Where(a => a.EmergencyRequestId == requestId)
            .OrderBy(a => a.AssignedAt)
            .Select(ToAmbulanceAssignmentDto)
            .ToListAsync(ct);
    }

    public async Task<AmbulanceAssignmentDto> GetAmbulanceAssignmentAsync(long assignmentId, CancellationToken ct = default) =>
        await _db.AmbulanceAssignments.AsNoTracking()
            .Where(a => a.AmbulanceAssignmentId == assignmentId)
            .Select(ToAmbulanceAssignmentDto)
            .FirstOrDefaultAsync(ct)
        ?? throw new KeyNotFoundException("Ambulance assignment not found.");

    // Assignments of the logged in paramedic, newest first
    public async Task<List<AmbulanceAssignmentDto>> GetMyAmbulanceAssignmentsAsync(string paramedicUserId, CancellationToken ct = default) =>
        await _db.AmbulanceAssignments.AsNoTracking()
            .Where(a => a.Paramedic.UserId == paramedicUserId)
            .OrderByDescending(a => a.AssignedAt)
            .Select(ToAmbulanceAssignmentDto)
            .ToListAsync(ct);

    // Helpers

    private static readonly System.Linq.Expressions.Expression<Func<AmbulanceAssignment, AmbulanceAssignmentDto>> ToAmbulanceAssignmentDto =
        a => new AmbulanceAssignmentDto
        {
            AmbulanceAssignmentId = a.AmbulanceAssignmentId,
            EmergencyRequestId = a.EmergencyRequestId,
            AmbulanceId = a.AmbulanceId,
            VehicleNumber = a.Ambulance.VehicleNumber,
            ParamedicId = a.ParamedicId,
            ParamedicName = a.Paramedic.FullName,
            AssignedByUserId = a.AssignedByUserId,
            AssignmentStatus = a.AssignmentStatus,
            AssignedAt = a.AssignedAt,
            CompletedAt = a.CompletedAt
        };

    private async Task<HospitalAssignmentDto> GetHospitalAssignmentAsync(long assignmentId, CancellationToken ct) =>
        await _db.EmergencyHospitalAssignments.AsNoTracking()
            .Where(a => a.EmergencyHospitalAssignmentId == assignmentId)
            .Select(ToHospitalAssignmentDto)
            .FirstAsync(ct);

    private static readonly System.Linq.Expressions.Expression<Func<EmergencyHospitalAssignment, HospitalAssignmentDto>> ToHospitalAssignmentDto =
        a => new HospitalAssignmentDto
        {
            EmergencyHospitalAssignmentId = a.EmergencyHospitalAssignmentId,
            EmergencyRequestId = a.EmergencyRequestId,
            HospitalId = a.HospitalId,
            HospitalName = a.Hospital.Name,
            AssignmentStatus = a.AssignmentStatus,
            AssignedByOperatorId = a.AssignedByOperatorId,
            AssignedAt = a.AssignedAt,
            RespondedAt = a.RespondedAt,
            ResponseNotes = a.ResponseNotes
        };

    // Straight line distance between two points (Haversine)
    private static double? DistanceKm(decimal? lat1, decimal? lon1, decimal? lat2, decimal? lon2)
    {
        if (lat1 is null || lon1 is null || lat2 is null || lon2 is null)
            return null;

        const double earthRadiusKm = 6371;
        double ToRad(decimal deg) => (double)deg * Math.PI / 180;

        var dLat = ToRad(lat2.Value - lat1.Value);
        var dLon = ToRad(lon2.Value - lon1.Value);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRad(lat1.Value)) * Math.Cos(ToRad(lat2.Value)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        return Math.Round(earthRadiusKm * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a)), 2);
    }

    // Changes the request status (if needed) and writes a history row
    private void AddStatusChange(EmergencyRequest request, RequestStatus newStatus, string userId, string? notes)
    {
        if (request.CurrentStatus != newStatus)
        {
            if (!RequestStatusTransitions.CanTransition(request.CurrentStatus, newStatus))
                throw new InvalidOperationException($"Can't change status from {request.CurrentStatus} to {newStatus}.");

            request.CurrentStatus = newStatus;
        }

        _db.CaseStatusHistories.Add(new CaseStatusHistory
        {
            EmergencyRequestId = request.EmergencyRequestId,
            Status = newStatus,
            ChangedByUserId = userId,
            Notes = notes,
            ChangedAt = DateTime.UtcNow
        });
    }
}