using System.ComponentModel.DataAnnotations;

namespace Munientry.Poc.Api.Data
{
    public class GeneralNoticeOfHearingDto
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
        public DateTime HearingDate { get; set; }
        [Required]
        public string HearingTime { get; set; }
        [Required]
        public string AssignedCourtroom { get; set; }
        public string DefenseCounselName { get; set; }
        public bool InterpreterRequired { get; set; }
        public string LanguageRequired { get; set; }
    }
}
