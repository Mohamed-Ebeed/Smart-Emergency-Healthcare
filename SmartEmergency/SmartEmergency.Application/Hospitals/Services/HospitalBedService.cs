using Microsoft.EntityFrameworkCore;
using SmartEmergency.Application.Common.Interfaces;
using SmartEmergency.Application.Hospitals.DTOs;
using SmartEmergency.Application.Hospitals.Interfaces;
using SmartEmergency.Application.Patients.Services;
using SmartEmergency.Domain.Entities;
using SmartEmergency.Domain.Enums;

namespace SmartEmergency.Application.Hospitals.Services;

public class HospitalBedService : IHospitalBedService
{
    private readonly IAppDbContext _db;

    public HospitalBedService(IAppDbContext db) => _db = db;

    public async Task<HospitalBedDto> CreateAsync(CreateHospitalBedDto dto, CancellationToken ct = default)
    {
        ValidationHelper.Validate(dto);

        var departmentExists = await _db.Departments.AnyAsync(d => d.DepartmentId == dto.DepartmentId, ct);
        if (!departmentExists)
            throw new InvalidOperationException("Department not found.");

        // Bed number can't be repeated in the same department
        var bedNumber = dto.BedNumber.Trim();
        var numberUsed = await _db.HospitalBeds.AnyAsync(b => b.DepartmentId == dto.DepartmentId && b.BedNumber == bedNumber, ct);
        if (numberUsed)
            throw new InvalidOperationException("This bed number already exists in this department.");

        var bed = new HospitalBed
        {
            DepartmentId = dto.DepartmentId,
            BedNumber = bedNumber,
            BedStatus = BedStatus.Available
        };

        _db.HospitalBeds.Add(bed);
        await _db.SaveChangesAsync(ct);

        return ToDto(bed);
    }

    public async Task<HospitalBedDto?> UpdateAsync(int hospitalBedId, UpdateHospitalBedDto dto, CancellationToken ct = default)
    {
        ValidationHelper.Validate(dto);

        var bed = await _db.HospitalBeds.FirstOrDefaultAsync(b => b.HospitalBedId == hospitalBedId, ct);
        if (bed is null) return null;

        var bedNumber = dto.BedNumber.Trim();
        var numberUsed = await _db.HospitalBeds.AnyAsync(b =>
            b.DepartmentId == bed.DepartmentId && b.BedNumber == bedNumber && b.HospitalBedId != hospitalBedId, ct);
        if (numberUsed)
            throw new InvalidOperationException("This bed number already exists in this department.");

        bed.BedNumber = bedNumber;

        await _db.SaveChangesAsync(ct);
        return ToDto(bed);
    }

    // All beds of a hospital, or only one department if departmentId is sent
    public Task<List<HospitalBedDto>> GetByHospitalAsync(int hospitalId, int? departmentId = null, CancellationToken ct = default) =>
        _db.HospitalBeds.AsNoTracking()
            .Where(b => b.Department.HospitalId == hospitalId)
            .Where(b => departmentId == null || b.DepartmentId == departmentId)
            .OrderBy(b => b.DepartmentId).ThenBy(b => b.BedNumber)
            .Select(b => new HospitalBedDto
            {
                HospitalBedId = b.HospitalBedId,
                DepartmentId = b.DepartmentId,
                BedNumber = b.BedNumber,
                BedStatus = b.BedStatus
            })
            .ToListAsync(ct);

    public async Task<bool> ChangeStatusAsync(int hospitalBedId, BedStatus status, CancellationToken ct = default)
    {
        if (!Enum.IsDefined(status))
            throw new InvalidOperationException("Invalid bed status.");

        var bed = await _db.HospitalBeds.FirstOrDefaultAsync(b => b.HospitalBedId == hospitalBedId, ct);
        if (bed is null) return false;

        bed.BedStatus = status;
        await _db.SaveChangesAsync(ct);
        return true;
    }

    private static HospitalBedDto ToDto(HospitalBed b) => new()
    {
        HospitalBedId = b.HospitalBedId,
        DepartmentId = b.DepartmentId,
        BedNumber = b.BedNumber,
        BedStatus = b.BedStatus
    };
}