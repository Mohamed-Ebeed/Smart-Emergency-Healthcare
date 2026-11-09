using System;
using System.Collections.Generic;
using System.Text;

namespace SmartEmergency.Domain.Entities
{
    public class HospitalBed
    {
        public int HospitalBedId { get; set; }
        public int DepartmentId { get; set; }
        public string BedNumber { get; set; } = default!;
        public string BedStatus { get; set; } = default!;

        //Navigation Properties
        public Department Department { get; set; } = default!;
    }
}
