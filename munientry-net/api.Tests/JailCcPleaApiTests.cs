using System.Net.Http.Json;
using System.Threading.Tasks;
using Munientry.Api.Data;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Munientry.Api.Tests
{
    public class JailCcPleaApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public JailCcPleaApiTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task PostJailCcPlea_ReturnsDocx()
        {
            var client = _factory.CreateClient();
            var dto = new JailCcPleaDto
            {
                DefendantFirstName = "Test",
                DefendantLastName = "User",
                CaseNumber = "2026-CR-005",
                Date = System.DateTime.Today,
                DefenseCounselName = "Jane Smith",
                DefenseCounselType = "Public Defender",
                DefenseCounselWaived = false,
                AppearanceReason = "Sentencing",
                Charges = "OVI Alcohol / Drugs 1st",
                ChargeStatute = "4511.19A1A",
                ChargeDegree = "M1",
                ChargePlea = "Guilty",
                ChargeFinding = "Guilty",
                ChargeFinesAmount = "375.00",
                ChargeFinesSuspended = "0.00",
                ChargeJailDays = "180",
                ChargeJailDaysSuspended = "174",
                OffenseOfViolence = false,
                VictimStatements = false,
                VictimNotification = false,
                Impoundment = false,
                CommunityControl = false,
                LicenseSuspension = false,
                CommunityService = false,
                OtherConditions = false,
                AdditionalConditions = false,
                JailTimeCreditDays = 0,
                JailTimeCreditApply = "N/A",
                InJail = "No",
                CourtCosts = "Yes",
                TimeToPay = "forthwith",
                DueDate = System.DateTime.Today.AddDays(30),
                FraInFile = "N/A",
                FraInCourt = "N/A",
                DistractedDriving = false
            };
            var response = await client.PostAsJsonAsync("/api/v1/jailccplea", dto);
            response.EnsureSuccessStatusCode();
            Assert.Equal(
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                response.Content.Headers.ContentType?.MediaType);
            var bytes = await response.Content.ReadAsByteArrayAsync();
            Assert.True(bytes.Length > 0, "Response body should contain DOCX content.");
        }
    }
}
