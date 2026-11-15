using SmartEmergency.Application.Notifications.DTOs;
using SmartEmergency.Application.Notifications.Interfaces;
using SmartEmergency.Domain.Enums;

namespace SmartEmergency.Application.Notifications.Services
{
    public static class EmergencyNotificationHelper
    {
        public static async Task NotifyEmergencySubmittedAsync(
            INotificationService notificationService,
            long emergencyRequestId,
            string patientUserId,
            IEnumerable<long>? emergencyContactIds,
            CancellationToken cancellationToken = default)
        {
            var patientDto = new CreateNotificationDto
            {
                RecipientUserId = patientUserId,
                EmergencyRequestId = emergencyRequestId,
                Title = "Emergency Request Submitted",
                Message = $"Your emergency request #{emergencyRequestId} has been successfully submitted. Help is on the way.",
                Channel = NotificationChannel.InApp
            };
            await notificationService.CreateNotificationAsync(patientDto, cancellationToken);

            if (emergencyContactIds != null)
            {
                foreach (var contactId in emergencyContactIds)
                {
                    var contactDto = new CreateNotificationDto
                    {
                        EmergencyContactId = contactId,
                        EmergencyRequestId = emergencyRequestId,
                        Title = "Emergency Alert - Patient Update",
                        Message = $"An emergency request #{emergencyRequestId} has been triggered. You are listed as an emergency contact.",
                        Channel = NotificationChannel.InApp
                    };
                    await notificationService.CreateNotificationAsync(contactDto, cancellationToken);
                }
            }
        }

        public static async Task NotifyStatusChangedAsync(
            INotificationService notificationService,
            long emergencyRequestId,
            string patientUserId,
            string newStatus,
            CancellationToken cancellationToken = default)
        {
            var dto = new CreateNotificationDto
            {
                RecipientUserId = patientUserId,
                EmergencyRequestId = emergencyRequestId,
                Title = "Emergency Request Status Updated",
                Message = $"Your emergency request #{emergencyRequestId} status has changed to: {newStatus}.",
                Channel = NotificationChannel.InApp
            };

            await notificationService.CreateNotificationAsync(dto, cancellationToken);
        }
    }
}
