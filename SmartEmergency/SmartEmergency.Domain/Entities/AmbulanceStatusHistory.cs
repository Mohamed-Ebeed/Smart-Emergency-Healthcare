using System;
using System.Collections.Generic;
using System.Text;
using SmartEmergency.Domain.Enums;
namespace SmartEmergency.Domain.Entities
{
    public class AmbulanceStatusHistory
    {
        public long AmbulanceStatusHistoryId { get; set; }
        public int AmbulanceId { get; set; }
        public AmbulanceStatus Status { get; set; } = default!;
        public DateTime ChangedAt { get; set; }
        public string? Notes { get; set; }

        public Ambulance Ambulance { get; set; } = default!;
        
    }
}
