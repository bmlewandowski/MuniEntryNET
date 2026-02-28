using System.ComponentModel.DataAnnotations;

namespace Munientry.Poc.Api.Data
{
    public class CriminalFreeformEntryDto
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
        public string? EntryContent { get; set; }
    }
}
