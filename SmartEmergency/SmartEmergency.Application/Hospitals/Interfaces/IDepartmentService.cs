using SmartEmergency.Application.Hospitals.DTOs;

namespace SmartEmergency.Application.Hospitals.Interfaces;

public interface IDepartmentService
{
    Task<DepartmentDto> CreateAsync(CreateDepartmentDto dto, CancellationToken ct = default);
    Task<DepartmentDto?> UpdateAsync(int departmentId, UpdateDepartmentDto dto, CancellationToken ct = default);
    Task<List<DepartmentDto>> GetByHospitalAsync(int hospitalId, bool activeOnly = false, CancellationToken ct = default);
    Task<bool> SetActiveAsync(int departmentId, bool isActive, CancellationToken ct = default);
}