using System.Net.Http.Json;
using System.Threading.Tasks;
using Munientry.Api.Data;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Munientry.Api.Tests
{
    public class CommunityServiceSecondaryApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public CommunityServiceSecondaryApiTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task PostCommunityServiceSecondary_ReturnsDocx()
        {
            var client = _factory.CreateClient();
            var dto = new CommunityServiceSecondaryDto
            {
                CommunityServiceOrdered = true,
                CommunityServiceHours = "40",
                CommunityServiceDaysToComplete = "90",
                CommunityServiceDueDate = System.DateTime.Today.AddDays(90),
                LicenseSuspensionOrdered = false,
                LicenseSuspensionLength = null,
                FingerprintingOrdered = false,
                VictimNotificationOrdered = false,
                ImmobilizeImpoundOrdered = false,
                OtherConditions = null
            };
            var response = await client.PostAsJsonAsync("/api/v1/communityservicesecondary", dto);
            response.EnsureSuccessStatusCode();
            Assert.Equal(
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                response.Content.Headers.ContentType?.MediaType);
            var bytes = await response.Content.ReadAsByteArrayAsync();
            Assert.True(bytes.Length > 0, "Response body should contain DOCX content.");
        }
    }
}
