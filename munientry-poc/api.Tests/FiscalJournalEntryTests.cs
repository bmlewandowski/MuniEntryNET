using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using Munientry.Poc.Api.Data;
namespace Munientry.Api.Tests
{
    public class FiscalJournalEntryTests
    {
        private readonly Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory<Program> _factory = new();

        [Fact]
        public async Task FiscalJournalEntry_Post_Works()
        {
            var client = _factory.CreateClient();
            var dto = new FiscalJournalEntryDto
            {
                AccountNumber = "12345",
                AccountName = "General Fund",
                SubaccountNumber = "001",
                SubaccountName = "Operations",
                DisbursementReason = "Supplies",
                DisbursementVendor = "Office Depot",
                InvoiceNumber = "INV-2026-01",
                JudicialOfficer = "Judge Smith"
            };
            var response = await client.PostAsJsonAsync("/api/fiscaljournalentry", dto);
            response.EnsureSuccessStatusCode();
        }
    }
}
