
using Microsoft.Extensions.DependencyInjection;

namespace SmartEmergency.Application.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(
            this IServiceCollection services)
        {
            // Application services will be registered here.

            return services;
        }
    }
}
