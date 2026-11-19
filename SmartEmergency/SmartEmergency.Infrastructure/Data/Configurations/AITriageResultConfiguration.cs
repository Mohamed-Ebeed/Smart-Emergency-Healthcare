using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartEmergency.Domain.Entities;

namespace SmartEmergency.Infrastructure.Data.Configurations
{
    public class AITriageResultConfiguration
        : IEntityTypeConfiguration<AITriageResult>
    {
        public void Configure(EntityTypeBuilder<AITriageResult> builder)
        {
            // Suggested Emergency Type
            builder.HasOne(x => x.SuggestedEmergencyType)
                .WithMany(t => t.AITriageResults)
                .HasForeignKey(x => x.SuggestedEmergencyTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Suggested Department
            builder.HasOne(x => x.SuggestedDepartment)
                .WithMany(d => d.AITriageResults)
                .HasForeignKey(x => x.SuggestedDepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Confidence Score Precision
            builder.Property(x => x.ConfidenceScore)
                .HasPrecision(5, 4);

            // Suggested Priority Conversion
            builder.Property(x => x.SuggestedPriority)
                .HasConversion<string>()
                .HasMaxLength(50);
        }
    }
}