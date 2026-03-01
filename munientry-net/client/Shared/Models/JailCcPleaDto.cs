using System;
using System.ComponentModel.DataAnnotations;

namespace Munientry.Client.Shared.Models
{
    public class JailCcPleaDto
    {
        [Required]
        public string DefendantFirstName { get; set; }
        [Required]
        public string DefendantLastName { get; set; }
        [Required]
        public string CaseNumber { get; set; }
        [Required]
        public DateTime? Date { get; set; }
        public string DefenseCounselName { get; set; }
        public string DefenseCounselType { get; set; }
        public bool DefenseCounselWaived { get; set; }
        public string AppearanceReason { get; set; }
        public string Charges { get; set; }
        public string? ChargeStatute { get; set; }
        public string? ChargeDegree { get; set; }
        public string? ChargePlea { get; set; }
        public string? ChargeFinding { get; set; }
        public string? ChargeFinesAmount { get; set; }
        public string? ChargeFinesSuspended { get; set; }
        public string? ChargeJailDays { get; set; }
        public string? ChargeJailDaysSuspended { get; set; }
        public bool OffenseOfViolence { get; set; }
        public bool VictimStatements { get; set; }
        public bool VictimNotification { get; set; }
        public bool Impoundment { get; set; }
        public bool CommunityControl { get; set; }
        public bool LicenseSuspension { get; set; }
        public bool CommunityService { get; set; }
        public bool OtherConditions { get; set; }
        public bool AdditionalConditions { get; set; }
        public int? JailTimeCreditDays { get; set; }
        public string JailTimeCreditApply { get; set; }
        public string InJail { get; set; }
        public string CompanionCases { get; set; }
        public string CompanionCasesSentence { get; set; }
        public bool JailReportingTerms { get; set; }
        public bool AddJailReportDate { get; set; }
        public decimal? PayToday { get; set; }
        public decimal? MonthlyPay { get; set; }
        public string CourtCosts { get; set; }
        public string TimeToPay { get; set; }
        public DateTime? DueDate { get; set; }
        public string FraInFile { get; set; }
        public string FraInCourt { get; set; }
        public bool DistractedDriving { get; set; }
        public List<SentencingChargeItemDto> ChargeItems { get; set; } = new();
    }
}
