
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartEmergency.Domain.Entities;
using SmartEmergency.Infrastructure.Identity;

namespace SmartEmergency.Infrastructure.Data.Configurations
{
    public class EmergencyHospitalAssignmentConfiguration
        : IEntityTypeConfiguration<EmergencyHospitalAssignment>
    {
        public void Configure(EntityTypeBuilder<EmergencyHospitalAssignment> builder)
        {
            // Hospital Relationship
            builder.HasOne(x => x.Hospital)
                .WithMany(h => h.EmergencyHospitalAssignments)
                .HasForeignKey(x => x.HospitalId)
                .OnDelete(DeleteBehavior.Restrict);

            // Assigned By Operator User ID
            builder.Property(x => x.AssignedByOperatorId)
                .HasMaxLength(450);

            // Application User Relationship
            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(x => x.AssignedByOperatorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Assignment Status Conversion
            builder.Property(x => x.AssignmentStatus)
                .HasConversion<string>()
                .HasMaxLength(50);
        }
    }
}
