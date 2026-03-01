using System.ComponentModel.DataAnnotations;

namespace Munientry.Api.Data
{
    public class FailureToAppearDto
    {
        [Required]
        public string? DefendantFirstName { get; set; }
        [Required]
        public string? DefendantLastName { get; set; }
        [Required]
        public string? CaseNumber { get; set; }
        public DateTime? EntryDate { get; set; }
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
