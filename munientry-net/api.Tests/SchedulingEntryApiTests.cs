using System.Net.Http.Json;
using System.Threading.Tasks;
using Munientry.Poc.Api.Data;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;

namespace munientry_poc.api.Tests
{
    public class SchedulingEntryApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public SchedulingEntryApiTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        private static SchedulingEntryDto BuildDto(string judicialOfficer) => new()
        {
            JudicialOfficer           = judicialOfficer,
            DefendantFirstName        = "John",
            DefendantLastName         = "Doe",
            CaseNumber                = "2024-CR-123",
            ArrestSummonsDate         = System.DateTime.Today,
            DefenseCounsel            = "Jane Smith",
            HighestCharge             = "M1",
            InJail                    = false,
            DaysInJail                = 0,
            ContinuanceDays           = 0,
            PretrialOption            = "Pretrial 4 weeks before trial",
            PretrialDate              = System.DateTime.Today.AddDays(7),
            FinalPretrialDate         = System.DateTime.Today.AddDays(14),
            FinalPretrialTime         = "1:00 PM",
            JuryTrialDate             = System.DateTime.Today.AddDays(21),
            InterpreterRequired       = false,
            LanguageRequired          = "",
            DatesConfirmedWithCounsel = true,
        };

        private async Task AssertDocxResponse(string judicialOfficer)
        {
            var client   = _factory.CreateClient();
            var response = await client.PostAsJsonAsync("/api/schedulingentry", BuildDto(judicialOfficer));
            response.EnsureSuccessStatusCode();
            Assert.Equal(
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                response.Content.Headers.ContentType?.MediaType);
            var bytes = await response.Content.ReadAsByteArrayAsync();
            Assert.True(bytes.Length > 0, $"{judicialOfficer} scheduling entry response body should contain DOCX content.");
        }

        [Fact]
        public async Task PostSchedulingEntry_Rohrer_ReturnsDocx()
            => await AssertDocxResponse("Rohrer");

        [Fact]
        public async Task PostSchedulingEntry_Fowler_ReturnsDocx()
            => await AssertDocxResponse("Fowler");

        [Fact]
        public async Task PostSchedulingEntry_Hemmeter_ReturnsDocx()
            => await AssertDocxResponse("Hemmeter");
    }
}
