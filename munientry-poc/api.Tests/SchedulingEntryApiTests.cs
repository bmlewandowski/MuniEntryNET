using System.Net.Http.Json;
using System.Threading.Tasks;
using munientry_poc.api.DTOs;
using Xunit;

namespace munientry_poc.api.Tests
{
    public class SchedulingEntryApiTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public SchedulingEntryApiTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task PostSchedulingEntry_ReturnsOk()
        {
            var dto = new SchedulingEntryDto
            {
                DefendantFirstName = "John",
                DefendantLastName = "Doe",
                CaseNumber = "2024-CR-123",
                ArrestSummonsDate = System.DateTime.Today,
                DefenseCounsel = "Jane Smith",
                HighestCharge = "M1",
                InJail = false,
                DaysInJail = 0,
                ContinuanceDays = 0,
                PretrialOption = "4weeks",
                PretrialDate = System.DateTime.Today.AddDays(7),
                FinalPretrialDate = System.DateTime.Today.AddDays(14),
                FinalPretrialTime = "9:00 AM",
                JuryTrialDate = System.DateTime.Today.AddDays(21),
                InterpreterRequired = false,
                LanguageRequired = "",
                DatesConfirmedWithCounsel = true
            };

            var response = await _client.PostAsJsonAsync("/api/schedulingentry", dto);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<dynamic>();
            Assert.Equal("saved", (string)result.status);
        }
    }
}
