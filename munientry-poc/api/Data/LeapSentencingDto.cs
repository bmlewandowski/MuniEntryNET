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
        public string? CourtCosts { get; set; }
        public string? AbilityToPay { get; set; }
        public string? PayToday { get; set; }
        public string? MonthlyPay { get; set; }
        public string? JailTimeCreditDays { get; set; }
        public string? FraInFile { get; set; }
        public string? FraInCourt { get; set; }

        // ...existing LeapSentencingDto properties...
    }
}
