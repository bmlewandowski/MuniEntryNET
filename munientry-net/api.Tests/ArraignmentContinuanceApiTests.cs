using System.Net.Http.Json;
using System.Threading.Tasks;
using Munientry.Api.Data;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Munientry.Api.Tests
{
    public class ArraignmentContinuanceApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public ArraignmentContinuanceApiTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task PostArraignmentContinuance_ReturnsDocx()
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

            var client = _factory.CreateClient();
            var response = await client.PostAsJsonAsync("/api/v1/arraignmentcontinuance", dto);
            response.EnsureSuccessStatusCode();
            Assert.Equal(
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                response.Content.Headers.ContentType?.MediaType);
            var bytes = await response.Content.ReadAsByteArrayAsync();
            Assert.True(bytes.Length > 0, "Response body should contain DOCX content.");
        }
    }
}
