using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartEmergency.Domain.Entities;

namespace SmartEmergency.Infrastructure.Data.Configurations
{
    public class MedicalProfileConfiguration
        : IEntityTypeConfiguration<MedicalProfile>
    {
        public void Configure(EntityTypeBuilder<MedicalProfile> builder)
        {
            // Patient Relationship (One-to-One)
            builder.HasOne(m => m.Patient)
                .WithOne(p => p.MedicalProfile)
                .HasForeignKey<MedicalProfile>(m => m.PatientId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}