using System;
using System.Collections.Generic;
using System.Text;

namespace SmartEmergency.Domain.Entities
{
    public class AmbulanceLocationHistory
    {
        public long LocationHistoryId { get; set; }
        public int AmbulanceId { get; set; }
        public long? EmergencyRequestId { get; set; }
        public Decimal Latitude { get; set; }
        public Decimal Longitude { get; set; }
        public DateTime RecordedAt { get; set; }


        public Ambulance Ambulance { get; set; } = default!;
        public EmergencyRequest? EmergencyRequest { get; set; }
        
    }
}
