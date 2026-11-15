
using Microsoft.Extensions.DependencyInjection;
using SmartEmergency.Application.Patients.Interfaces;
using SmartEmergency.Application.Patients.Services;

namespace SmartEmergency.Application.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(
            this IServiceCollection services)
        {
            // Application services will be registered here.
            services.AddScoped<IPatientService, PatientService>();
            services.AddScoped<IMedicalProfileService, MedicalProfileService>();
            return services;
        }
    }
}
