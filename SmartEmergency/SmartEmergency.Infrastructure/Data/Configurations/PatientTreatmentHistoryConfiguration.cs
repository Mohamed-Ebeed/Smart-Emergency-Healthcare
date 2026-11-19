using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartEmergency.Domain.Entities;

namespace SmartEmergency.Infrastructure.Data.Configurations
{
    public class PatientTreatmentHistoryConfiguration
        : IEntityTypeConfiguration<PatientTreatmentHistory>
    {
        public void Configure(EntityTypeBuilder<PatientTreatmentHistory> builder)
        {
            // Patient Relationship
            builder.HasOne(x => x.Patient)
                .WithMany(p => p.PatientTreatmentHistories)
                .HasForeignKey(x => x.PatientId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}