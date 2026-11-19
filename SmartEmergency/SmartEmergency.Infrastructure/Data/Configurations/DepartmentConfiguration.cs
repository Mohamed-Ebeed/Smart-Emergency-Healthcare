using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartEmergency.Domain.Entities;

namespace SmartEmergency.Infrastructure.Data.Configurations
{
    public class DepartmentConfiguration
        : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            // Hospital Relationship
            builder.HasOne(x => x.Hospital)
                .WithMany(h => h.Departments)
                .HasForeignKey(x => x.HospitalId)
                .OnDelete(DeleteBehavior.Restrict);

            // Doctor Relationship
            builder.HasMany(d => d.Doctors)
                .WithOne(x => x.Department)
                .HasForeignKey(x => x.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // HospitalBed Relationship
            builder.HasMany(d => d.HospitalBeds)
                .WithOne(x => x.Department)
                .HasForeignKey(x => x.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // CaseDepartmentAssignment Relationship
            builder.HasMany(d => d.CaseDepartmentAssignments)
                .WithOne(x => x.Department)
                .HasForeignKey(x => x.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Name
            builder.Property(x => x.Name)
                .HasMaxLength(150);

            builder.HasIndex(x => new { x.HospitalId, x.Name })
                .IsUnique();
        }
    }
}