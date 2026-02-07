using System;
using System.ComponentModel.DataAnnotations;

namespace Munientry.Poc.Api.Data
{
    public class FinalJuryNoticeDto
    {
        [Required]
        public string? DefendantFirstName { get; set; }

        [Required]
        public string? DefendantLastName { get; set; }

        [Required]
        public string? CaseNumber { get; set; }

        [Required]
        public DateTime? EntryDate { get; set; }

        public string? DefenseCounselName { get; set; }
    }
}
