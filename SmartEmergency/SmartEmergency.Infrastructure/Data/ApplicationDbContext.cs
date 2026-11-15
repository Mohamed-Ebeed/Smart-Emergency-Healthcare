
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using SmartEmergency.Application.Common.Interfaces;
using SmartEmergency.Domain.Entities;
using SmartEmergency.Domain.Enums;
using SmartEmergency.Infrastructure.Identity;

using AppRoles = SmartEmergency.Domain.Common.Roles;

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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ==========================================
            // Patient Relationships
            // ==========================================

            builder.Entity<Patient>()
                .HasOne(p => p.MedicalProfile)
                .WithOne(m => m.Patient)
                .HasForeignKey<MedicalProfile>(m => m.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<EmergencyContact>()
                .HasOne(ec => ec.Patient)
                .WithMany(p => p.EmergencyContacts)
                .HasForeignKey(ec => ec.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<EmergencyRequest>()
                .HasOne(er => er.Patient)
                .WithMany(p => p.EmergencyRequests)
                .HasForeignKey(er => er.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            // ==========================================
            // Patient Medical Data (Many-to-Many)
            // ==========================================

            builder.Entity<PatientChronicDisease>()
                .HasKey(x => new { x.PatientId, x.ChronicDiseaseId });

            builder.Entity<PatientChronicDisease>()
                .HasOne(x => x.Patient)
                .WithMany(p => p.PatientChronicDiseases)
                .HasForeignKey(x => x.PatientId);

            builder.Entity<PatientChronicDisease>()
                .HasOne(x => x.ChronicDisease)
                .WithMany(c => c.PatientChronicDiseases)
                .HasForeignKey(x => x.ChronicDiseaseId);

            builder.Entity<PatientAllergy>()
                .HasKey(x => new { x.PatientId, x.AllergyId });

            builder.Entity<PatientAllergy>()
                .HasOne(x => x.Patient)
                .WithMany(p => p.PatientAllergies)
                .HasForeignKey(x => x.PatientId);

            builder.Entity<PatientAllergy>()
                .HasOne(x => x.Allergy)
                .WithMany(a => a.PatientAllergies)
                .HasForeignKey(x => x.AllergyId);

            builder.Entity<PatientMedication>()
                .HasKey(x => new { x.PatientId, x.MedicationId });

            builder.Entity<PatientMedication>()
                .HasOne(x => x.Patient)
                .WithMany(p => p.PatientMedications)
                .HasForeignKey(x => x.PatientId);

            builder.Entity<PatientMedication>()
                .HasOne(x => x.Medication)
                .WithMany(m => m.PatientMedications)
                .HasForeignKey(x => x.MedicationId);

            // ==========================================
            // ApplicationUser Relationships
            // ==========================================

            builder.Entity<Patient>(e =>
            {
                e.Property(x => x.UserId).HasMaxLength(450);
                e.HasIndex(x => x.UserId).IsUnique();

                e.HasOne<ApplicationUser>()
                    .WithOne()
                    .HasForeignKey<Patient>(x => x.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Doctor>(e =>
            {
                e.Property(x => x.UserId).HasMaxLength(450);
                e.HasIndex(x => x.UserId).IsUnique();

                e.HasOne<ApplicationUser>()
                    .WithOne()
                    .HasForeignKey<Doctor>(x => x.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Paramedic>(e =>
            {
                e.Property(x => x.UserId).HasMaxLength(450);
                e.HasIndex(x => x.UserId).IsUnique();

                e.HasOne<ApplicationUser>()
                    .WithOne()
                    .HasForeignKey<Paramedic>(x => x.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<HospitalStaff>(e =>
            {
                e.Property(x => x.UserId).HasMaxLength(450);
                e.HasIndex(x => x.UserId).IsUnique();

                e.HasOne<ApplicationUser>()
                    .WithOne()
                    .HasForeignKey<HospitalStaff>(x => x.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ==========================================
            // Hospital Relationships
            // ==========================================

            builder.Entity<Department>()
                .HasOne(x => x.Hospital)
                .WithMany(h => h.Departments)
                .HasForeignKey(x => x.HospitalId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<EmergencyHospitalAssignment>()
                .HasOne(x => x.Hospital)
                .WithMany(h => h.EmergencyHospitalAssignments)
                .HasForeignKey(x => x.HospitalId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<HospitalCaseAssessment>()
                .HasOne(x => x.Hospital)
                .WithMany(h => h.HospitalCaseAssessments)
                .HasForeignKey(x => x.HospitalId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Ambulance>()
                .HasOne(x => x.Hospital)
                .WithMany(h => h.Ambulances)
                .HasForeignKey(x => x.HospitalId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Paramedic>()
                .HasOne(x => x.Hospital)
                .WithMany(h => h.Paramedics)
                .HasForeignKey(x => x.HospitalId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<HospitalStaff>()
                .HasOne(x => x.Hospital)
                .WithMany(h => h.HospitalStaffs)
                .HasForeignKey(x => x.HospitalId)
                .OnDelete(DeleteBehavior.Restrict);

            // ==========================================
            // Department Relationships
            // ==========================================

            builder.Entity<Doctor>()
                .HasOne(x => x.Department)
                .WithMany(d => d.Doctors)
                .HasForeignKey(x => x.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<HospitalBed>()
                .HasOne(x => x.Department)
                .WithMany(d => d.HospitalBeds)
                .HasForeignKey(x => x.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CaseDepartmentAssignment>()
                .HasOne(x => x.Department)
                .WithMany(d => d.CaseDepartmentAssignments)
                .HasForeignKey(x => x.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // ==========================================
            // EmergencyType Relationships
            // ==========================================

            builder.Entity<EmergencyRequest>()
                .HasOne(x => x.EmergencyType)
                .WithMany(t => t.EmergencyRequests)
                .HasForeignKey(x => x.EmergencyTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AITriageResult>()
                .HasOne(x => x.SuggestedEmergencyType)
                .WithMany(t => t.AITriageResults)
                .HasForeignKey(x => x.SuggestedEmergencyTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<EmergencyReviewDecision>()
                .HasOne(x => x.ConfirmedEmergencyType)
                .WithMany(t => t.EmergencyReviewDecisions)
                .HasForeignKey(x => x.ConfirmedEmergencyTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // ==========================================
            // EmergencyRequest Relationships
            // ==========================================

            builder.Entity<AITriageResult>()
                .HasOne(x => x.EmergencyRequest)
                .WithMany(r => r.AITriageResults)
                .HasForeignKey(x => x.EmergencyRequestId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<EmergencyReviewDecision>()
                .HasOne(x => x.EmergencyRequest)
                .WithMany(r => r.EmergencyReviewDecisions)
                .HasForeignKey(x => x.EmergencyRequestId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CaseStatusHistory>()
                .HasOne(x => x.EmergencyRequest)
                .WithMany(r => r.CaseStatusHistories)
                .HasForeignKey(x => x.EmergencyRequestId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<EmergencyHospitalAssignment>()
                .HasOne(x => x.EmergencyRequest)
                .WithMany(r => r.EmergencyHospitalAssignments)
                .HasForeignKey(x => x.EmergencyRequestId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CaseDepartmentAssignment>()
                .HasOne(x => x.EmergencyRequest)
                .WithMany(r => r.CaseDepartmentAssignments)
                .HasForeignKey(x => x.EmergencyRequestId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CaseDoctorAssignment>()
                .HasOne(x => x.EmergencyRequest)
                .WithMany(r => r.CaseDoctorAssignments)
                .HasForeignKey(x => x.EmergencyRequestId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<HospitalCaseAssessment>()
                .HasOne(x => x.EmergencyRequest)
                .WithMany(r => r.HospitalCaseAssessments)
                .HasForeignKey(x => x.EmergencyRequestId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AmbulanceAssignment>()
                .HasOne(x => x.EmergencyRequest)
                .WithMany(r => r.AmbulanceAssignments)
                .HasForeignKey(x => x.EmergencyRequestId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AmbulanceLocationHistory>()
                .HasOne(x => x.EmergencyRequest)
                .WithMany(r => r.AmbulanceLocationHistories)
                .HasForeignKey(x => x.EmergencyRequestId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<DoctorNote>()
                .HasOne(x => x.EmergencyRequest)
                .WithMany(r => r.DoctorNotes)
                .HasForeignKey(x => x.EmergencyRequestId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PatientTreatmentHistory>()
                .HasOne(x => x.EmergencyRequest)
                .WithMany(r => r.PatientTreatmentHistories)
                .HasForeignKey(x => x.EmergencyRequestId)
                .OnDelete(DeleteBehavior.Restrict);

            // ==========================================
            // AI Triage Relationships
            // ==========================================

            builder.Entity<AITriageResult>()
                .HasOne(x => x.SuggestedDepartment)
                .WithMany(d => d.AITriageResults)
                .HasForeignKey(x => x.SuggestedDepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // ==========================================
            // Doctor Relationships
            // ==========================================

            builder.Entity<CaseDoctorAssignment>()
                .HasOne(x => x.Doctor)
                .WithMany(d => d.CaseDoctorAssignments)
                .HasForeignKey(x => x.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<HospitalCaseAssessment>()
                .HasOne(x => x.AssessedByDoctor)
                .WithMany(d => d.HospitalCaseAssessments)
                .HasForeignKey(x => x.AssessedByDoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<DoctorNote>()
                .HasOne(x => x.Doctor)
                .WithMany(d => d.DoctorNotes)
                .HasForeignKey(x => x.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PatientTreatmentHistory>()
                .HasOne(x => x.Doctor)
                .WithMany(d => d.PatientTreatmentHistories)
                .HasForeignKey(x => x.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PatientTreatmentHistory>()
                .HasOne(x => x.Patient)
                .WithMany(p => p.PatientTreatmentHistories)
                .HasForeignKey(x => x.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            // ==========================================
            // Ambulance Relationships
            // ==========================================

            builder.Entity<AmbulanceAssignment>()
                .HasOne(x => x.Ambulance)
                .WithMany(a => a.AmbulanceAssignments)
                .HasForeignKey(x => x.AmbulanceId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AmbulanceLocationHistory>()
                .HasOne(x => x.Ambulance)
                .WithMany(a => a.AmbulanceLocationHistories)
                .HasForeignKey(x => x.AmbulanceId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AmbulanceStatusHistory>()
                .HasOne(x => x.Ambulance)
                .WithMany(a => a.AmbulanceStatusHistories)
                .HasForeignKey(x => x.AmbulanceId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AmbulanceAssignment>()
                .HasOne(x => x.Paramedic)
                .WithMany(p => p.AmbulanceAssignments)
                .HasForeignKey(x => x.ParamedicId)
                .OnDelete(DeleteBehavior.Restrict);

            // ==========================================
            // Audit User Relationships
            // ==========================================

            builder.Entity<EmergencyReviewDecision>(e =>
            {
                e.Property(x => x.OperatorId).HasMaxLength(450);

                e.HasOne<ApplicationUser>()
                    .WithMany()
                    .HasForeignKey(x => x.OperatorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<EmergencyRequest>(e =>
            {
                e.Property(x => x.ClosedByUserId).HasMaxLength(450);

                e.HasOne<ApplicationUser>()
                    .WithMany()
                    .HasForeignKey(x => x.ClosedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<CaseStatusHistory>(e =>
            {
                e.Property(x => x.ChangedByUserId).HasMaxLength(450);

                e.HasOne<ApplicationUser>()
                    .WithMany()
                    .HasForeignKey(x => x.ChangedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<EmergencyHospitalAssignment>(e =>
            {
                e.Property(x => x.AssignedByOperatorId).HasMaxLength(450);

                e.HasOne<ApplicationUser>()
                    .WithMany()
                    .HasForeignKey(x => x.AssignedByOperatorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<CaseDepartmentAssignment>(e =>
            {
                e.Property(x => x.AssignedByUserId).HasMaxLength(450);

                e.HasOne<ApplicationUser>()
                    .WithMany()
                    .HasForeignKey(x => x.AssignedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<CaseDoctorAssignment>(e =>
            {
                e.Property(x => x.AssignedByUserId).HasMaxLength(450);

                e.HasOne<ApplicationUser>()
                    .WithMany()
                    .HasForeignKey(x => x.AssignedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<AmbulanceAssignment>(e =>
            {
                e.Property(x => x.AssignedByUserId).HasMaxLength(450);

                e.HasOne<ApplicationUser>()
                    .WithMany()
                    .HasForeignKey(x => x.AssignedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ==========================================
            // Notification Relationships
            // ==========================================

            builder.Entity<Notification>(e =>
            {
                e.Property(x => x.RecipientUserId)
                    .HasMaxLength(450);

                e.Property(x => x.Channel)
                    .HasConversion<string>()
                    .HasMaxLength(50);

                e.HasOne<ApplicationUser>()
                    .WithMany()
                    .HasForeignKey(x => x.RecipientUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.EmergencyContact)
                    .WithMany(x => x.Notifications)
                    .HasForeignKey(x => x.EmergencyContactId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.EmergencyRequest)
                    .WithMany(x => x.Notifications)
                    .HasForeignKey(x => x.EmergencyRequestId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.ToTable(t => t.HasCheckConstraint(
                    "CK_Notifications_Recipient",
                    "([RecipientUserId] IS NOT NULL AND [EmergencyContactId] IS NULL) OR " +
                    "([RecipientUserId] IS NULL AND [EmergencyContactId] IS NOT NULL)"
                ));
            });

            // ==========================================
            // Decimal Precision
            // ==========================================

            builder.Entity<Hospital>(e =>
            {
                e.Property(x => x.Latitude).HasPrecision(9, 6);
                e.Property(x => x.Longitude).HasPrecision(9, 6);
            });

            builder.Entity<Ambulance>(e =>
            {
                e.Property(x => x.LastKnownLatitude).HasPrecision(9, 6);
                e.Property(x => x.LastKnownLongitude).HasPrecision(9, 6);
            });

            builder.Entity<EmergencyRequest>(e =>
            {
                e.Property(x => x.Latitude).HasPrecision(9, 6);
                e.Property(x => x.Longitude).HasPrecision(9, 6);
            });

            builder.Entity<AmbulanceLocationHistory>(e =>
            {
                e.Property(x => x.Latitude).HasPrecision(9, 6);
                e.Property(x => x.Longitude).HasPrecision(9, 6);
            });

            builder.Entity<AITriageResult>()
                .Property(x => x.ConfidenceScore)
                .HasPrecision(5, 4);

            // ==========================================
            // Enum Conversions (Stored as Strings)
            // ==========================================

            builder.Entity<Patient>()
                .Property(x => x.Gender)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Entity<EmergencyRequest>(e =>
            {
                e.Property(x => x.CurrentStatus)
                    .HasConversion<string>()
                    .HasMaxLength(50);

                e.Property(x => x.Priority)
                    .HasConversion<string>()
                    .HasMaxLength(50);
            });

            builder.Entity<CaseStatusHistory>()
                .Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Entity<EmergencyType>()
                .Property(x => x.DefaultPriority)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Entity<AITriageResult>()
                .Property(x => x.SuggestedPriority)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Entity<EmergencyReviewDecision>()
                .Property(x => x.ConfirmedPriority)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Entity<HospitalCaseAssessment>()
                .Property(x => x.AssessedPriority)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Entity<EmergencyHospitalAssignment>()
                .Property(x => x.AssignmentStatus)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Entity<CaseDepartmentAssignment>()
                .Property(x => x.AssignmentStatus)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Entity<CaseDoctorAssignment>()
                .Property(x => x.AssignmentStatus)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Entity<AmbulanceAssignment>()
                .Property(x => x.AssignmentStatus)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Entity<HospitalBed>()
                .Property(x => x.BedStatus)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Entity<Ambulance>()
                .Property(x => x.AmbulanceStatus)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Entity<AmbulanceStatusHistory>()
                .Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(50);

            // ==========================================
            // Unique Indexes and Lengths
            // ==========================================

            builder.Entity<ChronicDisease>(e =>
            {
                e.Property(x => x.Name).HasMaxLength(150);
                e.HasIndex(x => x.Name).IsUnique();
            });

            builder.Entity<Allergy>(e =>
            {
                e.Property(x => x.Name).HasMaxLength(150);
                e.HasIndex(x => x.Name).IsUnique();
            });

            builder.Entity<EmergencyType>(e =>
            {
                e.Property(x => x.Name).HasMaxLength(100);
                e.HasIndex(x => x.Name).IsUnique();
            });

            builder.Entity<Ambulance>(e =>
            {
                e.Property(x => x.VehicleNumber).HasMaxLength(50);
                e.HasIndex(x => x.VehicleNumber).IsUnique();
            });

            builder.Entity<Department>(e =>
            {
                e.Property(x => x.Name).HasMaxLength(150);
                e.HasIndex(x => new { x.HospitalId, x.Name }).IsUnique();
            });

            builder.Entity<HospitalBed>(e =>
            {
                e.Property(x => x.BedNumber).HasMaxLength(50);
                e.HasIndex(x => new { x.DepartmentId, x.BedNumber }).IsUnique();
            });

            // ==========================================
            // Performance Indexes
            // ==========================================

            builder.Entity<EmergencyRequest>(e =>
            {
                e.HasIndex(x => x.CurrentStatus);
                e.HasIndex(x => x.CreatedAt);
            });

            builder.Entity<AmbulanceLocationHistory>()
                .HasIndex(x => new { x.AmbulanceId, x.RecordedAt });

            // ==========================================
            // Seed Identity Roles
            // ==========================================

            builder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = "b1f0c9a0-0001-4a00-9000-000000000001",
                    Name = AppRoles.Patient,
                    NormalizedName = AppRoles.Patient.ToUpper(),
                    ConcurrencyStamp = "role-patient-stamp"
                },
                new IdentityRole
                {
                    Id = "b1f0c9a0-0002-4a00-9000-000000000002",
                    Name = AppRoles.Operator,
                    NormalizedName = AppRoles.Operator.ToUpper(),
                    ConcurrencyStamp = "role-operator-stamp"
                },
                new IdentityRole
                {
                    Id = "b1f0c9a0-0003-4a00-9000-000000000003",
                    Name = AppRoles.HospitalStaff,
                    NormalizedName = AppRoles.HospitalStaff.ToUpper(),
                    ConcurrencyStamp = "role-hospital-staff-stamp"
                },
                new IdentityRole
                {
                    Id = "b1f0c9a0-0004-4a00-9000-000000000004",
                    Name = AppRoles.Paramedic,
                    NormalizedName = AppRoles.Paramedic.ToUpper(),
                    ConcurrencyStamp = "role-paramedic-stamp"
                },
                new IdentityRole
                {
                    Id = "b1f0c9a0-0005-4a00-9000-000000000005",
                    Name = AppRoles.Doctor,
                    NormalizedName = AppRoles.Doctor.ToUpper(),
                    ConcurrencyStamp = "role-doctor-stamp"
                },
                new IdentityRole
                {
                    Id = "b1f0c9a0-0006-4a00-9000-000000000006",
                    Name = AppRoles.Admin,
                    NormalizedName = AppRoles.Admin.ToUpper(),
                    ConcurrencyStamp = "role-admin-stamp"
                }
            );
        }
    }
}
