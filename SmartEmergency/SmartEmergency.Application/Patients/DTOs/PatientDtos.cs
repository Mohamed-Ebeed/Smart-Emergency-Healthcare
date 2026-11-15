using System.ComponentModel.DataAnnotations;
using SmartEmergency.Domain.Enums;

namespace SmartEmergency.Application.Patients.DTOs;

public class PatientDto
{
    public int PatientId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateOnly? DateOfBirth { get; set; }
    public Gender? Gender { get; set; }
    public string? Address { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class UpdatePatientDto : IValidatableObject
{
    public DateOnly? DateOfBirth { get; set; }

    [EnumDataType(typeof(Gender))]
    public Gender? Gender { get; set; }

    [StringLength(300)]
    public string? Address { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext context)
    {
        if (DateOfBirth is { } dob && dob > DateOnly.FromDateTime(DateTime.UtcNow))
            yield return new ValidationResult("Date of birth cannot be in the future.", new[] { nameof(DateOfBirth) });
    }
}
