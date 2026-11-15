using SmartEmergency.Application.Patients.DTOs;

namespace SmartEmergency.Application.Patients.Interfaces;

public interface IMedicalProfileService
{
    // ---- Medical Profile (one-to-one مع Patient) ----
    Task<MedicalProfileDto?> GetAsync(int patientId, CancellationToken ct = default);
    Task<MedicalProfileDto> CreateAsync(int patientId, CreateMedicalProfileDto dto, CancellationToken ct = default);
    Task<MedicalProfileDto> UpdateAsync(int patientId, UpdateMedicalProfileDto dto, CancellationToken ct = default);

    // ---- Chronic Diseases ----
    Task<IReadOnlyList<PatientChronicDiseaseDto>> GetChronicDiseasesAsync(int patientId, CancellationToken ct = default);
    Task<PatientChronicDiseaseDto> AddChronicDiseaseAsync(int patientId, AddPatientChronicDiseaseDto dto, CancellationToken ct = default);
    Task<PatientChronicDiseaseDto?> UpdateChronicDiseaseAsync(int patientId, int chronicDiseaseId, UpdatePatientChronicDiseaseDto dto, CancellationToken ct = default);
    Task<bool> RemoveChronicDiseaseAsync(int patientId, int chronicDiseaseId, CancellationToken ct = default);

    // ---- Allergies ----
    Task<IReadOnlyList<PatientAllergyDto>> GetAllergiesAsync(int patientId, CancellationToken ct = default);
    Task<PatientAllergyDto> AddAllergyAsync(int patientId, AddPatientAllergyDto dto, CancellationToken ct = default);
    Task<PatientAllergyDto?> UpdateAllergyAsync(int patientId, int allergyId, UpdatePatientAllergyDto dto, CancellationToken ct = default);
    Task<bool> RemoveAllergyAsync(int patientId, int allergyId, CancellationToken ct = default);

    // ---- Medications ----
    Task<IReadOnlyList<PatientMedicationDto>> GetMedicationsAsync(int patientId, CancellationToken ct = default);
    Task<PatientMedicationDto> AddMedicationAsync(int patientId, AddPatientMedicationDto dto, CancellationToken ct = default);
    Task<PatientMedicationDto?> UpdateMedicationAsync(int patientId, int medicationId, UpdatePatientMedicationDto dto, CancellationToken ct = default);
    Task<bool> RemoveMedicationAsync(int patientId, int medicationId, CancellationToken ct = default);

    // ---- Emergency Contacts ----
    Task<IReadOnlyList<EmergencyContactDto>> GetEmergencyContactsAsync(int patientId, CancellationToken ct = default);
    Task<EmergencyContactDto> AddEmergencyContactAsync(int patientId, SaveEmergencyContactDto dto, CancellationToken ct = default);
    Task<EmergencyContactDto?> UpdateEmergencyContactAsync(int patientId, int emergencyContactId, SaveEmergencyContactDto dto, CancellationToken ct = default);
    Task<bool> RemoveEmergencyContactAsync(int patientId, int emergencyContactId, CancellationToken ct = default);
    Task<bool> SetPrimaryEmergencyContactAsync(int patientId, int emergencyContactId, CancellationToken ct = default);
}
