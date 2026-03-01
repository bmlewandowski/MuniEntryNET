using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using Munientry.Api.Data;
using Munientry.Api.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Munientry.Api.Tests
{
    /// <summary>
    /// No-op stub that satisfies the IFiscalJournalEntryService contract without
    /// requiring a real SQL Server connection during tests.
    /// </summary>
    internal sealed class FiscalJournalEntryServiceStub : IFiscalJournalEntryService
    {
        public void InsertFiscalJournalEntry(FiscalJournalEntryDto dto) { /* no-op */ }
    }

    public class FiscalJournalEntryTests
    {
        private readonly WebApplicationFactory<Program> _factory;

        public FiscalJournalEntryTests()
        {
            _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Replace the real service (which connects to SQL Server) with a stub.
                    services.AddScoped<IFiscalJournalEntryService, FiscalJournalEntryServiceStub>();
                });
            });
        }

        [Fact]
        public async Task FiscalJournalEntry_Post_ReturnsOk()
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
        }
    }
}

