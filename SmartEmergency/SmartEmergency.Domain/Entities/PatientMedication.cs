using System;
using System.Collections.Generic;
using System.Text;

namespace SmartEmergency.Domain.Entities
{
    public class PatientMedication
    {
        public int PatientId { get; set; }
        public int MedicationId { get; set; }
        public string? Dosage { get; set; }
        public string? Frequency { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }


        // Navigation Properties
        public Patient Patient { get; set; } = default!;
        public Medication Medication { get; set; } = default!;
    }
}
