using Microsoft.EntityFrameworkCore;
using SmartEmergency.Application.Common.Interfaces;
using SmartEmergency.Application.Hospitals.DTOs;
using SmartEmergency.Application.Hospitals.Interfaces;
using SmartEmergency.Application.Patients.Services;
using SmartEmergency.Domain.Entities;

namespace SmartEmergency.Application.Hospitals.Services;

public class HospitalService : IHospitalService
{
    private readonly IAppDbContext _db;

    public HospitalService(IAppDbContext db) => _db = db;

    public async Task<HospitalDto> CreateAsync(CreateHospitalDto dto, CancellationToken ct = default)
    {
        ValidationHelper.Validate(dto);

        var hospital = new Hospital
        {
            Name = dto.Name.Trim(),
            Address = dto.Address.Trim(),
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            PhoneNumber = dto.PhoneNumber?.Trim(),
            IsActive = true
        };

        _db.Hospitals.Add(hospital);
        await _db.SaveChangesAsync(ct);

        return ToDto(hospital);
    }

    public async Task<HospitalDto?> UpdateAsync(int hospitalId, UpdateHospitalDto dto, CancellationToken ct = default)
    {
        ValidationHelper.Validate(dto);

        var hospital = await _db.Hospitals.FirstOrDefaultAsync(h => h.HospitalId == hospitalId, ct);
        if (hospital is null) return null;

        hospital.Name = dto.Name.Trim();
        hospital.Address = dto.Address.Trim();
        hospital.Latitude = dto.Latitude;
        hospital.Longitude = dto.Longitude;
        hospital.PhoneNumber = dto.PhoneNumber?.Trim();

        await _db.SaveChangesAsync(ct);
        return ToDto(hospital);
    }

    public Task<HospitalDto?> GetByIdAsync(int hospitalId, CancellationToken ct = default) =>
        _db.Hospitals.AsNoTracking()
            .Where(h => h.HospitalId == hospitalId)
            .Select(h => new HospitalDto
            {
                HospitalId = h.HospitalId,
                Name = h.Name,
                Address = h.Address,
                Latitude = h.Latitude,
                Longitude = h.Longitude,
                PhoneNumber = h.PhoneNumber,
                IsActive = h.IsActive
            })
            .FirstOrDefaultAsync(ct);

    public Task<List<HospitalDto>> GetAllAsync(bool activeOnly = false, CancellationToken ct = default) =>
        _db.Hospitals.AsNoTracking()
            .Where(h => !activeOnly || h.IsActive)
            .OrderBy(h => h.Name)
            .Select(h => new HospitalDto
            {
                HospitalId = h.HospitalId,
                Name = h.Name,
                Address = h.Address,
                Latitude = h.Latitude,
                Longitude = h.Longitude,
                PhoneNumber = h.PhoneNumber,
                IsActive = h.IsActive
            })
            .ToListAsync(ct);

    // Deactivate instead of delete, so old cases keep their hospital
    public async Task<bool> SetActiveAsync(int hospitalId, bool isActive, CancellationToken ct = default)
    {
        var hospital = await _db.Hospitals.FirstOrDefaultAsync(h => h.HospitalId == hospitalId, ct);
        if (hospital is null) return false;

        hospital.IsActive = isActive;
        await _db.SaveChangesAsync(ct);
        return true;
    }

    private static HospitalDto ToDto(Hospital h) => new()
    {
        HospitalId = h.HospitalId,
        Name = h.Name,
        Address = h.Address,
        Latitude = h.Latitude,
        Longitude = h.Longitude,
        PhoneNumber = h.PhoneNumber,
        IsActive = h.IsActive
    };
}