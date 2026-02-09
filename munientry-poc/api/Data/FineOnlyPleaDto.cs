using System;

namespace MuniEntry.Api.Data
{
    public class FineOnlyPleaDto
    {
        public string? DefendantFirstName { get; set; }
        public string? DefendantLastName { get; set; }
        public string? CaseNumber { get; set; }
        public DateTime? Date { get; set; }
        public string? DefenseCounselName { get; set; }
        public string? DefenseCounselType { get; set; }
        public bool DefenseCounselWaived { get; set; }
        public string? AppearanceReason { get; set; }
        public string? Charges { get; set; }
        public string? CourtCosts { get; set; }
        public string? TimeToPay { get; set; }
        public DateTime? DueDate { get; set; }
        public decimal? PayToday { get; set; }
        public decimal? MonthlyPay { get; set; }
        public bool CreditForJail { get; set; }
        public int? JailTimeCreditDays { get; set; }
        public bool LicenseSuspension { get; set; }
        public bool CommunityService { get; set; }
        public bool OtherConditions { get; set; }
        public bool DistractedDriving { get; set; }
        public string? FraInFile { get; set; }
        public string? FraInCourt { get; set; }
        public decimal? FineAmount { get; set; }
    }
}
