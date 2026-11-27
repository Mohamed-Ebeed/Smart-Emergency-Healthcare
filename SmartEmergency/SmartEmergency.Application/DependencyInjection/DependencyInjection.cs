using Microsoft.Extensions.DependencyInjection;

using SmartEmergency.Application.Notifications.Interfaces;
using SmartEmergency.Application.Notifications.Services;

using SmartEmergency.Application.EmergencyRequests.Interfaces;
using SmartEmergency.Application.EmergencyRequests.Services;

using SmartEmergency.Application.Patients.Interfaces;
using SmartEmergency.Application.Patients.Services;


using SmartEmergency.Application.Hospitals.Interfaces;
using SmartEmergency.Application.Hospitals.Services;

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
            services.AddScoped<IEmergencyCoordinationService, EmergencyCoordinationService>();
            services.AddScoped<INotificationService, NotificationService>();

            // Real time defaults (do nothing). The API overrides them with SignalR
            services.AddScoped<IRealtimeNotifier, NoRealtimeNotifier>();
            services.AddScoped<INotificationSender, NoNotificationSender>();

            // Hospitals :)
            services.AddScoped<IHospitalService, HospitalService>();
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<IHospitalBedService, HospitalBedService>();
            services.AddScoped<IAmbulanceService, AmbulanceService>();
            services.AddScoped<IStaffProfileService, StaffProfileService>();

            return services;
        }
    }
}