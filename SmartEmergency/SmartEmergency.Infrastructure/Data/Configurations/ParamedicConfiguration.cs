using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartEmergency.Domain.Entities;
using SmartEmergency.Infrastructure.Identity;

namespace SmartEmergency.Infrastructure.Data.Configurations
{
    public class ParamedicConfiguration
        : IEntityTypeConfiguration<Paramedic>
    {
        public void Configure(EntityTypeBuilder<Paramedic> builder)
        {
            // ApplicationUser Relationship
            builder.Property(x => x.UserId)
                .HasMaxLength(450);

            builder.HasIndex(x => x.UserId)
                .IsUnique();

            builder.HasOne<ApplicationUser>()
                .WithOne()
                .HasForeignKey<Paramedic>(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Hospital Relationship
            builder.HasOne(x => x.Hospital)
                .WithMany(h => h.Paramedics)
                .HasForeignKey(x => x.HospitalId)
                .OnDelete(DeleteBehavior.Restrict);

            // AmbulanceAssignments
            builder.HasMany(x => x.AmbulanceAssignments)
                .WithOne(a => a.Paramedic)
                .HasForeignKey(a => a.ParamedicId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
