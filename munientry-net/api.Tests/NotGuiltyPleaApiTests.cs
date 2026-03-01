using System.Net.Http.Json;
using System.Threading.Tasks;
using Munientry.Api.Data;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Munientry.Api.Tests
{
    public class NotGuiltyPleaApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public NotGuiltyPleaApiTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task PostNotGuiltyPlea_ReturnsDocx()
        {
            var client = _factory.CreateClient();
            var dto = new NotGuiltyPleaDto
            {
                DefendantFirstName = "Test",
                DefendantLastName = "User",
                CaseNumber = "2026-CR-010",
                DefenseCounselName = "Jane Smith",
                DefenseCounselType = "Public Defender",
                DefenseCounselWaived = false,
                AppearanceReason = "Not guilty plea",
                PleaDate = System.DateTime.Today,
                Charges = "M1",
                CourtCosts = "Yes",
                AbilityToPay = "forthwith",
                BalanceDueDate = System.DateTime.Today.AddDays(30),
                PayToday = "100",
                MonthlyPay = "0",
                CreditForJail = false,
                JailTimeCreditDays = "0",
                FraInFile = "N/A",
                FraInCourt = "N/A",
                LicenseSuspension = false,
                CommunityService = false,
                OtherConditions = false,
                BondType = "Cash",
                BondAmount = "500",
                NoContact = false,
                NoAlcoholDrugs = false,
                AlcoholDrugsAssessment = false,
                MentalHealthAssessment = false,
                FingerprintInCourt = false,
                SpecializedDocket = false,
                SpecializedDocketType = null,
                Monitoring = false,
                MonitoringType = null,
                ComplyProtectionOrder = false,
                PublicSafetySuspension = false
            };
            var response = await client.PostAsJsonAsync("/api/v1/notguiltyplea", dto);
            response.EnsureSuccessStatusCode();
            Assert.Equal(
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                response.Content.Headers.ContentType?.MediaType);
            var bytes = await response.Content.ReadAsByteArrayAsync();
            Assert.True(bytes.Length > 0, "Response body should contain DOCX content.");
        }
    }
}
