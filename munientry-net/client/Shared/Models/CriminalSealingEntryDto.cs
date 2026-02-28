using System;
using System.ComponentModel.DataAnnotations;

namespace munientry_poc.client.Shared.Models
{
    public class CriminalSealingEntryDto
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
        public string? BciNumber { get; set; }
        public string? SealDecision { get; set; }
        public string? DenialReasons { get; set; }
    }
}
