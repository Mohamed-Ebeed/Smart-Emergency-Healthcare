using SmartEmergency.Application.Hospitals.DTOs;
using SmartEmergency.Domain.Enums;

namespace SmartEmergency.Application.Hospitals.Interfaces;

public interface IAmbulanceService
{
    Task<AmbulanceDto> CreateAsync(CreateAmbulanceDto dto, CancellationToken ct = default);
    Task<AmbulanceDto?> UpdateAsync(int ambulanceId, UpdateAmbulanceDto dto, CancellationToken ct = default);
    Task<AmbulanceDto?> GetByIdAsync(int ambulanceId, CancellationToken ct = default);
    Task<List<AmbulanceDto>> GetByHospitalAsync(int hospitalId, CancellationToken ct = default);
    Task<bool> SetActiveAsync(int ambulanceId, bool isActive, CancellationToken ct = default);
    Task<bool> ChangeStatusAsync(int ambulanceId, AmbulanceStatus status, CancellationToken ct = default);
    Task<bool> UpdateLocationAsync(int ambulanceId, UpdateAmbulanceLocationDto dto, CancellationToken ct = default);
}