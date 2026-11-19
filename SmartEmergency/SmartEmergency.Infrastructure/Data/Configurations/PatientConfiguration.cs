
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SmartEmergency.Domain.Entities;
using SmartEmergency.Infrastructure.Identity;

namespace SmartEmergency.Infrastructure.Data.Configurations
{
    public class PatientConfiguration : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {

            // Emergency Contacts (One-to-Many)
            builder.HasMany(p => p.EmergencyContacts)
                .WithOne(ec => ec.Patient)
                .HasForeignKey(ec => ec.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            // Emergency Requests (One-to-Many)
            builder.HasMany(p => p.EmergencyRequests)
                .WithOne(er => er.Patient)
                .HasForeignKey(er => er.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            // ApplicationUser Relationship (One-to-One)
            builder.Property(p => p.UserId)
                .HasMaxLength(450);

            builder.HasIndex(p => p.UserId)
                .IsUnique();

            builder.HasOne<ApplicationUser>()
                .WithOne()
                .HasForeignKey<Patient>(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Gender Enum Conversion
            builder.Property(p => p.Gender)
                .HasConversion<string>()
                .HasMaxLength(20);
        }
    }
}
