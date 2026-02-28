using System;
using System.ComponentModel.DataAnnotations;

namespace Munientry.Poc.Api.Data
{
    public class LeapSentencingDto
    {
        public string? DefendantFirstName { get; set; }
        public string? DefendantLastName { get; set; }
        public string? CaseNumber { get; set; }
        public string? DefenseCounselName { get; set; }
        public string? DefenseCounselType { get; set; }
        public bool DefenseCounselWaived { get; set; }
        public string? AppearanceReason { get; set; }
        public DateTime? LeapPleaDate { get; set; }
        public DateTime? PleaTrialDate { get; set; }
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
    }
}
