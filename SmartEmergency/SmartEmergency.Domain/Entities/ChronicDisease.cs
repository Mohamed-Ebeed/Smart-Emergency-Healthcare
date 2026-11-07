using System;
using System.Collections.Generic;
using System.Text;

namespace SmartEmergency.Domain.Entities
{
    public class ChronicDisease
    {
        public int ChronicDiseaseId { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }

        public ICollection<PatientChronicDisease> PatientChronicDiseases { get; set; }
        = new List<PatientChronicDisease>();

    }
}
