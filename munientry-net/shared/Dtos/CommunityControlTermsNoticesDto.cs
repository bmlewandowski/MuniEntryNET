using System;
using System.ComponentModel.DataAnnotations;

namespace Munientry.Shared.Dtos
{
    public class CommunityControlTermsNoticesDto
    {
        [Required]
        public string? CaseNumber { get; set; }

        [Required]
        public DateTime EntryDate { get; set; }

        [Required]
        public string? DefendantFirstName { get; set; }

        [Required]
        public string? DefendantLastName { get; set; }

        [Required]
        public DateTime HearingDate { get; set; }

        [Required]
        public string? HearingTime { get; set; }

        [Required]
        public string? AssignedCourtroom { get; set; }

        public string? DefenseCounselName { get; set; }

        public string? ViolationType { get; set; }

        public bool InterpreterRequired { get; set; }

        public string? LanguageRequired { get; set; }
        public string? JudicialOfficerFirstName { get; set; }
        public string? JudicialOfficerLastName { get; set; }
        public string? JudicialOfficerType { get; set; }
    }
}
