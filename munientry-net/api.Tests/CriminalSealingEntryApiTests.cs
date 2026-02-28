using System.Net.Http.Json;
using System.Threading.Tasks;
using Munientry.Poc.Api.Data;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;

namespace munientry_poc.api.Tests
{
    public class CriminalSealingEntryApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public CriminalSealingEntryApiTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task PostCriminalSealingGranted_ReturnsDocx()
        {
            var dto = new CriminalSealingEntryDto
            {
                DefendantFirstName = "John",
                DefendantLastName = "Doe",
                CaseNumber = "2026-CRB-002",
                EntryDate = System.DateTime.Today,
                DefenseCounselName = "Jane Smith",
                DefenseCounselType = "Private Counsel",
                CounselWaived = false,
                BciNumber = "A123456",
                SealDecision = "Granted",
                DenialReasons = null
            };

            var client = _factory.CreateClient();
            var response = await client.PostAsJsonAsync("/api/criminalsealing", dto);
            response.EnsureSuccessStatusCode();
            Assert.Equal(
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                response.Content.Headers.ContentType?.MediaType);
            var bytes = await response.Content.ReadAsByteArrayAsync();
            Assert.True(bytes.Length > 0, "Response body should contain DOCX content.");
        }

        [Fact]
        public async Task PostCriminalSealingDeniedWithReason_ReturnsDocx()
        {
            var dto = new CriminalSealingEntryDto
            {
                DefendantFirstName = "Jane",
                DefendantLastName = "Smith",
                CaseNumber = "2026-CRB-003",
                EntryDate = System.DateTime.Today,
                DefenseCounselName = "Bob Jones",
                DefenseCounselType = "Public Defender",
                CounselWaived = false,
                BciNumber = "B789012",
                SealDecision = "Denied - with reason",
                DenialReasons = "Defendant has a prior felony conviction within the past 10 years."
            };

            var client = _factory.CreateClient();
            var response = await client.PostAsJsonAsync("/api/criminalsealing", dto);
            response.EnsureSuccessStatusCode();
            Assert.Equal(
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                response.Content.Headers.ContentType?.MediaType);
            var bytes = await response.Content.ReadAsByteArrayAsync();
            Assert.True(bytes.Length > 0, "Response body should contain DOCX content.");
        }
    }
}
