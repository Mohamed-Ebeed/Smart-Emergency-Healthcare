using System;
using System.Collections.Generic;
using System.Text;

namespace SmartEmergency.Domain.Entities
{
    public class HospitalStaff
    {
        public int HospitalStaffId { get; set; }
        public string UserId { get; set; } = default!;
        public int HospitalId { get; set; }
        public string FullName { get; set; } = default!;
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; } = true;


        public Hospital Hospital { get; set; } = default!;
    }
}
