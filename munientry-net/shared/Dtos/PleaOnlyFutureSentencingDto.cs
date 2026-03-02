using System;
using System.ComponentModel.DataAnnotations;

namespace Munientry.Shared.Dtos
{
    public class PleaOnlyFutureSentencingDto
    {
        [Required]
        public string DefendantFirstName { get; set; } = string.Empty;
        [Required]
        public string DefendantLastName { get; set; } = string.Empty;
        [Required]
        public string CaseNumber { get; set; } = string.Empty;
        public string? DefenseCounselName { get; set; }
        public string? DefenseCounselType { get; set; }
        public bool DefenseCounselWaived { get; set; }
        public string? AppearanceReason { get; set; }
        public DateTime? PleaDate { get; set; }
        public DateTime? SentencingDate { get; set; }
        public string? Charges { get; set; }
        public string? CourtCosts { get; set; }
        public string? AbilityToPay { get; set; }
        public DateTime? BalanceDueDate { get; set; }
        public string? PayToday { get; set; }
        public string? MonthlyPay { get; set; }
        public bool CreditForJail { get; set; }
        public string? JailTimeCreditDays { get; set; }
        public string? FraInFile { get; set; }
        public string? FraInCourt { get; set; }
        public bool LicenseSuspension { get; set; }
        public bool CommunityService { get; set; }
        public bool OtherConditions { get; set; }
        public string? JudicialOfficerFirstName { get; set; }
        public string? JudicialOfficerLastName { get; set; }
        public string? JudicialOfficerType { get; set; }
    }
}