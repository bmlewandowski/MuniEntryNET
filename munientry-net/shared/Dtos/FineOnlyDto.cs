using System;

namespace Munientry.Shared.Dtos
{
    public class FineOnlyDto
    {
        public string? CaseNumber { get; set; }
        public string? DefendantName { get; set; }
        public string? Charge { get; set; }
        public decimal? FineAmount { get; set; }
        public string? JudicialOfficerFirstName { get; set; }
        public string? JudicialOfficerLastName { get; set; }
        public string? JudicialOfficerType { get; set; }
    }
}
