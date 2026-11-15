
using Microsoft.Extensions.DependencyInjection;
using SmartEmergency.Application.EmergencyRequests.Interfaces;
using SmartEmergency.Application.EmergencyRequests.Services;

namespace SmartEmergency.Application.DependencyInjection

{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(
            this IServiceCollection services)
        {
            services.AddScoped<IEmergencyRequestService, EmergencyRequestService>();

            return services;
        }
    }
}
