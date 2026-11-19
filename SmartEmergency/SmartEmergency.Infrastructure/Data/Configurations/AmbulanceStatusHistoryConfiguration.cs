
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartEmergency.Domain.Entities;

namespace SmartEmergency.Infrastructure.Data.Configurations
{
    public class AmbulanceStatusHistoryConfiguration
        : IEntityTypeConfiguration<AmbulanceStatusHistory>
    {
        public void Configure(EntityTypeBuilder<AmbulanceStatusHistory> builder)
        {
            // Ambulance Relationship
            builder.HasOne(x => x.Ambulance)
                .WithMany(a => a.AmbulanceStatusHistories)
                .HasForeignKey(x => x.AmbulanceId)
                .OnDelete(DeleteBehavior.Restrict);

            // Status Conversion
            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(50);
        }
    }
}
