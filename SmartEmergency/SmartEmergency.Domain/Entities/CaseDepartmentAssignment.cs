using System;
using System.Collections.Generic;
using System.Text;
using SmartEmergency.Domain.Enums;
namespace SmartEmergency.Domain.Entities
{
    public class CaseDepartmentAssignment
    {
        public long CaseDepartmentAssignmentId { get; set; }
        public long EmergencyRequestId { get; set; }
        public int DepartmentId { get; set; }
        public string? AssignedByUserId { get; set; }
        public DateTime AssignedAt { get; set; }
        public AssignmentStatus AssignmentStatus { get; set; } = default!;

        public Department Department { get; set; } = default!;
        public EmergencyRequest EmergencyRequest { get; set; } = default!;

    }
}
