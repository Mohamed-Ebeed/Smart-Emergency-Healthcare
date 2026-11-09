using System;
using System.Collections.Generic;
using System.Text;

namespace SmartEmergency.Domain.Entities
{
    public class PatientAllergie
    {
        public int PatientId { get; set; }
        public int AllergyId { get; set; }
        public string? Severity { get; set; }
        public string? Reaction { get; set; }

        //Navigation Properties
        public Patient Patient { get; set; } = default!;
        public Allergie Allergie { get; set; } = default!;
    }
}
