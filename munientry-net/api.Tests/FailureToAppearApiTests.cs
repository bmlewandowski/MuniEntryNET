using System.Net.Http.Json;
using System.Threading.Tasks;
using Munientry.Poc.Api.Data;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;

namespace munientry_poc.api.Tests
{
    public class FailureToAppearApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public FailureToAppearApiTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task PostFailureToAppear_ReturnsDocx()
        {
            var dto = new FailureToAppearDto
            {
                DefendantFirstName = "John",
                DefendantLastName = "Doe",
                CaseNumber = "2026-CRB-001",
                EntryDate = System.DateTime.Today,
                DefenseCounselName = "Jane Smith",
                DefenseCounselType = "Public Defender",
                CounselWaived = false,
                AppearanceReason = "arraignment",
                ArrestWarrantIssued = true,
                ArrestWarrantRadius = "county only",
                SetBond = true,
                BondType = "Cash or Surety Bond",
                BondAmount = "500"
            };

            var client = _factory.CreateClient();
            var response = await client.PostAsJsonAsync("/api/failuretooappear", dto);
            response.EnsureSuccessStatusCode();
            Assert.Equal(
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                response.Content.Headers.ContentType?.MediaType);
            var bytes = await response.Content.ReadAsByteArrayAsync();
            Assert.True(bytes.Length > 0, "Response body should contain DOCX content.");
        }
    }
}
