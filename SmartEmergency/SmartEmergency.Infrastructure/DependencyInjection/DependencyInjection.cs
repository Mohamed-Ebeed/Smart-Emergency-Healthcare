
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using SmartEmergency.Application.Common.Interfaces;
using SmartEmergency.Infrastructure.Data;
using SmartEmergency.Infrastructure.Identity;

namespace SmartEmergency.Infrastructure.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Database
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")
                )
            );

            // IAppDbContext
            services.AddScoped<IAppDbContext>(provider =>
                provider.GetRequiredService<ApplicationDbContext>()
            );

            // ASP.NET Core Identity
            services
                .AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            return services;
        }
    }
}
