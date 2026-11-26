using SmartEmergency.Application.Hospitals.DTOs;

namespace SmartEmergency.Application.Hospitals.Interfaces;

public interface IHospitalService
{
    Task<HospitalDto> CreateAsync(CreateHospitalDto dto, CancellationToken ct = default);
    Task<HospitalDto?> UpdateAsync(int hospitalId, UpdateHospitalDto dto, CancellationToken ct = default);
    Task<HospitalDto?> GetByIdAsync(int hospitalId, CancellationToken ct = default);
    Task<List<HospitalDto>> GetAllAsync(bool activeOnly = false, CancellationToken ct = default);
    Task<bool> SetActiveAsync(int hospitalId, bool isActive, CancellationToken ct = default);
}