using System.ComponentModel.DataAnnotations;
using SmartEmergency.Domain.Enums;

namespace SmartEmergency.Application.Hospitals.DTOs;

public class HospitalBedDto
{
    public int HospitalBedId { get; set; }
    public int DepartmentId { get; set; }
    public string BedNumber { get; set; } = string.Empty;
    public BedStatus BedStatus { get; set; }
}

public class CreateHospitalBedDto
{
    [Required]
    public int DepartmentId { get; set; }

    [Required]
    [StringLength(50)]
    public string BedNumber { get; set; } = string.Empty;
}

public class UpdateHospitalBedDto
{
    [Required]
    [StringLength(50)]
    public string BedNumber { get; set; } = string.Empty;
}