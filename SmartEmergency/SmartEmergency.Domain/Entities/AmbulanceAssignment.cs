using System;
using System.Collections.Generic;
using System.Text;

namespace SmartEmergency.Domain.Entities
{
    public class AmbulanceAssignment
    {
        public long AmbulanceAssignmentId { get; set; }
        public long EmergencyRequestId { get; set; }
        public int AmbulanceId { get; set; }
        public int ParamedicId { get; set; }
        public string? AssignedByUserId { get; set; }
        public DateTime AssignedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string AssignmentStatus { get; set; } = default!;

        public EmergencyRequest EmergencyRequest { get; set; } = default!;
        public Ambulance Ambulance { get; set; } = default!;
        public Paramedic Paramedic { get; set; } = default!;
    }
}
