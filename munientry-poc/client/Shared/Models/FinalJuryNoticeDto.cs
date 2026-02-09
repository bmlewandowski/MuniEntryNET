using System;
using System.ComponentModel.DataAnnotations;

namespace munientry_poc.client.Shared.Models
{
    public class FinalJuryNoticeDto
    {
        [Required]
        public string? DefendantFirstName { get; set; }
        [Required]
        public string? DefendantLastName { get; set; }
        [Required]
        public string? CaseNumber { get; set; }
        public DateTime? EntryDate { get; set; }
        public string? DefenseCounselName { get; set; }
        public DateTime? ArrestSummonsDate { get; set; }
        public string? HighestCharge { get; set; }
        public int? DaysInJail { get; set; }
        public int? ContinuanceDays { get; set; }
        public DateTime? TrialToCourtDate { get; set; }
        public string? TrialToCourtTime { get; set; }
        public string? AssignedCourtroom { get; set; }
        public bool InterpreterRequired { get; set; }
        public string? LanguageRequired { get; set; }
        public bool DateConfirmedWithCounsel { get; set; }
    }
}
