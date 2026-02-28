using System;
using System.ComponentModel.DataAnnotations;

namespace Munientry.Poc.Api.Data
{
    public class BondModificationRevocationDto
    {
        [Required]
        public string? DefendantFirstName { get; set; }

        [Required]
        public string? DefendantLastName { get; set; }

        [Required]
        public string? CaseNumber { get; set; }

        [Required]
        public DateTime? EntryDate { get; set; }

        [Required]
        public string? DecisionOnBond { get; set; }

        public string? BondType { get; set; }
        public string? BondAmount { get; set; }
        public string? DefenseCounselName { get; set; }
    }
}
