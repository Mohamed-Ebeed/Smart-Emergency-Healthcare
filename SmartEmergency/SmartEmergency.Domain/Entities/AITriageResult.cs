using System;
using System.Collections.Generic;
using System.Text;

namespace SmartEmergency.Domain.Entities
{
    public class AITriageResult
    {
        public long AITriageResultId { get; set; }
        public long EmergencyRequestId { get; set; }
        public int? SuggestedEmergencyTypeId { get; set; }
        public string? SuggestedPriority { get; set; }
        public int? SuggestedDepartmentId { get; set; }
        public decimal? ConfidenceScore { get; set; }
        public string? ReasoningSummary { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        //Navigation Properties
        public EmergencyRequest EmergencyRequest { get; set; } = default!;

        public EmergencyType? SuggestedEmergencyType { get; set; }

        
    }
}
