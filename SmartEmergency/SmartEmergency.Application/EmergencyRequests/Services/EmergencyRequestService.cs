
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SmartEmergency.Application.Common.Interfaces;
using SmartEmergency.Application.Notifications.Interfaces;
using SmartEmergency.Application.EmergencyRequests.DTOs;
using SmartEmergency.Application.EmergencyRequests.Interfaces;
using SmartEmergency.Domain.Common;
using SmartEmergency.Domain.Entities;
using SmartEmergency.Domain.Enums;
using SmartEmergency.Domain.Rules;

namespace SmartEmergency.Application.EmergencyRequests.Services
{
    public class EmergencyRequestService : IEmergencyRequestService
    {
        private readonly IAppDbContext _db;
        private readonly INotificationService _notificationService;
        private readonly IRealtimeNotifier _realtime;

        public EmergencyRequestService(
            IAppDbContext db,
            INotificationService notificationService,
            IRealtimeNotifier realtime)
        {
            _db = db;
            _notificationService = notificationService;
            _realtime = realtime;
        }

        public async Task<EmergencyRequestDto> CreateAsync(
            CreateEmergencyRequestDto dto,
            CancellationToken cancellationToken = default)
        {
            ValidateCreate(dto);

            var patient = await _db.Patients
                .FirstOrDefaultAsync(
                    p => p.PatientId == dto.PatientId,
                    cancellationToken);

            if (patient == null)
                throw new KeyNotFoundException("Patient not found.");

            var emergencyType = await _db.EmergencyTypes
                .FirstOrDefaultAsync(
                    t => t.EmergencyTypeId == dto.EmergencyTypeId,
                    cancellationToken);

            if (emergencyType == null)
                throw new KeyNotFoundException("Emergency type not found.");

            var now = DateTime.UtcNow;

            var request = new EmergencyRequest
            {
                PatientId = patient.PatientId,
                EmergencyTypeId = emergencyType.EmergencyTypeId,
                Description = dto.Description,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                AddressText = dto.AddressText,
                Priority = emergencyType.DefaultPriority,
                CurrentStatus = RequestStatus.Submitted,
                CreatedAt = now
            };

            request.CaseStatusHistories.Add(new CaseStatusHistory
            {
                Status = RequestStatus.Submitted,
                ChangedByUserId = patient.UserId,
                Notes = "Request created",
                ChangedAt = now
            });

            _db.EmergencyRequests.Add(request);

            await _db.SaveChangesAsync(cancellationToken);

            await _notificationService.NotifyEmergencyRequestSubmittedAsync(
                request.EmergencyRequestId,
                cancellationToken);

            await _realtime.RequestStatusChangedAsync(
                request.EmergencyRequestId,
                RequestStatus.Submitted,
                cancellationToken);

            return await GetByIdAsync(
                request.EmergencyRequestId,
                cancellationToken);
        }

        public async Task<EmergencyRequestDto> GetByIdAsync(
            long requestId,
            CancellationToken cancellationToken = default)
        {
            var request = await _db.EmergencyRequests
                .AsNoTracking()
                .Include(r => r.EmergencyType)
                .FirstOrDefaultAsync(
                    r => r.EmergencyRequestId == requestId,
                    cancellationToken);

            if (request == null)
                throw new KeyNotFoundException(
                    "Emergency request not found.");

            return MapToDto(request);
        }

