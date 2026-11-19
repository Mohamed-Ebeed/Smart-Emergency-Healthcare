using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartEmergency.Domain.Entities;

namespace SmartEmergency.Infrastructure.Data.Configurations
{
    public class EmergencyTypeConfiguration
        : IEntityTypeConfiguration<EmergencyType>
    {
        public void Configure(EntityTypeBuilder<EmergencyType> builder)
        {
            builder.Property(x => x.Name)
                .HasMaxLength(100);

            builder.HasIndex(x => x.Name)
                .IsUnique();

            builder.Property(x => x.DefaultPriority)
                .HasConversion<string>()
                .HasMaxLength(50);
        }
    }
}