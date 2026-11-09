using System;
using System.Collections.Generic;
using System.Text;

namespace SmartEmergency.Domain.Entities
{
    public class DoctorNote
    {
        public long DoctorNoteId { get; set; }
        public long EmergencyRequestId { get; set; }
        public int DoctorId { get; set; }
        public string NoteText { get; set; } = default!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        public EmergencyRequest EmergencyRequest { get; set; } = default!;
        public Doctor Doctor { get; set; } = default!;

    }
}
