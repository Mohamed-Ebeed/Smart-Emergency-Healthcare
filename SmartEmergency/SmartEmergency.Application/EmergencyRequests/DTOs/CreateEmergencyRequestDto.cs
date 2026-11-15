using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SmartEmergency.Application.EmergencyRequests.DTOs
{
    public class CreateEmergencyRequestDto
    {
        [Range(1, int.MaxValue)]
        public int PatientId { get; set; }

        [Range(1, int.MaxValue)]
        public int EmergencyTypeId { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        [Range(typeof(decimal), "-90", "90")]
        public decimal? Latitude { get; set; }

        [Range(typeof(decimal), "-180", "180")]
        public decimal? Longitude { get; set; }

        [StringLength(500)]
        public string? AddressText { get; set; }
    }
}