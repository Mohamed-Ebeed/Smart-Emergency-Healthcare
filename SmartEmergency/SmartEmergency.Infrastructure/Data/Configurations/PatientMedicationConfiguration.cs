
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SmartEmergency.Domain.Entities;

namespace SmartEmergency.Infrastructure.Data.Configurations
{
    public class PatientMedicationConfiguration
        : IEntityTypeConfiguration<PatientMedication>
    {
        public void Configure(
            EntityTypeBuilder<PatientMedication> builder)
        {
            // Composite Primary Key
            builder.HasKey(x => new
            {
                x.PatientId,
                x.MedicationId
            });

            // Patient Relationship
            builder.HasOne(x => x.Patient)
                .WithMany(p => p.PatientMedications)
                .HasForeignKey(x => x.PatientId);

            // Medication Relationship
            builder.HasOne(x => x.Medication)
                .WithMany(m => m.PatientMedications)
                .HasForeignKey(x => x.MedicationId);
        }
    }
}
