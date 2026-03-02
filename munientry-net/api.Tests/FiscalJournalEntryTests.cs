using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using Munientry.Shared.Dtos;
using Munientry.Api.Tests.Infrastructure;

namespace Munientry.Api.Tests
{
    /// <summary>
    /// Integration tests for POST /api/v1/fiscaljournalentry.
    /// FiscalJournalEntryService is a pure DOCX service (no SQL) — no stub needed;
    /// MuniEntryWebApplicationFactory is used unchanged.
    /// </summary>
    public class FiscalJournalEntryTests : IClassFixture<MuniEntryWebApplicationFactory>
    {
        private readonly MuniEntryWebApplicationFactory _factory;

        public FiscalJournalEntryTests(MuniEntryWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task FiscalJournalEntry_Post_ReturnsDocx()
        {
            var client = _factory.CreateClient();
            var dto = new FiscalJournalEntryDto
            {
                AccountNumber = "12345",
                AccountName = "General Fund",
                SubaccountNumber = "001",
                SubaccountName = "Operations",
                DisbursementReason = "Supplies",
                DisbursementAmount = 150.00m,
                DisbursementVendor = "Office Depot",
                InvoiceNumber = "INV-2026-01",
                JudicialOfficer = "Judge Smith"
            };
            var response = await client.PostAsJsonAsync("/api/v1/fiscaljournalentry", dto);
            response.EnsureSuccessStatusCode();
            Assert.Equal(
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                response.Content.Headers.ContentType?.MediaType);
            var bytes = await response.Content.ReadAsByteArrayAsync();
            Assert.True(bytes.Length > 0, "Response body should contain DOCX content.");
        }
    }
}

