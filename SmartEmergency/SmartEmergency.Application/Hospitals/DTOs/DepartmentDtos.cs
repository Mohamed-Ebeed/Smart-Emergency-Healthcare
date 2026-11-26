using System.ComponentModel.DataAnnotations;

namespace SmartEmergency.Application.Hospitals.DTOs;

public class DepartmentDto
{
    public int DepartmentId { get; set; }
    public int HospitalId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? Capacity { get; set; }
    public bool IsActive { get; set; }
}

public class CreateDepartmentDto
{
    [Required]
    public int HospitalId { get; set; }

    [Required]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Range(0, 10000)]
    public int? Capacity { get; set; }
}

public class UpdateDepartmentDto
{
    [Required]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Range(0, 10000)]
    public int? Capacity { get; set; }
}