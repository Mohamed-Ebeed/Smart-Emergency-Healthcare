using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartEmergency.Domain.Entities;
using SmartEmergency.Infrastructure.Identity;

namespace SmartEmergency.Infrastructure.Data.Configurations
{
    public class HospitalStaffConfiguration
        : IEntityTypeConfiguration<HospitalStaff>
    {
        public void Configure(EntityTypeBuilder<HospitalStaff> builder)
        {
            // ApplicationUser Relationship
            builder.Property(x => x.UserId)
                .HasMaxLength(450);

            builder.HasIndex(x => x.UserId)
                .IsUnique();

            builder.HasOne<ApplicationUser>()
                .WithOne()
                .HasForeignKey<HospitalStaff>(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Hospital Relationship
            builder.HasOne(x => x.Hospital)
                .WithMany(h => h.HospitalStaffs)
                .HasForeignKey(x => x.HospitalId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}