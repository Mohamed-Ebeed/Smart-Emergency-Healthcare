using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartEmergency.Infrastructure.Identity;
using AppRoles = SmartEmergency.Domain.Common.Roles;

namespace SmartEmergency.Infrastructure.Data.Configurations
{
    public class IdentityRoleConfiguration
        : IEntityTypeConfiguration<IdentityRole>
    {
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            builder.HasData(
                new IdentityRole
                {
                    Id = "b1f0c9a0-0001-4a00-9000-000000000001",
                    Name = AppRoles.Patient,
                    NormalizedName = AppRoles.Patient.ToUpper(),
                    ConcurrencyStamp = "role-patient-stamp"
                },
                new IdentityRole
                {
                    Id = "b1f0c9a0-0002-4a00-9000-000000000002",
                    Name = AppRoles.Operator,
                    NormalizedName = AppRoles.Operator.ToUpper(),
                    ConcurrencyStamp = "role-operator-stamp"
                },
                new IdentityRole
                {
                    Id = "b1f0c9a0-0003-4a00-9000-000000000003",
                    Name = AppRoles.HospitalStaff,
                    NormalizedName = AppRoles.HospitalStaff.ToUpper(),
                    ConcurrencyStamp = "role-hospital-staff-stamp"
                },
                new IdentityRole
                {
                    Id = "b1f0c9a0-0004-4a00-9000-000000000004",
                    Name = AppRoles.Paramedic,
                    NormalizedName = AppRoles.Paramedic.ToUpper(),
                    ConcurrencyStamp = "role-paramedic-stamp"
                },
                new IdentityRole
                {
                    Id = "b1f0c9a0-0005-4a00-9000-000000000005",
                    Name = AppRoles.Doctor,
                    NormalizedName = AppRoles.Doctor.ToUpper(),
                    ConcurrencyStamp = "role-doctor-stamp"
                },
                new IdentityRole
                {
                    Id = "b1f0c9a0-0006-4a00-9000-000000000006",
                    Name = AppRoles.Admin,
                    NormalizedName = AppRoles.Admin.ToUpper(),
                    ConcurrencyStamp = "role-admin-stamp"
                }
            );
        }
    }
}