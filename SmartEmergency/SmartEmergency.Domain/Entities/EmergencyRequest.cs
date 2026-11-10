using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using SmartEmergency.Domain.Enums;

namespace SmartEmergency.Domain.Entities
{
    public class EmergencyRequest
    {
        public long EmergencyRequestId { get; set; }
        public int PatientId { get; set; }
        public int? EmergencyTypeId { get; set; }
        public string? Description { get; set; }
        public Decimal? Latitude { get; set; }
        public Decimal? Longitude { get; set; }
        public string? AddressText { get; set; }
        public Priority? Priority { get; set; }
        public RequestStatus CurrentStatus { get; set; } = RequestStatus.Submitted;
        public DateTime CreatedAt { get; set; }
        public DateTime? PickedUpAt { get; set; }
        public DateTime? ArrivedAtHospitalAt { get; set; }
        public DateTime? ClosureRequestedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public string? ClosedByUserId { get; set; }


        //Navigation Properties
        public Patient Patient { get; set; } = default!;
        public EmergencyType? EmergencyType { get; set; }
        public ICollection<AITriageResult> AITriageResults { get; set; } = new List<AITriageResult>();
        public ICollection<EmergencyReviewDecision> EmergencyReviewDecisions { get; set; } = new List<EmergencyReviewDecision>();
        public ICollection<CaseStatusHistory> CaseStatusHistories { get; set; } = new List<CaseStatusHistory>();
        public ICollection<EmergencyHospitalAssignment> EmergencyHospitalAssignments { get; set; } = new List<EmergencyHospitalAssignment>();
        public ICollection<CaseDepartmentAssignment> CaseDepartmentAssignments { get; set; } = new List<CaseDepartmentAssignment>();
        public ICollection<CaseDoctorAssignment> CaseDoctorAssignments { get; set; } = new List<CaseDoctorAssignment>();
        public ICollection<HospitalCaseAssessment> HospitalCaseAssessments { get; set; } = new List<HospitalCaseAssessment>();
        public ICollection<AmbulanceAssignment> AmbulanceAssignments { get; set; } = new List<AmbulanceAssignment>();
        public ICollection<AmbulanceLocationHistory> AmbulanceLocationHistories { get; set; } = new List<AmbulanceLocationHistory>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public ICollection<DoctorNote> DoctorNotes { get; set; } = new List<DoctorNote>();
        public ICollection<PatientTreatmentHistory> PatientTreatmentHistories { get; set; } = new List<PatientTreatmentHistory>();
    }
}
