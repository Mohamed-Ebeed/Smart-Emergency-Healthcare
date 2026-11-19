
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SmartEmergency.Domain.Entities;

namespace SmartEmergency.Infrastructure.Data.Configurations
{
    public class PatientChronicDiseaseConfiguration
        : IEntityTypeConfiguration<PatientChronicDisease>
    {
        public void Configure(
            EntityTypeBuilder<PatientChronicDisease> builder)
        {
            // Composite Primary Key
            builder.HasKey(x => new
            {
                x.PatientId,
                x.ChronicDiseaseId
            });

            // Patient Relationship
            builder.HasOne(x => x.Patient)
                .WithMany(p => p.PatientChronicDiseases)
                .HasForeignKey(x => x.PatientId);

            // ChronicDisease Relationship
            builder.HasOne(x => x.ChronicDisease)
                .WithMany(c => c.PatientChronicDiseases)
                .HasForeignKey(x => x.ChronicDiseaseId);
        }
    }
}
