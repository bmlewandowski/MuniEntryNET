using System;
using System.ComponentModel.DataAnnotations;
using Munientry.Shared.Validation;

namespace Munientry.Shared.Dtos
{
    public class NotGuiltyPleaDto
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
        [Required]
        [PastDate]
        public DateTime? PleaDate { get; set; }
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
        public string? BondType { get; set; }
        public string? BondAmount { get; set; }
        public bool NoContact { get; set; }
        public bool NoAlcoholDrugs { get; set; }
        public bool AlcoholDrugsAssessment { get; set; }
        public bool MentalHealthAssessment { get; set; }
        public bool FingerprintInCourt { get; set; }
        public bool SpecializedDocket { get; set; }
        public string? SpecializedDocketType { get; set; }
        public bool Monitoring { get; set; }
        public string? MonitoringType { get; set; }
        public bool ComplyProtectionOrder { get; set; }
        public bool PublicSafetySuspension { get; set; }
        public string? JudicialOfficerFirstName { get; set; }
        public string? JudicialOfficerLastName { get; set; }
        public string? JudicialOfficerType { get; set; }
    }
}
