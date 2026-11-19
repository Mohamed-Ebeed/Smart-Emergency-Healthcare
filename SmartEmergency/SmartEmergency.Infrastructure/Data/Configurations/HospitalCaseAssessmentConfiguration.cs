
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartEmergency.Domain.Entities;

namespace SmartEmergency.Infrastructure.Data.Configurations
{
    public class HospitalCaseAssessmentConfiguration
        : IEntityTypeConfiguration<HospitalCaseAssessment>
    {
        public void Configure(EntityTypeBuilder<HospitalCaseAssessment> builder)
        {
            // Hospital Relationship
            builder.HasOne(x => x.Hospital)
                .WithMany(h => h.HospitalCaseAssessments)
                .HasForeignKey(x => x.HospitalId)
                .OnDelete(DeleteBehavior.Restrict);

            // Assessed Priority Conversion
            builder.Property(x => x.AssessedPriority)
                .HasConversion<string>()
                .HasMaxLength(50);
        }
    }
}
