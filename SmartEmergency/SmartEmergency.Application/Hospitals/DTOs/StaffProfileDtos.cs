using System.ComponentModel.DataAnnotations;

namespace SmartEmergency.Application.Hospitals.DTOs;

public class DoctorProfileDto
{
    public int DoctorId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Specialization { get; set; }
    public string? PhoneNumber { get; set; }
    public bool IsActive { get; set; }
}

public class UpdateDoctorProfileDto
{
    [Required]
    public int DepartmentId { get; set; }

    [Required]
    [StringLength(150)]
    public string FullName { get; set; } = string.Empty;

    [StringLength(150)]
    public string? Specialization { get; set; }

    [Phone]
    public string? PhoneNumber { get; set; }
}

public class ParamedicProfileDto
{
    public int ParamedicId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int HospitalId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public bool IsAvailable { get; set; }
    public bool IsActive { get; set; }
}

public class UpdateParamedicProfileDto
{
    [Required]
    [StringLength(150)]
    public string FullName { get; set; } = string.Empty;

    [Phone]
    public string? PhoneNumber { get; set; }

    public bool IsAvailable { get; set; } = true;
}

public class HospitalStaffProfileDto
{
    public int HospitalStaffId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int HospitalId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public bool IsActive { get; set; }
}

public class UpdateHospitalStaffProfileDto
{
    [Required]
    [StringLength(150)]
    public string FullName { get; set; } = string.Empty;

    [Phone]
    public string? PhoneNumber { get; set; }
}