using System;
using System.Collections.Generic;
using System.Text;

namespace SmartEmergency.Domain.Entities
{
    public class PatientTreatmentHistory
    {
        public long TreatmentHistoryId { get; set; }
        public long EmergencyRequestId { get; set; }
        public int PatientId { get; set; }
        public int? DoctorId { get; set; }
        public string TreatmentDetails { get; set; } = default!;
        public DateTime RecordedAt { get; set; } = DateTime.UtcNow;


        public EmergencyRequest EmergencyRequest { get; set; } = default!;
        public Patient Patient { get; set; } = default!;
        public Doctor? Doctor { get; set; }
    }
}
