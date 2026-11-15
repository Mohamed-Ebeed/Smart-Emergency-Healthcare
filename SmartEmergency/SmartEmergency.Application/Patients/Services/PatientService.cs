using Microsoft.EntityFrameworkCore;
using SmartEmergency.Application.Common.Interfaces;
using SmartEmergency.Application.Patients.DTOs;
using SmartEmergency.Application.Patients.Interfaces;

namespace SmartEmergency.Application.Patients.Services;

public class PatientService : IPatientService
{
    private readonly IAppDbContext _db;

    public PatientService(IAppDbContext db) => _db = db;

    public Task<PatientDto?> GetByIdAsync(int patientId, CancellationToken ct = default) =>
        _db.Patients.AsNoTracking()
            .Where(p => p.PatientId == patientId)
            .Select(p => new PatientDto
            {
                PatientId = p.PatientId,
                UserId = p.UserId,
                DateOfBirth = p.DateOfBirth,
                Gender = p.Gender,
                Address = p.Address,
                CreatedAt = p.CreatedAt
            })
            .FirstOrDefaultAsync(ct);

    public Task<PatientDto?> GetByUserIdAsync(string userId, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("UserId is required.", nameof(userId));

        return _db.Patients.AsNoTracking()
            .Where(p => p.UserId == userId)
            .Select(p => new PatientDto
            {
                PatientId = p.PatientId,
                UserId = p.UserId,
                DateOfBirth = p.DateOfBirth,
                Gender = p.Gender,
                Address = p.Address,
                CreatedAt = p.CreatedAt
            })
            .FirstOrDefaultAsync(ct);
    }

    public async Task<PatientDto?> UpdateAsync(int patientId, UpdatePatientDto dto, CancellationToken ct = default)
    {
        ValidationHelper.Validate(dto);

        var patient = await _db.Patients.FirstOrDefaultAsync(p => p.PatientId == patientId, ct);
        if (patient is null) return null;

        patient.DateOfBirth = dto.DateOfBirth;
        patient.Gender = dto.Gender;
        patient.Address = dto.Address?.Trim();

        await _db.SaveChangesAsync(ct);
        return await GetByIdAsync(patientId, ct);
    }
}
