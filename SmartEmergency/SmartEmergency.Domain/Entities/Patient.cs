using System;
using System.Collections.Generic;
using System.Text;

namespace SmartEmergency.Domain.Entities
{
    public class Patient
    {
        public int PatientId { get; set; }
        public string UserId { get; set; } = default!;
        public DateOnly? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public DateTime CreatedAt { get; set; }

        public MedicalProfile? MedicalProfile { get; set; } = default!; //Navigation property

        public ICollection<EmergencyContact> EmergencyContacts { get; set; }
        = new List<EmergencyContact>();

        public ICollection<PatientChronicDisease> PatientChronicDiseases { get; set; }
        = new List<PatientChronicDisease>();
        public ICollection<PatientMedication> PatientMedications { get; set; }
        = new List<PatientMedication>();
        public ICollection<EmergencyRequest> EmergencyRequests { get; set; }
        = new List<EmergencyRequest>();
        public ICollection<PatientTreatmentHistory> PatientTreatmentHistories { get; set; }
        = new List<PatientTreatmentHistory>();
        public ICollection<PatientAllergy> PatientAllergies { get; set; }
        = new List<PatientAllergy>();
    }
}
