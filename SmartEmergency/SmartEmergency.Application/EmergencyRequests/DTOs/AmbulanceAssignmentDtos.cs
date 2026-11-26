using System.ComponentModel.DataAnnotations;
using SmartEmergency.Domain.Enums;

namespace SmartEmergency.Application.EmergencyRequests.DTOs;

public class AmbulanceAssignmentDto
{
    public long AmbulanceAssignmentId { get; set; }
    public long EmergencyRequestId { get; set; }
    public int AmbulanceId { get; set; }
    public string VehicleNumber { get; set; } = string.Empty;
    public int ParamedicId { get; set; }
    public string ParamedicName { get; set; } = string.Empty;
    public string? AssignedByUserId { get; set; }
    public AssignmentStatus AssignmentStatus { get; set; }
    public DateTime AssignedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public class AssignAmbulanceDto
{
    [Range(1, int.MaxValue)]
    public int AmbulanceId { get; set; }

    [Range(1, int.MaxValue)]
    public int ParamedicId { get; set; }
}