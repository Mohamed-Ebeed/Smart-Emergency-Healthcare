using System.ComponentModel.DataAnnotations;

namespace SmartEmergency.Application.Patients.DTOs;

// ================= Medical Profile =================
public class MedicalProfileDto
{
    public int MedicalProfileId { get; set; }
    public int PatientId { get; set; }
    public string? BloodType { get; set; }
    public string? PrimaryConditions { get; set; }
    public string? CurrentNotes { get; set; }
    public DateTime LastUpdatedAt { get; set; }
}

public class CreateMedicalProfileDto
{
    [StringLength(3)]
    [RegularExpression(@"^(A|B|AB|O)[+-]$", ErrorMessage = "Blood type must be one of: A+, A-, B+, B-, AB+, AB-, O+, O-.")]
    public string? BloodType { get; set; }

    [StringLength(500)]
    public string? PrimaryConditions { get; set; }

    [StringLength(1000)]
    public string? CurrentNotes { get; set; }
}

public class UpdateMedicalProfileDto : CreateMedicalProfileDto { }

// ================= Chronic Diseases =================
public class PatientChronicDiseaseDto
{
    public int ChronicDiseaseId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateOnly? DiagnosedAt { get; set; }
    public string? Notes { get; set; }
}

// الحقول اللي ينفع تتعدل بعد الإضافة (الجدول الوسيط PatientChronicDisease)
public class UpdatePatientChronicDiseaseDto : IValidatableObject
{
    public DateOnly? DiagnosedAt { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext context)
    {
        if (DiagnosedAt is { } d && d > DateOnly.FromDateTime(DateTime.UtcNow))
            yield return new ValidationResult("Diagnosis date cannot be in the future.", new[] { nameof(DiagnosedAt) });
    }
}

public class AddPatientChronicDiseaseDto : UpdatePatientChronicDiseaseDto
{
    [Required, StringLength(150, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }   // بتتسجل في الـ Catalog لو المرض جديد
}

// ================= Allergies =================
public class PatientAllergyDto
{
    public int AllergyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Severity { get; set; }
    public string? Reaction { get; set; }
}

public class UpdatePatientAllergyDto
{
    [StringLength(50)]
    public string? Severity { get; set; }

    [StringLength(300)]
    public string? Reaction { get; set; }
}

public class AddPatientAllergyDto : UpdatePatientAllergyDto
{
    [Required, StringLength(150, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }
}

// ================= Medications =================
public class PatientMedicationDto
{
    public int MedicationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Dosage { get; set; }
    public string? Frequency { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
}

public class UpdatePatientMedicationDto : IValidatableObject
{
    [StringLength(100)]
    public string? Dosage { get; set; }

    [StringLength(100)]
    public string? Frequency { get; set; }

    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext context)
    {
        if (StartDate is { } s && EndDate is { } e && e < s)
            yield return new ValidationResult("End date cannot be before start date.", new[] { nameof(EndDate) });
    }
}

public class AddPatientMedicationDto : UpdatePatientMedicationDto
{
    [Required, StringLength(150, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }
}

// ================= Emergency Contacts =================
public class EmergencyContactDto
{
    public int EmergencyContactId { get; set; }
    public int PatientId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Relationship { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
}

public class SaveEmergencyContactDto
{
    [Required, StringLength(150, MinimumLength = 2)]
    public string FullName { get; set; } = string.Empty;

    [StringLength(50)]
    public string? Relationship { get; set; }

    [Required]
    [RegularExpression(@"^\+?[0-9]{8,15}$", ErrorMessage = "Phone number must be 8-15 digits, optionally starting with +.")]
    public string PhoneNumber { get; set; } = string.Empty;

    public bool IsPrimary { get; set; }
}
