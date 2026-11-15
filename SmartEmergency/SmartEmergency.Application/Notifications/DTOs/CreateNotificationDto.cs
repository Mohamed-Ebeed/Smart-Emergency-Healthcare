using SmartEmergency.Domain.Enums;

namespace SmartEmergency.Application.Notifications.DTOs
{
    public class CreateNotificationDto
    {
        public string? RecipientUserId { get; set; }
        public long? EmergencyContactId { get; set; }
        public long EmergencyRequestId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public NotificationChannel Channel { get; set; } = NotificationChannel.InApp;
    }
}
