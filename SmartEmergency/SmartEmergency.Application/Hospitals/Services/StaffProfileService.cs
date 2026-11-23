using Microsoft.EntityFrameworkCore;
using SmartEmergency.Application.Common.Interfaces;
using SmartEmergency.Application.Hospitals.DTOs;
using SmartEmergency.Application.Hospitals.Interfaces;
using SmartEmergency.Application.Patients.Services;

namespace SmartEmergency.Application.Hospitals.Services;

public class StaffProfileService : IStaffProfileService
{
    private readonly IAppDbContext _db;

    public StaffProfileService(IAppDbContext db) => _db = db;

    // Doctor

    public Task<DoctorProfileDto?> GetDoctorAsync(int doctorId, CancellationToken ct = default) =>
        _db.Doctors.AsNoTracking()
            .Where(d => d.DoctorId == doctorId)
            .Select(d => new DoctorProfileDto
            {
                DoctorId = d.DoctorId,
                UserId = d.UserId,
                DepartmentId = d.DepartmentId,
                FullName = d.FullName,
                Specialization = d.Specialization,
                PhoneNumber = d.PhoneNumber,
                IsActive = d.IsActive
            })
            .FirstOrDefaultAsync(ct);

    public Task<List<DoctorProfileDto>> GetDoctorsByHospitalAsync(int hospitalId, CancellationToken ct = default) =>
        _db.Doctors.AsNoTracking()
            .Where(d => d.Department.HospitalId == hospitalId)
            .OrderBy(d => d.FullName)
            .Select(d => new DoctorProfileDto
            {
                DoctorId = d.DoctorId,
                UserId = d.UserId,
                DepartmentId = d.DepartmentId,
                FullName = d.FullName,
                Specialization = d.Specialization,
                PhoneNumber = d.PhoneNumber,
                IsActive = d.IsActive
            })
            .ToListAsync(ct);

    public async Task<DoctorProfileDto?> UpdateDoctorAsync(int doctorId, UpdateDoctorProfileDto dto, CancellationToken ct = default)
    {
        ValidationHelper.Validate(dto);

        var doctor = await _db.Doctors.FirstOrDefaultAsync(d => d.DoctorId == doctorId, ct);
        if (doctor is null) return null;

        var departmentExists = await _db.Departments.AnyAsync(d => d.DepartmentId == dto.DepartmentId, ct);
        if (!departmentExists)
            throw new InvalidOperationException("Department not found.");

        doctor.DepartmentId = dto.DepartmentId;
        doctor.FullName = dto.FullName.Trim();
        doctor.Specialization = dto.Specialization?.Trim();
        doctor.PhoneNumber = dto.PhoneNumber?.Trim();

        await _db.SaveChangesAsync(ct);
        return await GetDoctorAsync(doctorId, ct);
    }

    // Paramedic

    public Task<ParamedicProfileDto?> GetParamedicAsync(int paramedicId, CancellationToken ct = default) =>
        _db.Paramedics.AsNoTracking()
            .Where(p => p.ParamedicId == paramedicId)
            .Select(p => new ParamedicProfileDto
            {
                ParamedicId = p.ParamedicId,
                UserId = p.UserId,
                HospitalId = p.HospitalId,
                FullName = p.FullName,
                PhoneNumber = p.PhoneNumber,
                IsAvailable = p.IsAvailable,
                IsActive = p.IsActive
            })
            .FirstOrDefaultAsync(ct);

    public Task<List<ParamedicProfileDto>> GetParamedicsByHospitalAsync(int hospitalId, CancellationToken ct = default) =>
        _db.Paramedics.AsNoTracking()
            .Where(p => p.HospitalId == hospitalId)
            .OrderBy(p => p.FullName)
            .Select(p => new ParamedicProfileDto
            {
                ParamedicId = p.ParamedicId,
                UserId = p.UserId,
                HospitalId = p.HospitalId,
                FullName = p.FullName,
                PhoneNumber = p.PhoneNumber,
                IsAvailable = p.IsAvailable,
                IsActive = p.IsActive
            })
            .ToListAsync(ct);

    public async Task<ParamedicProfileDto?> UpdateParamedicAsync(int paramedicId, UpdateParamedicProfileDto dto, CancellationToken ct = default)
    {
        ValidationHelper.Validate(dto);

        var paramedic = await _db.Paramedics.FirstOrDefaultAsync(p => p.ParamedicId == paramedicId, ct);
        if (paramedic is null) return null;

        paramedic.FullName = dto.FullName.Trim();
        paramedic.PhoneNumber = dto.PhoneNumber?.Trim();
        paramedic.IsAvailable = dto.IsAvailable;

        await _db.SaveChangesAsync(ct);
        return await GetParamedicAsync(paramedicId, ct);
    }

    // Hospital staff

    public Task<HospitalStaffProfileDto?> GetHospitalStaffAsync(int hospitalStaffId, CancellationToken ct = default) =>
        _db.HospitalStaffs.AsNoTracking()
            .Where(s => s.HospitalStaffId == hospitalStaffId)
            .Select(s => new HospitalStaffProfileDto
            {
                HospitalStaffId = s.HospitalStaffId,
                UserId = s.UserId,
                HospitalId = s.HospitalId,
                FullName = s.FullName,
                PhoneNumber = s.PhoneNumber,
                IsActive = s.IsActive
            })
            .FirstOrDefaultAsync(ct);

    public Task<List<HospitalStaffProfileDto>> GetHospitalStaffByHospitalAsync(int hospitalId, CancellationToken ct = default) =>
        _db.HospitalStaffs.AsNoTracking()
            .Where(s => s.HospitalId == hospitalId)
            .OrderBy(s => s.FullName)
            .Select(s => new HospitalStaffProfileDto
            {
                HospitalStaffId = s.HospitalStaffId,
                UserId = s.UserId,
                HospitalId = s.HospitalId,
                FullName = s.FullName,
                PhoneNumber = s.PhoneNumber,
                IsActive = s.IsActive
            })
            .ToListAsync(ct);

    public async Task<HospitalStaffProfileDto?> UpdateHospitalStaffAsync(int hospitalStaffId, UpdateHospitalStaffProfileDto dto, CancellationToken ct = default)
    {
        ValidationHelper.Validate(dto);

        var staff = await _db.HospitalStaffs.FirstOrDefaultAsync(s => s.HospitalStaffId == hospitalStaffId, ct);
        if (staff is null) return null;

        staff.FullName = dto.FullName.Trim();
        staff.PhoneNumber = dto.PhoneNumber?.Trim();

        await _db.SaveChangesAsync(ct);
        return await GetHospitalStaffAsync(hospitalStaffId, ct);
    }
}