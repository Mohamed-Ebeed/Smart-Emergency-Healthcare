using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartEmergency.Domain.Entities;
using SmartEmergency.Infrastructure.Identity;

namespace SmartEmergency.Infrastructure.Data.Configurations
{
    public class DoctorConfiguration
        : IEntityTypeConfiguration<Doctor>
    {
        public void Configure(EntityTypeBuilder<Doctor> builder)
        {
            // ApplicationUser Relationship
            builder.Property(x => x.UserId)
                .HasMaxLength(450);

            builder.HasIndex(x => x.UserId)
                .IsUnique();

            builder.HasOne<ApplicationUser>()
                .WithOne()
                .HasForeignKey<Doctor>(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);


            // CaseDoctorAssignments
            builder.HasMany(x => x.CaseDoctorAssignments)
                .WithOne(a => a.Doctor)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            // HospitalCaseAssessments
            builder.HasMany(x => x.HospitalCaseAssessments)
                .WithOne(a => a.AssessedByDoctor)
                .HasForeignKey(a => a.AssessedByDoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            // DoctorNotes
            builder.HasMany(x => x.DoctorNotes)
                .WithOne(n => n.Doctor)
                .HasForeignKey(n => n.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            // PatientTreatmentHistories
            builder.HasMany(x => x.PatientTreatmentHistories)
                .WithOne(h => h.Doctor)
                .HasForeignKey(h => h.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}