using System.Collections.Generic;

namespace SmartEmergency.Application.Notifications.DTOs
{
    public class NotificationListDto
    {
        public List<NotificationDto> Notifications { get; set; } = new();
        public int TotalCount { get; set; }
        public int UnreadCount { get; set; }
    }
}
