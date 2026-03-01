using System;
using System.ComponentModel.DataAnnotations;

namespace Munientry.Api.Data
{
    public class LeapValidSentencingDto
    {
        [Required]
        public string? DefendantFirstName { get; set; }
        [Required]
        public string? DefendantLastName { get; set; }
        [Required]
        public string? CaseNumber { get; set; }
        [Required]
        public DateTime? Date { get; set; }
        public string? DefenseCounselName { get; set; }
        public string? DefenseCounselType { get; set; }
        public bool DefenseCounselWaived { get; set; }
        public string? AppearanceReason { get; set; }
        public string? Charges { get; set; }
        public string? ChargeStatute { get; set; }
        public string? ChargeDegree { get; set; }
        public string? ChargePlea { get; set; }
    }
}
