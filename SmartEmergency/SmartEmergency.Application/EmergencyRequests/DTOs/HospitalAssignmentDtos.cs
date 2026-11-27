using System.ComponentModel.DataAnnotations;
using SmartEmergency.Domain.Enums;

namespace SmartEmergency.Application.EmergencyRequests.DTOs;

public class HospitalAssignmentDto
{
    public long EmergencyHospitalAssignmentId { get; set; }
    public long EmergencyRequestId { get; set; }
    public int HospitalId { get; set; }
    public string HospitalName { get; set; } = string.Empty;
    public AssignmentStatus AssignmentStatus { get; set; }
    public string? AssignedByOperatorId { get; set; }
    public DateTime AssignedAt { get; set; }
    public DateTime? RespondedAt { get; set; }
    public string? ResponseNotes { get; set; }
}

public class SendToHospitalDto
{
    [Range(1, int.MaxValue)]
    public int HospitalId { get; set; }
}

public class HospitalResponseDto
{
    [Required]
    public bool? Accept { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }
}