
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartEmergency.Domain.Entities;
using SmartEmergency.Infrastructure.Identity;

namespace SmartEmergency.Infrastructure.Data.Configurations
{
    public class AmbulanceAssignmentConfiguration
        : IEntityTypeConfiguration<AmbulanceAssignment>
    {
        public void Configure(EntityTypeBuilder<AmbulanceAssignment> builder)
        {
            // Ambulance Relationship
            builder.HasOne(x => x.Ambulance)
                .WithMany(a => a.AmbulanceAssignments)
                .HasForeignKey(x => x.AmbulanceId)
                .OnDelete(DeleteBehavior.Restrict);

            // Assigned By User ID
            builder.Property(x => x.AssignedByUserId)
                .HasMaxLength(450);

            // Application User Relationship
            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(x => x.AssignedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Assignment Status Conversion
            builder.Property(x => x.AssignmentStatus)
                .HasConversion<string>()
                .HasMaxLength(50);
        }
    }
}
