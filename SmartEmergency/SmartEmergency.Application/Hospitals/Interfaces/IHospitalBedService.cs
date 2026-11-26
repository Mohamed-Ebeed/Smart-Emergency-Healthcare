using SmartEmergency.Application.Hospitals.DTOs;
using SmartEmergency.Domain.Enums;

namespace SmartEmergency.Application.Hospitals.Interfaces;

public interface IHospitalBedService
{
    Task<HospitalBedDto> CreateAsync(CreateHospitalBedDto dto, CancellationToken ct = default);
    Task<HospitalBedDto?> UpdateAsync(int hospitalBedId, UpdateHospitalBedDto dto, CancellationToken ct = default);
    Task<List<HospitalBedDto>> GetByHospitalAsync(int hospitalId, int? departmentId = null, CancellationToken ct = default);
    Task<bool> ChangeStatusAsync(int hospitalBedId, BedStatus status, CancellationToken ct = default);
}