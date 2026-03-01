using System.Net.Http.Json;
using System.Threading.Tasks;
using Munientry.Api.Data;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Munientry.Api.Tests
{
    public class LeapValidSentencingApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public LeapValidSentencingApiTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task PostLeapValidSentencing_ReturnsDocx()
        {
            var client = _factory.CreateClient();
            var dto = new LeapValidSentencingDto
            {
                DefendantFirstName = "Test",
                DefendantLastName = "User",
                CaseNumber = "2026-CR-008",
                Date = System.DateTime.Today,
                DefenseCounselName = "Jane Smith",
                DefenseCounselType = "Public Defender",
                DefenseCounselWaived = false,
                AppearanceReason = "LEAP valid sentencing",
                Charges = "OVI Alcohol / Drugs 1st",
                ChargeStatute = "4511.19A1A",
                ChargeDegree = "M1",
                ChargePlea = "Guilty",
            };
            var response = await client.PostAsJsonAsync("/api/v1/leapvalidsentencing", dto);
            response.EnsureSuccessStatusCode();
            Assert.Equal(
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                response.Content.Headers.ContentType?.MediaType);
            var bytes = await response.Content.ReadAsByteArrayAsync();
            Assert.True(bytes.Length > 0, "Response body should contain DOCX content.");
        }
    }
}
