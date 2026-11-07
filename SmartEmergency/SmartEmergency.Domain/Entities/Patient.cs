using System;
using System.Collections.Generic;
using System.Text;

namespace SmartEmergency.Domain.Entities
{
    public class Patient
    {
        public int PatientId { get; set; }
        public string UserId { get; set; } = default!;
        public DateOnly DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
