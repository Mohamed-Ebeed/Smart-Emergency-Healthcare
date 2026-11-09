using System;
using System.Collections.Generic;
using System.Text;

namespace SmartEmergency.Domain.Entities
{
    public class HospitalCaseAssessment
    {
        public long HospitalCaseAssessmentId { get; set; }
        public long EmergencyRequestId { get; set; }
        public int HospitalId { get; set; }
        public int AssessedByDoctorId { get; set; }
        public string? AssessedPriority { get; set; }
        public string? AssessmentNotes { get; set; }
        public DateTime AssessedAt { get; set; }

        public EmergencyRequest EmergencyRequest { get; set; } = default!;
        public Hospital Hospital { get; set; } = default!;
        public Doctor AssessedByDoctor { get; set; } = default!;
    }
}
