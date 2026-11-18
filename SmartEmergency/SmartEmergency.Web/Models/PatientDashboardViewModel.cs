using SmartEmergency.Application.Patients.DTOs;

namespace SmartEmergency.Web.Models;

// بيجمع كل بيانات صفحة المريض في Model واحد للـ View
public class PatientDashboardViewModel
{
    public PatientDto Patient { get; set; } = null!;
    public MedicalProfileDto? Profile { get; set; }
    public IReadOnlyList<PatientChronicDiseaseDto> ChronicDiseases { get; set; } = Array.Empty<PatientChronicDiseaseDto>();
    public IReadOnlyList<PatientAllergyDto> Allergies { get; set; } = Array.Empty<PatientAllergyDto>();
    public IReadOnlyList<PatientMedicationDto> Medications { get; set; } = Array.Empty<PatientMedicationDto>();
    public IReadOnlyList<EmergencyContactDto> EmergencyContacts { get; set; } = Array.Empty<EmergencyContactDto>();
}
