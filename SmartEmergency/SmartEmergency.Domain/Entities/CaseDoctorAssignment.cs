using System;
using System.Collections.Generic;
using System.Text;
using SmartEmergency.Domain.Enums;
namespace SmartEmergency.Domain.Entities
{
    public class CaseDoctorAssignment
    {
        public long CaseDoctorAssignmentId { get; set; }
        public long EmergencyRequestId { get; set; }
        public int DoctorId { get; set; }
        public string? AssignedByUserId { get; set; }
        public DateTime AssignedAt { get; set; }
        public AssignmentStatus AssignmentStatus { get; set; } = default!;

        public Doctor Doctor { get; set; } = default!;
        public EmergencyRequest EmergencyRequest { get; set; } = default!;
    }
}
