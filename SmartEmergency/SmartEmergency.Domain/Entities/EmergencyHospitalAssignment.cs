using System;
using System.Collections.Generic;
using System.Text;

namespace SmartEmergency.Domain.Entities
{
    public class EmergencyHospitalAssignment
    {
        public long EmergencyHospitalAssignmentId { get; set; }
        public long EmergencyRequestId { get; set; }
        public int HospitalId { get; set; }
        public string? AssignedByOperatorId { get; set; }
        public string AssignmentStatus { get; set; } = default!;
        public DateTime AssignedAt { get; set; }
        public DateTime? RespondedAt { get; set; }
        public string? ResponseNotes { get; set; }

        public EmergencyRequest EmergencyRequest { get; set; } = default!;
        public Hospital Hospital { get; set; } = default!;

    }
}
