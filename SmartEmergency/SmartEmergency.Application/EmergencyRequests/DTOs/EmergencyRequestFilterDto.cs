using System.ComponentModel.DataAnnotations;
using SmartEmergency.Domain.Enums;

namespace SmartEmergency.Application.EmergencyRequests.DTOs;

public class EmergencyRequestFilterDto : IValidatableObject
{
    [EnumDataType(typeof(RequestStatus))]
    public RequestStatus? Status { get; set; }

    [EnumDataType(typeof(Priority))]
    public Priority? Priority { get; set; }

    public DateOnly? FromDate { get; set; }
    public DateOnly? ToDate { get; set; }

    [Range(1, int.MaxValue)]
    public int PageNumber { get; set; } = 1;

    [Range(1, 100)]
    public int PageSize { get; set; } = 10;

    public IEnumerable<ValidationResult> Validate(ValidationContext context)
    {
        if (FromDate is { } from && ToDate is { } to && to < from)
            yield return new ValidationResult("ToDate can't be before FromDate.", new[] { nameof(ToDate) });
    }
}