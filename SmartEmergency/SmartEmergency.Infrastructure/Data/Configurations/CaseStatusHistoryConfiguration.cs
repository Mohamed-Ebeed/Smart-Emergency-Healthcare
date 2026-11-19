using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartEmergency.Domain.Entities;
using SmartEmergency.Infrastructure.Identity;

namespace SmartEmergency.Infrastructure.Data.Configurations
{
    public class CaseStatusHistoryConfiguration
        : IEntityTypeConfiguration<CaseStatusHistory>
    {
        public void Configure(EntityTypeBuilder<CaseStatusHistory> builder)
        {
            // Changed By User ID
            builder.Property(x => x.ChangedByUserId)
                .HasMaxLength(450);

            // Application User Relationship
            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(x => x.ChangedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Status Conversion
            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(50);
        }
    }
}