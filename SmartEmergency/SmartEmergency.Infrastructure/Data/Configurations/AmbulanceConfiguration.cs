using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartEmergency.Domain.Entities;

namespace SmartEmergency.Infrastructure.Data.Configurations
{
    public class AmbulanceConfiguration
        : IEntityTypeConfiguration<Ambulance>
    {
        public void Configure(EntityTypeBuilder<Ambulance> builder)
        {
            // Hospital Relationship
            builder.HasOne(x => x.Hospital)
                .WithMany(h => h.Ambulances)
                .HasForeignKey(x => x.HospitalId)
                .OnDelete(DeleteBehavior.Restrict);

            // Location Precision
            builder.Property(x => x.LastKnownLatitude)
                .HasPrecision(9, 6);

            builder.Property(x => x.LastKnownLongitude)
                .HasPrecision(9, 6);

            // Ambulance Status Conversion
            builder.Property(x => x.AmbulanceStatus)
                .HasConversion<string>()
                .HasMaxLength(50);

            // Vehicle Number
            builder.Property(x => x.VehicleNumber)
                .HasMaxLength(50);

            builder.HasIndex(x => x.VehicleNumber)
                .IsUnique();
        }
    }
}