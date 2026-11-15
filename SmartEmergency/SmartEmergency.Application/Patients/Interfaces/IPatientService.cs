using SmartEmergency.Application.Patients.DTOs;

namespace SmartEmergency.Application.Patients.Interfaces;

public interface IPatientService
{
    Task<PatientDto?> GetByIdAsync(int patientId, CancellationToken ct = default);
    Task<PatientDto?> GetByUserIdAsync(string userId, CancellationToken ct = default);
    Task<PatientDto?> UpdateAsync(int patientId, UpdatePatientDto dto, CancellationToken ct = default);
}
