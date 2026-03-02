using System;
using System.ComponentModel.DataAnnotations;

namespace Munientry.Shared.Dtos
{
    public class BondHearingDto
    {
        [Required]
        public string? DefendantFirstName { get; set; }

        [Required]
        public string? DefendantLastName { get; set; }

        [Required]
        public string? CaseNumber { get; set; }

        [Required]
        public DateTime? EntryDate { get; set; }

        public string? BondType { get; set; }
        public string? BondAmount { get; set; }
        public string? BondModificationDecision { get; set; }
        public string? DefenseCounselName { get; set; }
        public bool DefenseCounselWaived { get; set; }
        public string? JudicialOfficerFirstName { get; set; }
        public string? JudicialOfficerLastName { get; set; }
        public string? JudicialOfficerType { get; set; }
    }
}
