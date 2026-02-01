using System;

namespace Munientry.Poc.Api.Data
{
    public class TrialToCourtNoticeDto
    {
        public string? DefendantFirstName { get; set; }
        public string? DefendantLastName { get; set; }
        public string? CaseNumber { get; set; }
        public DateTime? EntryDate { get; set; }
        public string? DefenseCounselName { get; set; }
        public DateTime? TrialToCourtDate { get; set; }
        public string? TrialToCourtTime { get; set; }
        public string? AssignedCourtroom { get; set; }
        public bool InterpreterRequired { get; set; }
        public string? LanguageRequired { get; set; }
        public bool DateConfirmedWithCounsel { get; set; }
    }
}