using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartEmergency.Domain.Entities;

namespace SmartEmergency.Infrastructure.Data.Configurations
{
    public class AllergyConfiguration : IEntityTypeConfiguration<Allergy>
    {
        public void Configure(EntityTypeBuilder<Allergy> builder)
        {
            builder.Property(x => x.Name)
                .HasMaxLength(150);

            builder.HasIndex(x => x.Name)
                .IsUnique();
        }
    }
}