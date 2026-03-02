using System;
using System.ComponentModel.DataAnnotations;
using Munientry.Shared.Validation;

namespace Munientry.Shared.Dtos
{
    public class LeapValidSentencingDto
    {
        [Required]
        public string? DefendantFirstName { get; set; }
        [Required]
        public string? DefendantLastName { get; set; }
        [Required]
        public string? CaseNumber { get; set; }
        [Required]
        [PastDate]
        public DateTime? Date { get; set; }
        public string? DefenseCounselName { get; set; }
        public string? DefenseCounselType { get; set; }
        public bool DefenseCounselWaived { get; set; }
        public string? AppearanceReason { get; set; }
        public string? Charges { get; set; }
        public string? ChargeStatute { get; set; }
        public string? ChargeDegree { get; set; }
        public string? ChargePlea { get; set; }

        /// <summary>
        /// All charges for the entry. Drives the <c>{%tc for charge in charges_list %}</c>
        /// table-row loop in the DOCX template.
        /// </summary>
        public List<ChargeItemDto> ChargeItems { get; set; } = new();
        public string? JudicialOfficerFirstName { get; set; }
        public string? JudicialOfficerLastName { get; set; }
        public string? JudicialOfficerType { get; set; }
    }
}
