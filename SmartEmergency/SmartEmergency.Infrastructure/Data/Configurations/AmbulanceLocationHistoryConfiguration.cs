
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartEmergency.Domain.Entities;

namespace SmartEmergency.Infrastructure.Data.Configurations
{
    public class AmbulanceLocationHistoryConfiguration
        : IEntityTypeConfiguration<AmbulanceLocationHistory>
    {
        public void Configure(EntityTypeBuilder<AmbulanceLocationHistory> builder)
        {
            // Ambulance Relationship
            builder.HasOne(x => x.Ambulance)
                .WithMany(a => a.AmbulanceLocationHistories)
                .HasForeignKey(x => x.AmbulanceId)
                .OnDelete(DeleteBehavior.Restrict);

            // Location Precision
            builder.Property(x => x.Latitude)
                .HasPrecision(9, 6);

            builder.Property(x => x.Longitude)
                .HasPrecision(9, 6);

            // Performance Index
            builder.HasIndex(x => new { x.AmbulanceId, x.RecordedAt });
        }
    }
}
