using System;
using System.Collections.Generic;
using System.Text;
using SmartEmergency.Domain.Enums;
namespace SmartEmergency.Domain.Entities
{
    public class CaseStatusHistory
    {
        public long CaseStatusHistoryId { get; set; }
        public long EmergencyRequestId { get; set; }
        public RequestStatus Status { get; set; } 
        public string? ChangedByUserId { get; set; }
        public string? Notes { get; set; }
        public DateTime ChangedAt { get; set; }
        public EmergencyRequest EmergencyRequest { get; set; } = default!; //Navigation property

    }
}
