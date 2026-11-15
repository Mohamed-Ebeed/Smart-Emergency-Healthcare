using SmartEmergency.Application.Notifications.Interfaces;
using SmartEmergency.Application.Notifications.Services;using Microsoft.Extensions.DependencyInjection;
using SmartEmergency.Application.EmergencyRequests.Interfaces;
using SmartEmergency.Application.EmergencyRequests.Services;
using SmartEmergency.Application.Patients.Interfaces;
using SmartEmergency.Application.Patients.Services;

namespace SmartEmergency.Application.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(
            this IServiceCollection services)
        {
            services.AddScoped<IPatientService, PatientService>();
            services.AddScoped<IMedicalProfileService, MedicalProfileService>();
            services.AddScoped<IEmergencyRequestService, EmergencyRequestService>();

            return services;
        }
    }
}
