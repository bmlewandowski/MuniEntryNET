using System.Net.Http.Json;
using System.Threading.Tasks;
using Munientry.Poc.Api.Data;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Munientry.Poc.Api.Tests
{
    public class LeapAdmissionAlreadyValidApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public LeapAdmissionAlreadyValidApiTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task PostLeapAdmissionAlreadyValid_ReturnsDocx()
        {
            var client = _factory.CreateClient();
            var dto = new LeapAdmissionAlreadyValidDto
            {
                DefendantFirstName = "Test",
                DefendantLastName = "User",
                CaseNumber = "2026-CR-006",
                DefenseCounselName = "Jane Smith",
                DefenseCounselType = "Public Defender",
                DefenseCounselWaived = false,
                AppearanceReason = "LEAP admission",
                AdmissionDate = System.DateTime.Today,
                Charges = "OVI Alcohol / Drugs 1st",
                ChargeStatute = "4511.19A1A",
                ChargeDegree = "Misdemeanor",
                ChargePlea = "Guilty",
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
                OtherConditions = false
            };
            var response = await client.PostAsJsonAsync("/api/leapadmissionalreadyvalid", dto);
            response.EnsureSuccessStatusCode();
            Assert.Equal(
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                response.Content.Headers.ContentType?.MediaType);
            var bytes = await response.Content.ReadAsByteArrayAsync();
            Assert.True(bytes.Length > 0, "Response body should contain DOCX content.");
        }
    }
}
