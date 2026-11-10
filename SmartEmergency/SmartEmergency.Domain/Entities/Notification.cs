using System;
using System.Collections.Generic;
using System.Text;

namespace SmartEmergency.Domain.Entities
{
    public class Notification
    {
        public long NotificationId { get; set; }
        public string? RecipientUserId { get; set; }
        public int? EmergencyContactId { get; set; }
        public long? EmergencyRequestId { get; set; }
        public string Title { get; set; } = default!;
        public string Message { get; set; } = default!;
        public string Channel { get; set; } = default!;
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? SentAt { get; set; }

        public EmergencyRequest? EmergencyRequest { get; set; }
        public EmergencyContact? EmergencyContact { get; set; }
        
    }
}
