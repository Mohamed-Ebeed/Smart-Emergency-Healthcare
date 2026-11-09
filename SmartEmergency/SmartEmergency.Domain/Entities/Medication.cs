using System;
using System.Collections.Generic;
using System.Text;

namespace SmartEmergency.Domain.Entities
{
    public class Medication
    {
        public int MedicationId { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }


        public ICollection<PatientMedication> PatientMedications { get; set; }
        = new List<PatientMedication>();
    }
}
