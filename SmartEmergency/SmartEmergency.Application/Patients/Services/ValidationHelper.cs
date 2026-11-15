using System.ComponentModel.DataAnnotations;

namespace SmartEmergency.Application.Patients.Services;

// بيشغل الـ DataAnnotations (و IValidatableObject) اللي على الـ DTOs
// ولو فيه خطأ بيرمي ValidationException والـ Controller يحوله لرسالة للمستخدم
internal static class ValidationHelper
{
    public static void Validate(object dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var results = new List<ValidationResult>();
        if (!Validator.TryValidateObject(dto, new ValidationContext(dto), results, validateAllProperties: true))
            throw new ValidationException(string.Join(" | ", results.Select(r => r.ErrorMessage)));
    }
}
