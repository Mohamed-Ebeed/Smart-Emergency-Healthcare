using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartEmergency.Domain.Entities;

namespace SmartEmergency.Infrastructure.Data.Configurations
{
    public class HospitalBedConfiguration
        : IEntityTypeConfiguration<HospitalBed>
    {
        public void Configure(EntityTypeBuilder<HospitalBed> builder)
        {
            // Bed Number
            builder.Property(x => x.BedNumber)
                .HasMaxLength(50);

            // Unique Bed Number per Department
            builder.HasIndex(x => new { x.DepartmentId, x.BedNumber })
                .IsUnique();

            // Bed Status Conversion
            builder.Property(x => x.BedStatus)
                .HasConversion<string>()
                .HasMaxLength(50);
        }
    }
}