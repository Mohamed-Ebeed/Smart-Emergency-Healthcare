
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SmartEmergency.Domain.Entities;

namespace SmartEmergency.Infrastructure.Data.Configurations
{
    public class PatientAllergyConfiguration
        : IEntityTypeConfiguration<PatientAllergy>
    {
        public void Configure(
            EntityTypeBuilder<PatientAllergy> builder)
        {
            // Composite Primary Key
            builder.HasKey(x => new
            {
                x.PatientId,
                x.AllergyId
            });

            // Patient Relationship
            builder.HasOne(x => x.Patient)
                .WithMany(p => p.PatientAllergies)
                .HasForeignKey(x => x.PatientId);

            // Allergy Relationship
            builder.HasOne(x => x.Allergy)
                .WithMany(a => a.PatientAllergies)
                .HasForeignKey(x => x.AllergyId);
        }
    }
}
