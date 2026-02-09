using System;
using System.ComponentModel.DataAnnotations;

namespace Munientry.Poc.Api.Data
{
    public class NoticesFreeformCivilDto
    {
        [Required]
        public string CaseNumber { get; set; }

        [Required]
        public DateTime EntryDate { get; set; }

        [Required]
        public string DefendantFirstName { get; set; }

        [Required]
        public string DefendantLastName { get; set; }

        [Required]
        public string NoticeText { get; set; }
    }
}
