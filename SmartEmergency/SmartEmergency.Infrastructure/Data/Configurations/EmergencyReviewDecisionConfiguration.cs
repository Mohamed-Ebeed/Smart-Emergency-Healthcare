
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartEmergency.Domain.Entities;
using SmartEmergency.Infrastructure.Identity;

namespace SmartEmergency.Infrastructure.Data.Configurations
{
    public class EmergencyReviewDecisionConfiguration
        : IEntityTypeConfiguration<EmergencyReviewDecision>
    {
        public void Configure(EntityTypeBuilder<EmergencyReviewDecision> builder)
        {
            // Confirmed Emergency Type Relationship
            builder.HasOne(x => x.ConfirmedEmergencyType)
                .WithMany(e => e.EmergencyReviewDecisions)
                .HasForeignKey(x => x.ConfirmedEmergencyTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Operator User ID
            builder.Property(x => x.OperatorId)
                .HasMaxLength(450);

            // Application User Relationship
            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(x => x.OperatorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Confirmed Priority Conversion
            builder.Property(x => x.ConfirmedPriority)
                .HasConversion<string>()
                .HasMaxLength(50);
        }
    }
}
