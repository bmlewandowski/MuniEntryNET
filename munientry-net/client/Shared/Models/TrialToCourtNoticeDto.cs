using System;
using System.ComponentModel.DataAnnotations;

namespace Munientry.Poc.Client.Shared.Models
{
    public class TrialToCourtNoticeDto
    {
        [Required(ErrorMessage = "Defendant First Name is required")]
        public string? DefendantFirstName { get; set; }

        [Required(ErrorMessage = "Defendant Last Name is required")]
        public string? DefendantLastName { get; set; }

        [Required(ErrorMessage = "Case Number is required")]
        public string? CaseNumber { get; set; }

        [Required(ErrorMessage = "Entry Date is required")]
        public DateTime? EntryDate { get; set; }

        public string? DefenseCounselName { get; set; }

        [Required(ErrorMessage = "Trial To Court Date is required")]
        public DateTime? TrialToCourtDate { get; set; }

        [Required(ErrorMessage = "Trial To Court Time is required")]
        public string? TrialToCourtTime { get; set; }

        [Required(ErrorMessage = "Assigned Courtroom is required")]
        public string? AssignedCourtroom { get; set; }

        public bool InterpreterRequired { get; set; }

        public string? LanguageRequired { get; set; }

        public bool DateConfirmedWithCounsel { get; set; }
    }
}
