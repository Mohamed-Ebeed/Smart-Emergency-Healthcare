using System;
using System.Collections.Generic;
using System.Text;
using SmartEmergency.Domain.Enums;
namespace SmartEmergency.Domain.Entities
{
    public class EmergencyHospitalAssignment
    {
        public long EmergencyHospitalAssignmentId { get; set; }
        public long EmergencyRequestId { get; set; }
        public int HospitalId { get; set; }
        public string? AssignedByOperatorId { get; set; }
        public AssignmentStatus AssignmentStatus { get; set; } = default!;
        public DateTime AssignedAt { get; set; }
        public DateTime? RespondedAt { get; set; }
        public string? ResponseNotes { get; set; }

        public EmergencyRequest EmergencyRequest { get; set; } = default!;
        public Hospital Hospital { get; set; } = default!;

    }
}
