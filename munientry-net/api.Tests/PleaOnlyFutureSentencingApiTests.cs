using System.Net.Http.Json;
using System.Threading.Tasks;
using Munientry.Poc.Api.Data;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Munientry.Poc.Api.Tests
{
    public class PleaOnlyFutureSentencingApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public PleaOnlyFutureSentencingApiTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task PostPleaOnlyFutureSentencing_ReturnsDocx()
        {
            var client = _factory.CreateClient();
            var dto = new PleaOnlyFutureSentencingDto
            {
                DefendantFirstName = "Test",
                DefendantLastName = "User",
                CaseNumber = "2026-CR-011",
                DefenseCounselName = "Jane Smith",
                DefenseCounselType = "Public Defender",
                DefenseCounselWaived = false,
                AppearanceReason = "Plea",
                PleaDate = System.DateTime.Today,
                SentencingDate = System.DateTime.Today.AddDays(30),
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
                OtherConditions = false
            };
            var response = await client.PostAsJsonAsync("/api/pleaonlyfuturesentencing", dto);
            response.EnsureSuccessStatusCode();
            Assert.Equal(
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                response.Content.Headers.ContentType?.MediaType);
            var bytes = await response.Content.ReadAsByteArrayAsync();
            Assert.True(bytes.Length > 0, "Response body should contain DOCX content.");
        }
    }
}
