using System;
using System.Collections.Generic;
using System.Text;
using SmartEmergency.Domain.Enums;
namespace SmartEmergency.Domain.Entities
{
    public class HospitalBed
    {
        public int HospitalBedId { get; set; }
        public int DepartmentId { get; set; }
        public string BedNumber { get; set; } = default!;
        public BedStatus BedStatus { get; set; } = default!;

        //Navigation Properties
        public Department Department { get; set; } = default!;
    }
}
