using System.ComponentModel.DataAnnotations;

namespace Munientry.Poc.Api.Data
{
    public class FiscalJournalEntryDto
    {
        [Required]
        public string? AccountNumber { get; set; }
        public string? AccountName { get; set; }
        public string? SubaccountNumber { get; set; }
        public string? SubaccountName { get; set; }
        public string? DisbursementReason { get; set; }
        public decimal? DisbursementAmount { get; set; }
        public string? DisbursementVendor { get; set; }
        public string? InvoiceNumber { get; set; }
        public string? JudicialOfficer { get; set; }
    }
}
