using System;
using System.Collections.Generic;
using System.Text;

namespace SmartEmergency.Domain.Entities
{
    public class Department
    {
        public int DepartmentId { get; set; }
        public int HospitalId { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public int? Capacity { get; set; }
        public bool IsActive { get; set; } = true;


        //Navigation Properties
        public Hospital Hospital { get; set; } = default!;
        public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
        public ICollection<HospitalBed> HospitalBeds { get; set; } = new List<HospitalBed>();
        public ICollection<CaseDepartmentAssignment> CaseDepartmentAssignments { get; set; } = new List<CaseDepartmentAssignment>();
        public ICollection<AITriageResult> AITriageResults { get; set; }
        = new List<AITriageResult>();

    }
}
