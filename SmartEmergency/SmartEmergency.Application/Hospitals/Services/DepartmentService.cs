using Microsoft.EntityFrameworkCore;
using SmartEmergency.Application.Common.Interfaces;
using SmartEmergency.Application.Hospitals.DTOs;
using SmartEmergency.Application.Hospitals.Interfaces;
using SmartEmergency.Application.Patients.Services;
using SmartEmergency.Domain.Entities;

namespace SmartEmergency.Application.Hospitals.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IAppDbContext _db;

    public DepartmentService(IAppDbContext db) => _db = db;

    public async Task<DepartmentDto> CreateAsync(CreateDepartmentDto dto, CancellationToken ct = default)
    {
        ValidationHelper.Validate(dto);

        // Department must belong to an existing hospital
        var hospitalExists = await _db.Hospitals.AnyAsync(h => h.HospitalId == dto.HospitalId, ct);
        if (!hospitalExists)
            throw new InvalidOperationException("Hospital not found.");

        // Same name can't be repeated in the same hospital
        var name = dto.Name.Trim();
        var nameUsed = await _db.Departments.AnyAsync(d => d.HospitalId == dto.HospitalId && d.Name == name, ct);
        if (nameUsed)
            throw new InvalidOperationException("This department already exists in this hospital.");

        var department = new Department
        {
            HospitalId = dto.HospitalId,
            Name = name,
            Description = dto.Description?.Trim(),
            Capacity = dto.Capacity,
            IsActive = true
        };

        _db.Departments.Add(department);
        await _db.SaveChangesAsync(ct);

        return ToDto(department);
    }

    public async Task<DepartmentDto?> UpdateAsync(int departmentId, UpdateDepartmentDto dto, CancellationToken ct = default)
    {
        ValidationHelper.Validate(dto);

        var department = await _db.Departments.FirstOrDefaultAsync(d => d.DepartmentId == departmentId, ct);
        if (department is null) return null;

        var name = dto.Name.Trim();
        var nameUsed = await _db.Departments.AnyAsync(d =>
            d.HospitalId == department.HospitalId && d.Name == name && d.DepartmentId != departmentId, ct);
        if (nameUsed)
            throw new InvalidOperationException("This department already exists in this hospital.");

        department.Name = name;
        department.Description = dto.Description?.Trim();
        department.Capacity = dto.Capacity;

        await _db.SaveChangesAsync(ct);
        return ToDto(department);
    }

    public Task<List<DepartmentDto>> GetByHospitalAsync(int hospitalId, bool activeOnly = false, CancellationToken ct = default) =>
        _db.Departments.AsNoTracking()
            .Where(d => d.HospitalId == hospitalId && (!activeOnly || d.IsActive))
            .OrderBy(d => d.Name)
            .Select(d => new DepartmentDto
            {
                DepartmentId = d.DepartmentId,
                HospitalId = d.HospitalId,
                Name = d.Name,
                Description = d.Description,
                Capacity = d.Capacity,
                IsActive = d.IsActive
            })
            .ToListAsync(ct);

    public async Task<bool> SetActiveAsync(int departmentId, bool isActive, CancellationToken ct = default)
    {
        var department = await _db.Departments.FirstOrDefaultAsync(d => d.DepartmentId == departmentId, ct);
        if (department is null) return false;

        department.IsActive = isActive;
        await _db.SaveChangesAsync(ct);
        return true;
    }

    private static DepartmentDto ToDto(Department d) => new()
    {
        DepartmentId = d.DepartmentId,
        HospitalId = d.HospitalId,
        Name = d.Name,
        Description = d.Description,
        Capacity = d.Capacity,
        IsActive = d.IsActive
    };
}