using SmartEmergency.Domain.Enums;
using System;


namespace SmartEmergency.Application.Notifications.DTOs
{
    public class NotificationDto
    {
        public long NotificationId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public NotificationChannel Channel { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? SentAt { get; set; }
        public long? EmergencyRequestId { get; set; }
    }
}