        public async Task<List<EmergencyRequestDto>> GetByPatientAsync(
            int patientId,
            CancellationToken cancellationToken = default)
        {
            var requests = await _db.EmergencyRequests
                .AsNoTracking()
                .Include(r => r.EmergencyType)
                .Where(r => r.PatientId == patientId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync(cancellationToken);

            return requests.Select(MapToDto).ToList();
        }

        public async Task<bool> ChangeStatusAsync(
            long requestId,
            RequestStatus newStatus,
            string userId,
            string userRole,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("UserId is required.");

            if (string.IsNullOrWhiteSpace(userRole))
                throw new UnauthorizedAccessException(
                    "User role is required.");

            if (newStatus == RequestStatus.SentToHospital)
            {
                throw new UnauthorizedAccessException(
                    "Use the hospital assignment workflow to send an emergency request to a hospital.");
            }

            var request = await _db.EmergencyRequests
                .FirstOrDefaultAsync(
                    r => r.EmergencyRequestId == requestId,
                    cancellationToken);

            if (request == null)
                return false;

            if (!RequestStatusTransitions.CanTransition(
                request.CurrentStatus, newStatus))
                return false;

            switch (userRole)
            {
                case Roles.Admin:
                    break;

                case Roles.Operator:
                    var isAllowedOperatorTransition =
                        request.CurrentStatus == RequestStatus.Submitted &&
                        (newStatus == RequestStatus.Confirmed ||
                         newStatus == RequestStatus.Invalid);

                    if (!isAllowedOperatorTransition)
                    {
                        throw new UnauthorizedAccessException(
                            "Operators can only confirm or invalidate submitted requests.");
                    }
                    break;

                case Roles.Paramedic:
                    var isAllowedParamedicTransition =
                        (request.CurrentStatus == RequestStatus.AmbulanceAssigned &&
                         newStatus == RequestStatus.OnTheWay) ||
                        (request.CurrentStatus == RequestStatus.OnTheWay &&
                         newStatus == RequestStatus.Arrived) ||
                        (request.CurrentStatus == RequestStatus.Arrived &&
                         newStatus == RequestStatus.PickedUp) ||
                        (request.CurrentStatus == RequestStatus.PickedUp &&
                         newStatus == RequestStatus.GoingToHospital) ||
                        (request.CurrentStatus == RequestStatus.GoingToHospital &&
                         newStatus == RequestStatus.AtHospital);

                    if (!isAllowedParamedicTransition)
                    {
                        throw new UnauthorizedAccessException(
                            "Paramedics cannot perform this status transition.");
                    }

                    var isAssignedParamedic = await _db.Paramedics
                        .AnyAsync(p =>
                            p.UserId == userId &&
                            p.IsActive &&
                            p.AmbulanceAssignments.Any(a =>
                                a.EmergencyRequestId == requestId &&
                                a.CompletedAt == null &&
                                a.AssignmentStatus == AssignmentStatus.Accepted),
                            cancellationToken);

                    if (!isAssignedParamedic)
                    {
                        throw new UnauthorizedAccessException(
                            "Only the active paramedic assigned to this request can change its status.");
                    }
                    break;

                case Roles.HospitalStaff:
                    throw new UnauthorizedAccessException(
                        "Hospital staff must use the hospital assignment response endpoint.");

                default:
                    throw new UnauthorizedAccessException(
                        "You are not authorized to change emergency request status.");
            }

            UpdateStatus(request, newStatus, userId, null);

            Ambulance? releasedAmbulance = null;

            if (newStatus == RequestStatus.Completed ||
                newStatus == RequestStatus.Cancelled)
            {
                releasedAmbulance =
                    await ReleaseAmbulanceAssignmentAsync(
                        requestId,
                        newStatus,
                        cancellationToken);
            }

            await _db.SaveChangesAsync(cancellationToken);

            await _notificationService.NotifyEmergencyRequestStatusChangedAsync(
                requestId,
                cancellationToken);

            await _realtime.RequestStatusChangedAsync(
                requestId,
                newStatus,
                cancellationToken);

            if (releasedAmbulance != null)
            {
                await _realtime.AmbulanceChangedAsync(
                    releasedAmbulance.AmbulanceId,
                    releasedAmbulance.HospitalId,
                    releasedAmbulance.AmbulanceStatus,
                    releasedAmbulance.LastKnownLatitude,
                    releasedAmbulance.LastKnownLongitude,
                    null,
                    cancellationToken);
            }

            return true;
        }

        private async Task<Ambulance?> ReleaseAmbulanceAssignmentAsync(
            long requestId,
            RequestStatus requestStatus,
            CancellationToken ct)
        {
            var assignment = await _db.AmbulanceAssignments
                .Include(a => a.Ambulance)
                .Include(a => a.Paramedic)
                .FirstOrDefaultAsync(a =>
                    a.EmergencyRequestId == requestId &&
                    a.CompletedAt == null &&
                    a.AssignmentStatus == AssignmentStatus.Accepted,
                    ct);

            if (assignment == null)
                return null;

            var now = DateTime.UtcNow;

            assignment.CompletedAt = now;

            if (requestStatus == RequestStatus.Cancelled)
            {
                assignment.AssignmentStatus =
                    AssignmentStatus.Cancelled;
            }

            assignment.Ambulance.AmbulanceStatus =
                AmbulanceStatus.Available;

            assignment.Paramedic.IsAvailable = true;

            _db.AmbulanceStatusHistories.Add(
                new AmbulanceStatusHistory
                {
                    AmbulanceId = assignment.AmbulanceId,
                    Status = AmbulanceStatus.Available,
                    ChangedAt = now,
                    Notes = $"Emergency request {requestId} {requestStatus}. Ambulance released."
                });

            return assignment.Ambulance;
        }

        public async Task CancelAsync(
            long requestId,
            string userId,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("UserId is required.");

            var request = await _db.EmergencyRequests
                .Include(r => r.Patient)
                .FirstOrDefaultAsync(
                    r => r.EmergencyRequestId == requestId,
                    cancellationToken);

            if (request == null)
                throw new KeyNotFoundException(
                    "Emergency request not found.");

            if (request.Patient.UserId != userId)
            {
                throw new UnauthorizedAccessException(
                    "Only the patient who owns the request can cancel it.");
            }

            if (request.CurrentStatus != RequestStatus.Submitted &&
                request.CurrentStatus != RequestStatus.Confirmed &&
                request.CurrentStatus != RequestStatus.SentToHospital)
            {
                throw new InvalidOperationException(
                    "The request can only be cancelled before an ambulance is assigned.");
            }

            UpdateStatus(
                request,
                RequestStatus.Cancelled,
                userId,
                "Cancelled by patient");

            var now = DateTime.UtcNow;

            var openAssignments = await _db.EmergencyHospitalAssignments
                .Where(a =>
                    a.EmergencyRequestId == requestId &&
                    (a.AssignmentStatus == AssignmentStatus.Pending ||
                     a.AssignmentStatus == AssignmentStatus.Accepted))
                .ToListAsync(cancellationToken);

            foreach (var assignment in openAssignments)
            {
                assignment.AssignmentStatus = AssignmentStatus.Cancelled;
                assignment.RespondedAt = now;
                assignment.ResponseNotes = "Cancelled by patient";
            }

            await _db.SaveChangesAsync(cancellationToken);

            await _notificationService.NotifyEmergencyRequestStatusChangedAsync(
                requestId,
                cancellationToken);

            await _realtime.RequestStatusChangedAsync(
                requestId,
                RequestStatus.Cancelled,
                cancellationToken);

            foreach (var assignment in openAssignments)
            {
                await _realtime.HospitalAssignmentChangedAsync(
                    assignment.HospitalId,
                    requestId,
                    assignment.EmergencyHospitalAssignmentId,
                    AssignmentStatus.Cancelled,
                    cancellationToken);
            }
        }

        private void UpdateStatus(
            EmergencyRequest request,
            RequestStatus newStatus,
            string userId,
            string? notes)
        {
            var now = DateTime.UtcNow;

            request.CurrentStatus = newStatus;

            if (newStatus == RequestStatus.PickedUp)
            {
                request.PickedUpAt = now;
            }
            else if (newStatus == RequestStatus.AtHospital)
            {
                request.ArrivedAtHospitalAt = now;
            }
            else if (newStatus == RequestStatus.Completed ||
                     newStatus == RequestStatus.Cancelled ||
                     newStatus == RequestStatus.Invalid)
            {
                request.ClosedAt = now;
                request.ClosedByUserId = userId;
            }

            _db.CaseStatusHistories.Add(new CaseStatusHistory
            {
                EmergencyRequestId = request.EmergencyRequestId,
                Status = newStatus,
                ChangedByUserId = userId,
                Notes = notes,
                ChangedAt = now
            });
        }

        private static EmergencyRequestDto MapToDto(
            EmergencyRequest request)
        {
            return new EmergencyRequestDto
            {
                EmergencyRequestId = request.EmergencyRequestId,
                PatientId = request.PatientId,
                EmergencyTypeId = request.EmergencyTypeId,
                EmergencyTypeName = request.EmergencyType?.Name,
                Description = request.Description,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                AddressText = request.AddressText,
                Priority = request.Priority,
                CurrentStatus = request.CurrentStatus,
                CreatedAt = request.CreatedAt,
                PickedUpAt = request.PickedUpAt,
                ArrivedAtHospitalAt = request.ArrivedAtHospitalAt,
                ClosedAt = request.ClosedAt
            };
        }

        private static void ValidateCreate(
            CreateEmergencyRequestDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (dto.PatientId <= 0)
                throw new ArgumentException("PatientId is required.");

            if (dto.EmergencyTypeId <= 0)
                throw new ArgumentException(
                    "EmergencyTypeId is required.");

            if (dto.Description != null &&
                dto.Description.Length > 1000)
            {
                throw new ArgumentException(
                    "Description cannot be more than 1000 characters.");
            }

            if (dto.Latitude.HasValue != dto.Longitude.HasValue)
            {
                throw new ArgumentException(
                    "Latitude and Longitude must be sent together.");
            }

            if (dto.Latitude.HasValue &&
                (dto.Latitude < -90 || dto.Latitude > 90))
            {
                throw new ArgumentException(
                    "Latitude must be between -90 and 90.");
            }

            if (dto.Longitude.HasValue &&
                (dto.Longitude < -180 || dto.Longitude > 180))
            {
                throw new ArgumentException(
                    "Longitude must be between -180 and 180.");
            }

            if (dto.AddressText != null &&
                dto.AddressText.Length > 500)
            {
                throw new ArgumentException(
                    "AddressText cannot be more than 500 characters.");
            }
        }
    }
}
