using System;
using System.ComponentModel.DataAnnotations;

namespace Munientry.Shared.Dtos
{
    public class DrivingPrivilegesDto
    {
        [Required]
        public string? DefendantFirstName { get; set; }
        [Required]
        public string? DefendantLastName { get; set; }
        [Required]
        public string? CaseNumber { get; set; }
        public DateTime? PleaTrialDate { get; set; }
        public string? AppearanceReason { get; set; }
        public string? DefenseCounselName { get; set; }
        public string? DefenseCounselType { get; set; }
        public bool DefenseCounselWaived { get; set; }
        public string? Offense { get; set; }
        public string? Statute { get; set; }
        public string? Degree { get; set; }
        public string? Plea { get; set; }
        public string? Finding { get; set; }
        public decimal? Fines { get; set; }
        public decimal? FinesSuspended { get; set; }
        public string? CourtCosts { get; set; }
        public string? AbilityToPay { get; set; }
        public DateTime? BalanceDueDate { get; set; }
        public decimal? PayToday { get; set; }
        public decimal? MonthlyPay { get; set; }
        public bool CreditForJail { get; set; }
        public int? JailTimeCredit { get; set; }
        public bool LicenseSuspension { get; set; }
        public bool CommunityService { get; set; }
        public bool OtherConditions { get; set; }
        public string? JudicialOfficerFirstName { get; set; }
        public string? JudicialOfficerLastName { get; set; }
        public string? JudicialOfficerType { get; set; }
    }
}
