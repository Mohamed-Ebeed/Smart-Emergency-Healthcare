using System;
using System.Collections.Generic;
using System.Text;
using SmartEmergency.Domain.Enums;

namespace SmartEmergency.Application.EmergencyRequests.DTOs
{
    public class EmergencyRequestDto
    {
        public long EmergencyRequestId { get; set; }
        public int PatientId { get; set; }
        public int? EmergencyTypeId { get; set; }
        public string? EmergencyTypeName { get; set; }
        public string? Description { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string? AddressText { get; set; }
        public Priority? Priority { get; set; }
        public RequestStatus CurrentStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? PickedUpAt { get; set; }
        public DateTime? ArrivedAtHospitalAt { get; set; }
        public DateTime? ClosedAt { get; set; }
    }
}