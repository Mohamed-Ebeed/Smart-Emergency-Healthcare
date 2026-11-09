using System;
using System.Collections.Generic;
using System.Text;

namespace SmartEmergency.Domain.Entities
{
    public class Ambulance
    {
        public int AmbulanceId { get; set; }
        public int HospitalId { get; set; }
        public string VehicleNumber { get; set; } = default!;
        public string AmbulanceStatus { get; set; } = default!;
        public Decimal? LastKnownLatitude { get; set; }
        public Decimal? LastKnownLongitude { get; set; }

        public Hospital Hospital { get; set; } = default!;
        public ICollection<AmbulanceAssignment> AmbulanceAssignments { get; set; } = new List<AmbulanceAssignment>();
        public ICollection<AmbulanceLocationHistory> AmbulanceLocationHistories { get; set; } = new List<AmbulanceLocationHistory>();
        public ICollection<AmbulanceStatusHistory> AmbulanceStatusHistories { get; set; } = new List<AmbulanceStatusHistory>();
    }
}
