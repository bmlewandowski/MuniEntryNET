using System;
using System.ComponentModel.DataAnnotations;

namespace Munientry.Shared.Dtos
{
    public class FinalJuryNoticeDto
    {
        [Required]
        public string? DefendantFirstName { get; set; }

        [Required]
        public string? DefendantLastName { get; set; }

        [Required]
        public string? CaseNumber { get; set; }

        [Required]
        public DateTime? EntryDate { get; set; }

        public string? DefenseCounselName { get; set; }

        // Added to match test usage
        public DateTime? FinalJuryDate { get; set; }
        public string? FinalJuryTime { get; set; }
        public string? AssignedCourtroom { get; set; }
        public bool InterpreterRequired { get; set; }
        public string? LanguageRequired { get; set; }
        public bool DateConfirmedWithCounsel { get; set; }

        // Fields used by TrialSentencing form
        public DateTime? ArrestSummonsDate { get; set; }
        public string? HighestCharge { get; set; }
        public int? DaysInJail { get; set; }
        public int? ContinuanceDays { get; set; }
        public DateTime? TrialToCourtDate { get; set; }
        public string? TrialToCourtTime { get; set; }
        public string? JudicialOfficerFirstName { get; set; }
        public string? JudicialOfficerLastName { get; set; }
        public string? JudicialOfficerType { get; set; }
    }
}
