using System;
using System.Collections.Generic;
using System.Text;

namespace SmartEmergency.Domain.Entities
{
    public class Doctor
    {
        public int DoctorId { get; set; }
        public string UserId { get; set; } = default!;
        public int DepartmentId { get; set; } = default!;
        public string FullName { get; set; } = default!;
        public string? Specialization { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; } = true;

        //Navigation Properties
        public Department Department { get; set; } = default!;
        public ICollection<CaseDoctorAssignment> CaseDoctorAssignments { get; set; } = new List<CaseDoctorAssignment>();
        public ICollection<HospitalCaseAssessment> HospitalCaseAssessments { get; set; } = new List<HospitalCaseAssessment>();
        public ICollection<DoctorNote> DoctorNotes { get; set; } = new List<DoctorNote>();
        public ICollection<PatientTreatmentHistory> PatientTreatmentHistories { get; set; } = new List<PatientTreatmentHistory>();
    }
}
