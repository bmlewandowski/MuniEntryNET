using System;
using System.ComponentModel.DataAnnotations;

namespace Munientry.Client.Shared.Models
{
    public class FailureToAppearDto
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
        public string? AppearanceReason { get; set; }
        public bool ArrestWarrantIssued { get; set; }
        public string? ArrestWarrantRadius { get; set; }
        public bool SetBond { get; set; }
        public string? BondType { get; set; }
        public string? BondAmount { get; set; }
    }
}
