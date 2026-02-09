using System.Net.Http.Json;
using System.Threading.Tasks;
using munientry_poc.api.DTOs;
using Xunit;

namespace munientry_poc.api.Tests
{
    public class ArraignmentContinuanceApiTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public ArraignmentContinuanceApiTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task PostArraignmentContinuance_ReturnsOk()
        {
            var dto = new ArraignmentContinuanceDto
            {
                DefendantFirstName = "John",
                DefendantLastName = "Doe",
                CaseNumber = "2026-CR-456",
                PleaTrialDate = System.DateTime.Today,
                DefenseCounselName = "Jane Smith",
                DefenseCounselType = "Public Defender",
                CounselWaived = false,
                AppearanceReason = "arraignment",
                CurrentArraignmentDate = System.DateTime.Today.AddDays(-7),
                NewArraignmentDate = System.DateTime.Today.AddDays(7),
                ContinuanceLength = "2 Weeks",
                ContinuanceReason = "general continuance - granted"
            };

            var response = await _client.PostAsJsonAsync("/api/arraignmentcontinuance", dto);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<dynamic>();
            Assert.Equal("saved", (string)result.status);
        }
    }
}
