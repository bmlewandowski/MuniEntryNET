using System;
using System.ComponentModel.DataAnnotations;

namespace Munientry.Poc.Api.Data
{
    public class TrialToCourtNoticeDto
    {
        [Required(ErrorMessage = "First name is required.")]
        public string? DefendantFirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        public string? DefendantLastName { get; set; }

        [Required(ErrorMessage = "Case number is required.")]
        public string? CaseNumber { get; set; }

        [Required(ErrorMessage = "Entry date is required.")]
        public DateTime? EntryDate { get; set; }

        public string? DefenseCounselName { get; set; }

        [Required(ErrorMessage = "Trial date is required.")]
        public DateTime? TrialToCourtDate { get; set; }

        [Required(ErrorMessage = "Trial time is required.")]
        public string? TrialToCourtTime { get; set; }

        [Required(ErrorMessage = "Assigned courtroom is required.")]
        public string? AssignedCourtroom { get; set; }

        public bool InterpreterRequired { get; set; }

        public string? LanguageRequired { get; set; }

        public bool DateConfirmedWithCounsel { get; set; }
    }
}