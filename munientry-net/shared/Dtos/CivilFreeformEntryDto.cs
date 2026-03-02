using System;
using System.ComponentModel.DataAnnotations;

namespace Munientry.Shared.Dtos
{
    public class CivilFreeformEntryDto
    {
        [Required]
        public DateTime EntryDate { get; set; }
        [Required]
        public string? Plaintiff { get; set; }
        [Required]
        public string? Defendant { get; set; }
        [Required]
        public string? CaseNumber { get; set; }
        public string? AppearanceReason { get; set; }
        [Required]
        public string? EntryContent { get; set; }
        public string? JudicialOfficerFirstName { get; set; }
        public string? JudicialOfficerLastName { get; set; }
        public string? JudicialOfficerType { get; set; }
    }
}
