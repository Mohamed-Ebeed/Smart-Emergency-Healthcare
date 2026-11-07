using System;
using System.Collections.Generic;
using System.Text;

namespace SmartEmergency.Domain.Entities
{
    public class PatientChronicDisease
    {
        public int PatientId { get; set; }
        public int ChronicDiseaseId { get; set; }
        public DateOnly? DiagnosedAt { get; set; }
        public string? Notes { get; set; }

        public Patient Patient { get; set; } = default!; //Navigation property
        public ChronicDisease ChronicDisease { get; set; } = default!; //Navigation property
    }
}
