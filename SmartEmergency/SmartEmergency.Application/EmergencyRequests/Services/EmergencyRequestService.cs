using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SmartEmergency.Application.Common.Interfaces;
using SmartEmergency.Application.EmergencyRequests.DTOs;
using SmartEmergency.Application.EmergencyRequests.Interfaces;
using SmartEmergency.Domain.Entities;
using SmartEmergency.Domain.Enums;
using SmartEmergency.Domain.Rules;

namespace SmartEmergency.Application.EmergencyRequests.Services
{
    public class EmergencyRequestService : IEmergencyRequestService
    {
        private readonly IAppDbContext _db;

        public EmergencyRequestService(IAppDbContext db)
        {
            _db = db;
        }

        public async Task<EmergencyRequestDto> CreateAsync(CreateEmergencyRequestDto dto, CancellationToken cancellationToken = default)
        {
            ValidateCreate(dto);

            var patient = await _db.Patients
                .FirstOrDefaultAsync(p => p.PatientId == dto.PatientId, cancellationToken);
            if (patient == null)
                throw new KeyNotFoundException("Patient not found.");

            var emergencyType = await _db.EmergencyTypes
                .FirstOrDefaultAsync(t => t.EmergencyTypeId == dto.EmergencyTypeId, cancellationToken);
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

            //first history record
            request.CaseStatusHistories.Add(new CaseStatusHistory
            {
                Status = RequestStatus.Submitted,
                ChangedByUserId = patient.UserId,
                Notes = "Request created",
                ChangedAt = now
            });

            _db.EmergencyRequests.Add(request);
            await _db.SaveChangesAsync(cancellationToken);

            return await GetByIdAsync(request.EmergencyRequestId, cancellationToken);
        }

        public async Task<EmergencyRequestDto> GetByIdAsync(long requestId, CancellationToken cancellationToken = default)
        {
            var request = await _db.EmergencyRequests
                .AsNoTracking()
                .Include(r => r.EmergencyType)
                .FirstOrDefaultAsync(r => r.EmergencyRequestId == requestId, cancellationToken);

            if (request == null)
                throw new KeyNotFoundException("Emergency request not found.");

            return MapToDto(request);
        }

        public async Task<List<EmergencyRequestDto>> GetByPatientAsync(int patientId, CancellationToken cancellationToken = default)
        {
            var requests = await _db.EmergencyRequests
                .AsNoTracking()
                .Include(r => r.EmergencyType)
                .Where(r => r.PatientId == patientId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync(cancellationToken);

            return requests.Select(r => MapToDto(r)).ToList();
        }

        public async Task<bool> ChangeStatusAsync(long requestId, RequestStatus newStatus, string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("UserId is required.");

            //1. check the request exists
            var request = await _db.EmergencyRequests
                .FirstOrDefaultAsync(r => r.EmergencyRequestId == requestId);
            if (request == null)
                return false;

            //2. check the transition is allowed
            if (!RequestStatusTransitions.CanTransition(request.CurrentStatus, newStatus))
                return false;

            //3,4,5. update the status and add history record
            UpdateStatus(request, newStatus, userId, null);

            //6. one save for both changes
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task CancelAsync(long requestId, string userId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("UserId is required.");

            var request = await _db.EmergencyRequests
                .Include(r => r.Patient)
                .FirstOrDefaultAsync(r => r.EmergencyRequestId == requestId, cancellationToken);
            if (request == null)
                throw new KeyNotFoundException("Emergency request not found.");

            //only the owner patient can cancel
            if (request.Patient.UserId != userId)
                throw new UnauthorizedAccessException("Only the patient who owns the request can cancel it.");

            //patient can cancel only before ambulance assignment
            if (request.CurrentStatus != RequestStatus.Submitted
                && request.CurrentStatus != RequestStatus.Confirmed
                && request.CurrentStatus != RequestStatus.SentToHospital)
                throw new InvalidOperationException("The request can only be cancelled before an ambulance is assigned.");

            UpdateStatus(request, RequestStatus.Cancelled, userId, "Cancelled by patient");
            await _db.SaveChangesAsync(cancellationToken);
        }

        private void UpdateStatus(EmergencyRequest request, RequestStatus newStatus, string userId, string? notes)
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
            else if (newStatus == RequestStatus.Completed
                  || newStatus == RequestStatus.Cancelled
                  || newStatus == RequestStatus.Invalid)
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

        private static EmergencyRequestDto MapToDto(EmergencyRequest request)
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

        private static void ValidateCreate(CreateEmergencyRequestDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (dto.PatientId <= 0)
                throw new ArgumentException("PatientId is required.");

            if (dto.EmergencyTypeId <= 0)
                throw new ArgumentException("EmergencyTypeId is required.");

            if (dto.Description != null && dto.Description.Length > 1000)
                throw new ArgumentException("Description cannot be more than 1000 characters.");


            if (dto.Latitude.HasValue != dto.Longitude.HasValue)
                throw new ArgumentException("Latitude and Longitude must be sent together.");

            if (dto.Latitude.HasValue && (dto.Latitude < -90 || dto.Latitude > 90))
                throw new ArgumentException("Latitude must be between -90 and 90.");

            if (dto.Longitude.HasValue && (dto.Longitude < -180 || dto.Longitude > 180))
                throw new ArgumentException("Longitude must be between -180 and 180.");

            if (dto.AddressText != null && dto.AddressText.Length > 500)
                throw new ArgumentException("AddressText cannot be more than 500 characters.");
        }
    }
}