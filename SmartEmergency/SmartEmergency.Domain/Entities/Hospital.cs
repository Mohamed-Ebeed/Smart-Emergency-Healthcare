using System;
using System.Collections.Generic;
using System.Text;

namespace SmartEmergency.Domain.Entities
{
    public class Hospital
    {
        public int HospitalId { get; set; }
        public string Name { get; set; } = default!;
        public string Address { get; set; } = default!;
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<Department> Departments { get; set; } = new List<Department>();
        public ICollection<EmergencyHospitalAssignment> EmergencyHospitalAssignments { get; set; } = new List<EmergencyHospitalAssignment>();
        public ICollection<HospitalCaseAssessment> HospitalCaseAssessments { get; set; } = new List<HospitalCaseAssessment>();
        public ICollection<Ambulance> Ambulances { get; set; } = new List<Ambulance>();
        public ICollection<Paramedic> Paramedics { get; set; } = new List<Paramedic>();
        public ICollection<HospitalStaff> HospitalStaffs { get; set; } = new List<HospitalStaff>();
    }
}
