using System.Net.Http.Json;
using System.Threading.Tasks;
using Munientry.Poc.Api.Data;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;

namespace munientry_poc.api.Tests
{
    public class CriminalFreeformEntryApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public CriminalFreeformEntryApiTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task PostCriminalFreeformEntry_ReturnsDocx()
        {
            var dto = new CriminalFreeformEntryDto
            {
                DefendantFirstName = "John",
                DefendantLastName = "Doe",
                CaseNumber = "2026-CRB-005",
                EntryDate = System.DateTime.Today,
                DefenseCounselName = "Jane Smith",
                DefenseCounselType = "Public Defender",
                CounselWaived = false,
                EntryContent = "This matter came before the Court for hearing. The Court orders that the defendant shall appear at the next scheduled hearing date. IT IS SO ORDERED."
            };

            var client = _factory.CreateClient();
            var response = await client.PostAsJsonAsync("/api/freeformentry", dto);
            response.EnsureSuccessStatusCode();
            Assert.Equal(
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                response.Content.Headers.ContentType?.MediaType);
            var bytes = await response.Content.ReadAsByteArrayAsync();
            Assert.True(bytes.Length > 0, "Response body should contain DOCX content.");
        }
    }
}
