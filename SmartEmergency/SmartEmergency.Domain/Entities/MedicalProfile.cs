using System;
using System.Collections.Generic;
using System.Text;

namespace SmartEmergency.Domain.Entities
{
    public class MedicalProfile
    {
        public int MedicalProfileId { get; set; }
        public int PatientId { get; set; }
        public string? BloodType { get; set; }
        public string? PrimaryConditions { get; set; }
        public string? CurrentNotes { get; set; }
        public DateTime LastUpdatedAt { get; set; }

        public Patient Patient { get; set; } = default!; //Navigation property
    }
}
