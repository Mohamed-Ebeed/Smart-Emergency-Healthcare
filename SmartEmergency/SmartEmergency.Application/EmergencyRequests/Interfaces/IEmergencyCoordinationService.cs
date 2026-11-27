
using SmartEmergency.Application.Common.Models;
using SmartEmergency.Application.EmergencyRequests.DTOs;

namespace SmartEmergency.Application.EmergencyRequests.Interfaces;

public interface IEmergencyCoordinationService
{
    Task<PagedResult<EmergencyRequestDto>> SearchAsync(
        EmergencyRequestFilterDto filter,
        CancellationToken ct = default);

    Task EnsureCanViewRequestAsync(
        long requestId,
        string userId,
        string userRole,
        CancellationToken ct = default);

    Task<List<CaseStatusHistoryDto>> GetStatusHistoryAsync(
        long requestId,
        string userId,
        string userRole,
        CancellationToken ct = default);

    Task<HospitalAssignmentDto> SendToHospitalAsync(
        long requestId,
        SendToHospitalDto dto,
        string operatorUserId,
        CancellationToken ct = default);

    Task<HospitalAssignmentDto> RespondToHospitalAssignmentAsync(
        long assignmentId,
        HospitalResponseDto dto,
        string userId,
        bool isAdmin,
        CancellationToken ct = default);

    Task<List<HospitalAssignmentDto>> GetHospitalAssignmentsAsync(
        long requestId,
        string userId,
        string userRole,
        CancellationToken ct = default);

    Task<NextHospitalDto> GetNextHospitalAsync(
        long requestId,
        CancellationToken ct = default);

    Task<HospitalAssignmentDto> SendToNextHospitalAsync(
        long requestId,
        string operatorUserId,
        CancellationToken ct = default);

    Task<AmbulanceAssignmentDto> AssignAmbulanceAsync(
        long requestId,
        AssignAmbulanceDto dto,
        string operatorUserId,
        CancellationToken ct = default);

    Task<List<AmbulanceAssignmentDto>> GetAmbulanceAssignmentsAsync(
        long requestId,
        string userId,
        string userRole,
        CancellationToken ct = default);

    Task<AmbulanceAssignmentDto> GetAmbulanceAssignmentAsync(
        long assignmentId,
        string userId,
        string userRole,
        CancellationToken ct = default);

    Task<List<AmbulanceAssignmentDto>> GetMyAmbulanceAssignmentsAsync(
        string paramedicUserId,
        CancellationToken ct = default);
}
