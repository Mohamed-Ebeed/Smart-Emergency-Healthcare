using System;
using System.Collections.Generic;
using System.Text;

namespace SmartEmergency.Domain.Entities
{
    public class CaseStatusHistory
    {
        public long CaseStatusHistoryId { get; set; }
        public long EmergencyRequestId { get; set; }
        public string Status { get; set; } = default!;
        public string? ChangedByUserId { get; set; }
        public string? Notes { get; set; }
        public DateTime ChangedAt { get; set; }
        public EmergencyRequest EmergencyRequest { get; set; } = default!; //Navigation property

    }
}
