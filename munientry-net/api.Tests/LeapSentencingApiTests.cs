using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using Munientry.Shared.Dtos;
using Munientry.Api.Tests.Infrastructure;

namespace Munientry.Api.Tests
{
    public class LeapSentencingApiTests : IClassFixture<MuniEntryWebApplicationFactory>
    {
        private readonly MuniEntryWebApplicationFactory _factory;
        public LeapSentencingApiTests(MuniEntryWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task PostLeapSentencing_ReturnsDocx()
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
                LeapPleaDate = System.DateTime.Today.AddDays(-1),
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
                OtherConditions = false,
                ChargeOffense = "OVI Alcohol / Drugs 1st",
                ChargeStatute = "4511.19A1A",
                ChargeDegree = "M1",
                ChargePlea = "Guilty",
                ChargeFinding = "Guilty",
                ChargeFinesAmount = "375.00",
                ChargeFinesSuspended = "0.00",
                JudicialOfficerFirstName = "John",
                JudicialOfficerLastName  = "Doe",
                JudicialOfficerType      = "Judge",
            };
            var response = await client.PostAsJsonAsync("/api/v1/leapsentencing", dto);
            response.EnsureSuccessStatusCode();
            Assert.Equal(
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                response.Content.Headers.ContentType?.MediaType);
            var bytes = await response.Content.ReadAsByteArrayAsync();
            Assert.True(bytes.Length > 0, "Response body should contain DOCX content.");
        }
    }
}
