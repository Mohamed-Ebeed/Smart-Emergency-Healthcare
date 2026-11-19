
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using SmartEmergency.Application.Common.Interfaces;
using SmartEmergency.Domain.Entities;
using SmartEmergency.Infrastructure.Identity;

namespace SmartEmergency.Infrastructure.Data
{
    public class ApplicationDbContext
        : IdentityDbContext<ApplicationUser>, IAppDbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // ==========================================
        // DbSets
        // ==========================================

        public DbSet<Patient> Patients => Set<Patient>();
        public DbSet<MedicalProfile> MedicalProfiles => Set<MedicalProfile>();
        public DbSet<EmergencyContact> EmergencyContacts => Set<EmergencyContact>();

        public DbSet<ChronicDisease> ChronicDiseases => Set<ChronicDisease>();
        public DbSet<PatientChronicDisease> PatientChronicDiseases => Set<PatientChronicDisease>();

        public DbSet<Allergy> Allergies => Set<Allergy>();
        public DbSet<PatientAllergy> PatientAllergies => Set<PatientAllergy>();

        public DbSet<Medication> Medications => Set<Medication>();
        public DbSet<PatientMedication> PatientMedications => Set<PatientMedication>();

        public DbSet<EmergencyType> EmergencyTypes => Set<EmergencyType>();
        public DbSet<EmergencyRequest> EmergencyRequests => Set<EmergencyRequest>();

        public DbSet<AITriageResult> AITriageResults => Set<AITriageResult>();
        public DbSet<EmergencyReviewDecision> EmergencyReviewDecisions => Set<EmergencyReviewDecision>();
        public DbSet<CaseStatusHistory> CaseStatusHistories => Set<CaseStatusHistory>();

        public DbSet<Hospital> Hospitals => Set<Hospital>();
        public DbSet<Department> Departments => Set<Department>();
        public DbSet<Doctor> Doctors => Set<Doctor>();
        public DbSet<HospitalBed> HospitalBeds => Set<HospitalBed>();

        public DbSet<EmergencyHospitalAssignment> EmergencyHospitalAssignments => Set<EmergencyHospitalAssignment>();
        public DbSet<CaseDepartmentAssignment> CaseDepartmentAssignments => Set<CaseDepartmentAssignment>();
        public DbSet<CaseDoctorAssignment> CaseDoctorAssignments => Set<CaseDoctorAssignment>();
        public DbSet<HospitalCaseAssessment> HospitalCaseAssessments => Set<HospitalCaseAssessment>();

        public DbSet<Ambulance> Ambulances => Set<Ambulance>();
        public DbSet<AmbulanceAssignment> AmbulanceAssignments => Set<AmbulanceAssignment>();
        public DbSet<AmbulanceLocationHistory> AmbulanceLocationHistories => Set<AmbulanceLocationHistory>();
        public DbSet<AmbulanceStatusHistory> AmbulanceStatusHistories => Set<AmbulanceStatusHistory>();

        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<DoctorNote> DoctorNotes => Set<DoctorNote>();
        public DbSet<PatientTreatmentHistory> PatientTreatmentHistories => Set<PatientTreatmentHistory>();

        public DbSet<Paramedic> Paramedics => Set<Paramedic>();
        public DbSet<HospitalStaff> HospitalStaffs => Set<HospitalStaff>();

        // ==========================================
        // Entity Configurations
        // ==========================================

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(
                typeof(ApplicationDbContext).Assembly);
        }
    }
}
