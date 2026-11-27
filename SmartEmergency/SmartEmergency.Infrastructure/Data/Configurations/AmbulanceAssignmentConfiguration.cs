
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

            // Prevent multiple active assignments for the same ambulance
            builder.HasIndex(x => x.AmbulanceId)
                .IsUnique()
                .HasFilter("[CompletedAt] IS NULL AND [AssignmentStatus] = 'Accepted'");

            // Prevent multiple active assignments for the same paramedic
            builder.HasIndex(x => x.ParamedicId)
                .IsUnique()
                .HasFilter("[CompletedAt] IS NULL AND [AssignmentStatus] = 'Accepted'");

            // Prevent multiple active ambulance assignments for the same emergency request
            builder.HasIndex(x => x.EmergencyRequestId)
                .IsUnique()
                .HasFilter("[CompletedAt] IS NULL AND [AssignmentStatus] = 'Accepted'");
        }
    }
}
