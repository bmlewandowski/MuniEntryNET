using System.Net.Http.Json;
using System.Threading.Tasks;
using Munientry.Shared.Dtos;
using Xunit;
using Munientry.Api.Tests.Infrastructure;

namespace Munientry.Api.Tests
{
    public class TrialSentencingApiTests : IClassFixture<MuniEntryWebApplicationFactory>
    {
        private readonly MuniEntryWebApplicationFactory _factory;

        public TrialSentencingApiTests(MuniEntryWebApplicationFactory factory)
        {
            _factory = factory;
        }

        // /api/trialsentencing accepts FinalJuryNoticeDto (shared DTO, fields used: ArrestSummonsDate,
        // HighestCharge, DaysInJail, ContinuanceDays, TrialToCourtDate, TrialToCourtTime, AssignedCourtroom)
        [Fact]
        public async Task PostTrialSentencing_ReturnsDocx()
        {
            var client = _factory.CreateClient();
            var dto = new FinalJuryNoticeDto
            {
                DefendantFirstName = "Test",
                DefendantLastName = "User",
                CaseNumber = "2026-CR-014",
                EntryDate = System.DateTime.Today,
                DefenseCounselName = "Jane Smith",
                ArrestSummonsDate = System.DateTime.Today.AddMonths(-3),
                HighestCharge = "M1",
                DaysInJail = 0,
                ContinuanceDays = 0,
                TrialToCourtDate = System.DateTime.Today.AddDays(14),
                TrialToCourtTime = "9:00 AM",
                AssignedCourtroom = "1A",
                InterpreterRequired = false,
                LanguageRequired = "",
                DateConfirmedWithCounsel = true
            };
            var response = await client.PostAsJsonAsync("/api/v1/trialsentencing", dto);
            response.EnsureSuccessStatusCode();
            Assert.Equal(
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                response.Content.Headers.ContentType?.MediaType);
            var bytes = await response.Content.ReadAsByteArrayAsync();
            Assert.True(bytes.Length > 0, "Response body should contain DOCX content.");
        }
    }
}
