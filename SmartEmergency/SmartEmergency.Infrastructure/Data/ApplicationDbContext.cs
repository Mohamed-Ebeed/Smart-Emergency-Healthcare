
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartEmergency.Application.Common.Interfaces;
using SmartEmergency.Domain.Entities;
using SmartEmergency.Infrastructure.Identity;
using SmartEmergency.Domain.Common;
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

            // ===== Patient - ChronicDisease (Many-to-Many) =====
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

            // ===== Patient - Allergy (Many-to-Many) =====
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

            // ===== Patient - Medication (Many-to-Many) =====
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

            // ===== Patient - MedicalProfile (One-to-One) =====
            builder.Entity<Patient>()
                .HasOne(p => p.MedicalProfile)
                .WithOne(m => m.Patient)
                .HasForeignKey<MedicalProfile>(m => m.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            // ===== UserId -> ApplicationUser (Patient, Doctor, Paramedic, HospitalStaff) =====
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
            // ===== Decimal precision =====
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
                .Property(x => x.ConfidenceScore).HasPrecision(5, 4);

            // ===== Enums stored as strings =====
            builder.Entity<Patient>()
                .Property(x => x.Gender).HasConversion<string>();

            builder.Entity<EmergencyRequest>(e =>
            {
                e.Property(x => x.CurrentStatus).HasConversion<string>();
                e.Property(x => x.Priority).HasConversion<string>();
            });

            builder.Entity<CaseStatusHistory>()
                .Property(x => x.Status).HasConversion<string>();

            builder.Entity<EmergencyType>()
                .Property(x => x.DefaultPriority).HasConversion<string>();

            builder.Entity<AITriageResult>()
                .Property(x => x.SuggestedPriority).HasConversion<string>();

            builder.Entity<EmergencyReviewDecision>()
                .Property(x => x.ConfirmedPriority).HasConversion<string>();

            builder.Entity<HospitalCaseAssessment>()
                .Property(x => x.AssessedPriority).HasConversion<string>();

            builder.Entity<EmergencyHospitalAssignment>()
                .Property(x => x.AssignmentStatus).HasConversion<string>();

            builder.Entity<CaseDepartmentAssignment>()
                .Property(x => x.AssignmentStatus).HasConversion<string>();

            builder.Entity<CaseDoctorAssignment>()
                .Property(x => x.AssignmentStatus).HasConversion<string>();

            builder.Entity<AmbulanceAssignment>()
                .Property(x => x.AssignmentStatus).HasConversion<string>();

            builder.Entity<HospitalBed>()
                .Property(x => x.BedStatus).HasConversion<string>();

            builder.Entity<Ambulance>()
                .Property(x => x.AmbulanceStatus).HasConversion<string>();

            builder.Entity<AmbulanceStatusHistory>()
                .Property(x => x.Status).HasConversion<string>();

            // ===== Unique indexes (لازم MaxLength عشان SQL Server يقبل الـ Index) =====
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

            // ===== Notification: لازم مستخدم أو جهة اتصال =====
            builder.Entity<Notification>(e =>
            {
                e.Property(x => x.RecipientUserId).HasMaxLength(450);
                e.ToTable(t => t.HasCheckConstraint(
                    "CK_Notifications_Recipient",
                    "[RecipientUserId] IS NOT NULL OR [EmergencyContactId] IS NOT NULL"));
            });

            // ===== Indexes للأداء =====
            builder.Entity<EmergencyRequest>(e =>
            {
                e.HasIndex(x => x.CurrentStatus);
                e.HasIndex(x => x.CreatedAt);
            });

            builder.Entity<AmbulanceLocationHistory>()
                .HasIndex(x => new { x.AmbulanceId, x.RecordedAt });
            // ===== كل علاقات الـ Domain: Restrict بدل Cascade =====
            // (عشان SQL Server ميرفضش الـ Cascade المتعدد، ومتتمسحش History بالغلط)
            //foreach (var fk in builder.Model.GetEntityTypes()
            //    .Where(t => t.ClrType.Namespace == "SmartEmergency.Domain.Entities")
            //    .SelectMany(t => t.GetForeignKeys()))
            //{
            //    fk.DeleteBehavior = DeleteBehavior.Restrict;
            //}

            // ===== Seed للأدوار الستة =====
            builder.Entity<IdentityRole>().HasData(
               new IdentityRole { Id = "b1f0c9a0-0001-4a00-9000-000000000001", Name = AppRoles.Patient, NormalizedName = AppRoles.Patient.ToUpper() },
               new IdentityRole { Id = "b1f0c9a0-0002-4a00-9000-000000000002", Name = AppRoles.Operator, NormalizedName = AppRoles.Operator.ToUpper() },
               new IdentityRole { Id = "b1f0c9a0-0003-4a00-9000-000000000003", Name = AppRoles.HospitalStaff, NormalizedName = AppRoles.HospitalStaff.ToUpper() },
               new IdentityRole { Id = "b1f0c9a0-0004-4a00-9000-000000000004", Name = AppRoles.Paramedic, NormalizedName = AppRoles.Paramedic.ToUpper() },
               new IdentityRole { Id = "b1f0c9a0-0005-4a00-9000-000000000005", Name = AppRoles.Doctor, NormalizedName = AppRoles.Doctor.ToUpper() },
               new IdentityRole { Id = "b1f0c9a0-0006-4a00-9000-000000000006", Name = AppRoles.Admin, NormalizedName = AppRoles.Admin.ToUpper() }
           );
        }
    }
}
    