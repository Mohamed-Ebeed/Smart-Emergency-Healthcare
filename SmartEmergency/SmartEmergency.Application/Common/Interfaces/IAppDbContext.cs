using Microsoft.EntityFrameworkCore;
using SmartEmergency.Domain.Entities;

namespace SmartEmergency.Application.Common.Interfaces
{
    public interface IAppDbContext
    {
        DbSet<Patient> Patients { get; }
        DbSet<MedicalProfile> MedicalProfiles { get; }
        DbSet<EmergencyContact> EmergencyContacts { get; }
        DbSet<ChronicDisease> ChronicDiseases { get; }
        DbSet<PatientChronicDisease> PatientChronicDiseases { get; }
        DbSet<Allergy> Allergies { get; }
        DbSet<PatientAllergy> PatientAllergies { get; }
        DbSet<Medication> Medications { get; }
        DbSet<PatientMedication> PatientMedications { get; }
        DbSet<EmergencyType> EmergencyTypes { get; }
        DbSet<EmergencyRequest> EmergencyRequests { get; }
        DbSet<AITriageResult> AITriageResults { get; }
        DbSet<EmergencyReviewDecision> EmergencyReviewDecisions { get; }
        DbSet<CaseStatusHistory> CaseStatusHistories { get; }
        DbSet<Hospital> Hospitals { get; }
        DbSet<Department> Departments { get; }
        DbSet<Doctor> Doctors { get; }
        DbSet<HospitalBed> HospitalBeds { get; }
        DbSet<EmergencyHospitalAssignment> EmergencyHospitalAssignments { get; }
        DbSet<CaseDepartmentAssignment> CaseDepartmentAssignments { get; }
        DbSet<CaseDoctorAssignment> CaseDoctorAssignments { get; }
        DbSet<HospitalCaseAssessment> HospitalCaseAssessments { get; }
        DbSet<Ambulance> Ambulances { get; }
        DbSet<AmbulanceAssignment> AmbulanceAssignments { get; }
        DbSet<AmbulanceLocationHistory> AmbulanceLocationHistories { get; }
        DbSet<AmbulanceStatusHistory> AmbulanceStatusHistories { get; }
        DbSet<Notification> Notifications { get; }
        DbSet<DoctorNote> DoctorNotes { get; }
        DbSet<PatientTreatmentHistory> PatientTreatmentHistories { get; }
        DbSet<Paramedic> Paramedics { get; }
        DbSet<HospitalStaff> HospitalStaffs { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}