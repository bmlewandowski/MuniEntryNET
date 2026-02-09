using System;
using System.ComponentModel.DataAnnotations;

namespace Munientry.Poc.Api.Data
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
    }
}
