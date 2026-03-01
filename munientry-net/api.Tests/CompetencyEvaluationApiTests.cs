using System.Net.Http.Json;
using System.Threading.Tasks;
using Munientry.Api.Data;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Munientry.Api.Tests
{
    public class CompetencyEvaluationApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public CompetencyEvaluationApiTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task PostCompetencyEvaluation_ReturnsDocx()
        {
            var dto = new CompetencyEvaluationDto
            {
                DefendantFirstName = "John",
                DefendantLastName = "Doe",
                CaseNumber = "2026-CRB-004",
                EntryDate = System.DateTime.Today,
                DefenseCounselName = "Jane Smith",
                DefenseCounselType = "Public Defender",
                CounselWaived = false,
                EvaluatorName = "Dr. Robert Brown",
                EvaluationDate = System.DateTime.Today.AddDays(14),
                CompetencyHearingDate = System.DateTime.Today.AddDays(45),
                CompetencyHearingType = "competency hearing"
            };

            var client = _factory.CreateClient();
            var response = await client.PostAsJsonAsync("/api/v1/competencyevaluation", dto);
            response.EnsureSuccessStatusCode();
            Assert.Equal(
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                response.Content.Headers.ContentType?.MediaType);
            var bytes = await response.Content.ReadAsByteArrayAsync();
            Assert.True(bytes.Length > 0, "Response body should contain DOCX content.");
        }
    }
}
