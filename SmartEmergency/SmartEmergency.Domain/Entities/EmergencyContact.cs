using System;
using System.Collections.Generic;
using System.Text;

namespace SmartEmergency.Domain.Entities
{
    public class EmergencyContact
    {
        public int EmergencyContactId { get; set; }
        public int PatientId { get; set; }
        public string FullName { get; set; } = default!;
        public string? Relationship { get; set; }
        public string PhoneNumber { get; set; } = default!;
        public bool IsPrimary { get; set; }

        public Patient Patient { get; set; } = default!; //Navigation property

    }
}
