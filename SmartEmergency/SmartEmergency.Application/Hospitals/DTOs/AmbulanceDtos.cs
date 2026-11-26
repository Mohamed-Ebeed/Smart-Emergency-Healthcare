using System.ComponentModel.DataAnnotations;
using SmartEmergency.Domain.Enums;

namespace SmartEmergency.Application.Hospitals.DTOs;

public class AmbulanceDto
{
    public int AmbulanceId { get; set; }
    public int HospitalId { get; set; }
    public string VehicleNumber { get; set; } = string.Empty;
    public AmbulanceStatus AmbulanceStatus { get; set; }
    public decimal? LastKnownLatitude { get; set; }
    public decimal? LastKnownLongitude { get; set; }
}

public class CreateAmbulanceDto
{
    [Required]
    public int HospitalId { get; set; }

    [Required]
    [StringLength(50)]
    public string VehicleNumber { get; set; } = string.Empty;
}

public class UpdateAmbulanceDto
{
    [Required]
    [StringLength(50)]
    public string VehicleNumber { get; set; } = string.Empty;
}

public class UpdateAmbulanceLocationDto
{
    [Required]
    [Range(-90, 90)]
    public decimal Latitude { get; set; }

    [Required]
    [Range(-180, 180)]
    public decimal Longitude { get; set; }
}