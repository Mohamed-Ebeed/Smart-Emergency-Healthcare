namespace SmartEmergency.Application.EmergencyRequests.DTOs;

// Suggested hospital to try after a rejection
public class NextHospitalDto
{
    public int HospitalId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int AvailableBeds { get; set; }

    // Null when the request or the hospital has no location
    public double? DistanceKm { get; set; }
}