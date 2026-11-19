using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartEmergency.Domain.Entities;
using SmartEmergency.Infrastructure.Identity;

namespace SmartEmergency.Infrastructure.Data.Configurations
{
    public class EmergencyRequestConfiguration
        : IEntityTypeConfiguration<EmergencyRequest>
    {
        public void Configure(EntityTypeBuilder<EmergencyRequest> builder)
        {
            // Emergency Type Relationship
            builder.HasOne(x => x.EmergencyType)
                .WithMany(t => t.EmergencyRequests)
                .HasForeignKey(x => x.EmergencyTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Related Entities
            builder.HasMany(x => x.AITriageResults)
                .WithOne(a => a.EmergencyRequest)
                .HasForeignKey(a => a.EmergencyRequestId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.EmergencyReviewDecisions)
                .WithOne(d => d.EmergencyRequest)
                .HasForeignKey(d => d.EmergencyRequestId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.CaseStatusHistories)
                .WithOne(h => h.EmergencyRequest)
                .HasForeignKey(h => h.EmergencyRequestId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.EmergencyHospitalAssignments)
                .WithOne(a => a.EmergencyRequest)
                .HasForeignKey(a => a.EmergencyRequestId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.CaseDepartmentAssignments)
                .WithOne(a => a.EmergencyRequest)
                .HasForeignKey(a => a.EmergencyRequestId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.CaseDoctorAssignments)
                .WithOne(a => a.EmergencyRequest)
                .HasForeignKey(a => a.EmergencyRequestId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.HospitalCaseAssessments)
                .WithOne(a => a.EmergencyRequest)
                .HasForeignKey(a => a.EmergencyRequestId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.AmbulanceAssignments)
                .WithOne(a => a.EmergencyRequest)
                .HasForeignKey(a => a.EmergencyRequestId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.AmbulanceLocationHistories)
                .WithOne(h => h.EmergencyRequest)
                .HasForeignKey(h => h.EmergencyRequestId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.DoctorNotes)
                .WithOne(n => n.EmergencyRequest)
                .HasForeignKey(n => n.EmergencyRequestId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.PatientTreatmentHistories)
                .WithOne(h => h.EmergencyRequest)
                .HasForeignKey(h => h.EmergencyRequestId)
                .OnDelete(DeleteBehavior.Restrict);

            // Closed By User
            builder.Property(x => x.ClosedByUserId)
                .HasMaxLength(450);

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(x => x.ClosedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Location Precision
            builder.Property(x => x.Latitude)
                .HasPrecision(9, 6);

            builder.Property(x => x.Longitude)
                .HasPrecision(9, 6);

            // Enum Conversions
            builder.Property(x => x.CurrentStatus)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(x => x.Priority)
                .HasConversion<string>()
                .HasMaxLength(50);

            // Performance Indexes
            builder.HasIndex(x => x.CurrentStatus);
            builder.HasIndex(x => x.CreatedAt);
        }
    }
}