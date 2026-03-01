using System.Net.Http.Json;
using System.Threading.Tasks;
using Munientry.Api.Data;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Munientry.Api.Tests
{
    public class DiversionPleaApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public DiversionPleaApiTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task PostDiversionPlea_ReturnsDocx()
        {
            var client = _factory.CreateClient();
            var dto = new DiversionPleaDto
            {
                DefendantFirstName = "Test",
                DefendantLastName = "User",
                CaseNumber = "2026-CR-004",
                Date = System.DateTime.Today,
                DefenseCounselName = "Jane Smith",
                DefenseCounselType = "Public Defender",
                DefenseCounselWaived = false,
                AppearanceReason = "Diversion Plea",
                Charges = "M1",
                DiversionType = "Standard",
                DiversionCompletionDate = System.DateTime.Today.AddMonths(6),
                DiversionFinePayDate = System.DateTime.Today.AddDays(30),
                Probation = false,
                PayRestitution = false,
                PayRestitutionTo = null,
                PayRestitutionAmount = null,
                OtherConditions = false,
                OtherConditionsText = null
            };
            var response = await client.PostAsJsonAsync("/api/v1/diversionplea", dto);
            response.EnsureSuccessStatusCode();
            Assert.Equal(
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                response.Content.Headers.ContentType?.MediaType);
            var bytes = await response.Content.ReadAsByteArrayAsync();
            Assert.True(bytes.Length > 0, "Response body should contain DOCX content.");
        }
    }
}
