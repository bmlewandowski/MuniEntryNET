using System;

namespace Munientry.Api.Data
{
    public class FineOnlyDto
    {
        public string? CaseNumber { get; set; }
        public string? DefendantName { get; set; }
        public string? Charge { get; set; }
        public decimal? FineAmount { get; set; }
    }
}
