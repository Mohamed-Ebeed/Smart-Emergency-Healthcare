using SmartEmergency.Domain.Enums;

namespace SmartEmergency.Application.EmergencyRequests.DTOs;

public class CaseStatusHistoryDto
{
    public long CaseStatusHistoryId { get; set; }
    public RequestStatus Status { get; set; }
    public string? ChangedByUserId { get; set; }
    public string? Notes { get; set; }
    public DateTime ChangedAt { get; set; }
}