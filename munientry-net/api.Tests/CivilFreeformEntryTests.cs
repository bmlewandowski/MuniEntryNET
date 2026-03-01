using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using Munientry.Api.Data;

namespace Munientry.Api.Tests
{
    public class CivilFreeformEntryTests
    {
        private readonly Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory<Program> _factory = new();

        [Fact]
        public async Task CivilFreeformEntry_Post_Works()
        {
            var client = _factory.CreateClient();
            var dto = new CivilFreeformEntryDto
            {
                EntryDate = System.DateTime.Today,
                Plaintiff = "Jane Roe",
                Defendant = "John Doe",
                CaseNumber = "2026-CV-00002",
                AppearanceReason = "motion hearing",
                EntryContent = "This is a civil freeform entry."
            };
            var response = await client.PostAsJsonAsync("/api/v1/civilfreeformentry", dto);
            response.EnsureSuccessStatusCode();
        }
    }
}
