using System;
using System.Collections.Generic;
using System.Text;

namespace SmartEmergency.Domain.Entities
{
    public class Allergy
    {
        public int AllergyId { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }

        //Navigation Property

        public ICollection<PatientAllergy> PatientAllergies { get; set; }
        = new List<PatientAllergy>();
    }
}
