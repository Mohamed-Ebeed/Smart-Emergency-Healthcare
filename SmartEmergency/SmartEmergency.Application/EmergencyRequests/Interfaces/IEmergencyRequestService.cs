using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SmartEmergency.Application.EmergencyRequests.DTOs;
using SmartEmergency.Domain.Enums;

namespace SmartEmergency.Application.EmergencyRequests.Interfaces
{
    public interface IEmergencyRequestService
    {
        Task<EmergencyRequestDto> CreateAsync(CreateEmergencyRequestDto dto, CancellationToken cancellationToken = default);
        Task<EmergencyRequestDto> GetByIdAsync(long requestId, CancellationToken cancellationToken = default);
        Task<List<EmergencyRequestDto>> GetByPatientAsync(int patientId, CancellationToken cancellationToken = default);
        Task<bool> ChangeStatusAsync(long requestId, RequestStatus newStatus, string userId);
        Task CancelAsync(long requestId, string userId, CancellationToken cancellationToken = default);
    }
}