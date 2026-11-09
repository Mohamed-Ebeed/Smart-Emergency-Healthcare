using System;
using System.Collections.Generic;
using System.Text;

namespace SmartEmergency.Domain.Entities
{
    public class EmergencyType
    {
        public int EmergencyTypeId { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public string? DefaultPriority { get; set; }

        public ICollection<EmergencyRequest> EmergencyRequests { get; set; }
        = new List<EmergencyRequest>();
        public ICollection<AITriageResult> AITriageResults { get; set; }
        = new List<AITriageResult>();
        public ICollection<EmergencyReviewDecision> EmergencyReviewDecisions { get; set; }
        = new List<EmergencyReviewDecision>();
    }
}
