using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartEmergency.Domain.Entities;
using SmartEmergency.Application.Common.Seed;

namespace SmartEmergency.Infrastructure.Data.Configurations
{
    public class ChronicDiseaseConfiguration
        : IEntityTypeConfiguration<ChronicDisease>
    {
        public void Configure(EntityTypeBuilder<ChronicDisease> builder)
        {
            builder.Property(x => x.Name)
                .HasMaxLength(150);

            builder.HasIndex(x => x.Name)
                .IsUnique();
            builder.HasData(ReferenceDataSeed.ChronicDiseases);
        }
    }
}