using System.ComponentModel.DataAnnotations;
using Munientry.Shared.Validation;

namespace Munientry.Shared.Dtos
{
    public class SchedulingEntryDto
    {
        [Required]
        public string? DefendantFirstName { get; set; }
        [Required]
        public string? DefendantLastName { get; set; }
        [Required]
        public string? CaseNumber { get; set; }
        [Required]
        public DateTime? ArrestSummonsDate { get; set; }
        public string? DefenseCounsel { get; set; }
        public string? HighestCharge { get; set; }
        public bool InJail { get; set; }
        public int? DaysInJail { get; set; }
        public int? ContinuanceDays { get; set; }
        public string? PretrialOption { get; set; }
        public DateTime? PretrialDate { get; set; }
        [FutureDate]
        public DateTime? FinalPretrialDate { get; set; }
        public string? FinalPretrialTime { get; set; }
        [Required]
        [FutureDate]
        public DateTime? JuryTrialDate { get; set; }
        public bool InterpreterRequired { get; set; }
        public string? LanguageRequired { get; set; }
        public bool DatesConfirmedWithCounsel { get; set; }
        /// <summary>"Rohrer", "Fowler", or "Hemmeter" � determines which template is used.</summary>
        public string? JudicialOfficer { get; set; }
        public string? JudicialOfficerFirstName { get; set; }
        public string? JudicialOfficerLastName { get; set; }
        public string? JudicialOfficerType { get; set; }
    }
}
