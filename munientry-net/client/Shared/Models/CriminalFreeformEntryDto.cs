using System;
using System.ComponentModel.DataAnnotations;

namespace Munientry.Client.Shared.Models
{
    public class CriminalFreeformEntryDto
    {
        [Required(ErrorMessage = "First name is required.")]
        public string? DefendantFirstName { get; set; }
        [Required(ErrorMessage = "Last name is required.")]
        public string? DefendantLastName { get; set; }
        [Required(ErrorMessage = "Case number is required.")]
        public string? CaseNumber { get; set; }
        public DateTime? EntryDate { get; set; } = DateTime.Today;
        public string? DefenseCounselName { get; set; }
        public string? DefenseCounselType { get; set; }
        public bool CounselWaived { get; set; }
        public string? EntryContent { get; set; }
    }
}
