using System;
using System.Collections.Generic;
using System.Text;
using SmartEmergency.Domain.Enums;
namespace SmartEmergency.Domain.Entities
{
    public class EmergencyReviewDecision
    {
        public long EmergencyReviewDecisionId { get; set; }
        public long EmergencyRequestId { get; set; }
        public string OperatorId { get; set; } = default!;
        public int? ConfirmedEmergencyTypeId { get; set; }
        public Priority? ConfirmedPriority { get; set; }
        public string Decision { get; set; } = default!;
        public string? Notes { get; set; }
        public DateTime ReviewedAt { get; set; }

        //Navigation Properties
        public EmergencyRequest EmergencyRequest { get; set; } = default!;

        public EmergencyType? ConfirmedEmergencyType { get; set; }

    }
}
