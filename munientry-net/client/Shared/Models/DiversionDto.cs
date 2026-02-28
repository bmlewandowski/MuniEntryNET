using System;
using System.Collections.Generic;

namespace Munientry.Poc.Client.Shared.Models
{
    public class DiversionDto
    {
        public string? CaseNumber { get; set; }
        public string? DefendantFirstName { get; set; }
        public string? DefendantLastName { get; set; }
        // Use strings for date fields in this simple POC to avoid binding/runtime conversion issues
        public string? DiversionCompletionDate { get; set; }
        public string? DiversionFinePayDate { get; set; }
        public List<ChargeDto>? Charges { get; set; }
    }

    public class ChargeDto
    {
        public string? Charge { get; set; }
        public string? Statute { get; set; }
    }
}
