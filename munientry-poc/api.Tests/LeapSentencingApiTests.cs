using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using Munientry.Poc.Api.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Munientry.Poc.Api.Tests
{
    public class LeapSentencingApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        public LeapSentencingApiTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task PostLeapSentencing_ReturnsOkAndSavesEntry()
        {
            var client = _factory.CreateClient();
            var dto = new LeapSentencingDto
            {
                DefendantFirstName = "Test",
                DefendantLastName = "User",
                CaseNumber = "CASE123",
                DefenseCounselName = "Counsel Name",
                DefenseCounselType = "Public Defender",
                DefenseCounselWaived = false,
                AppearanceReason = "LEAP sentencing",
                LeapPleaDate = System.DateTime.Today,
                PleaTrialDate = System.DateTime.Today,
                CourtCosts = "Yes",
                AbilityToPay = "forthwith",
                BalanceDueDate = System.DateTime.Today.AddDays(30),
                PayToday = "100",
                MonthlyPay = "50",
                CreditForJail = false,
                JailTimeCreditDays = "0",
                FraInFile = "N/A",
                FraInCourt = "N/A",
                LicenseSuspension = false,
                CommunityService = false,
                OtherConditions = false
            };
            var response = await client.PostAsJsonAsync("/api/leapsentencing", dto);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<dynamic>();
            Assert.Equal("saved", (string)result.status);
            Assert.NotNull(result.entryId);
        }
    }
}
