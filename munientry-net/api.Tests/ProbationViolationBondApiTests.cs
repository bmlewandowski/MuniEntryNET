using System.Net.Http.Json;
using System.Threading.Tasks;
using Munientry.Shared.Dtos;
using Xunit;
using Munientry.Api.Tests.Infrastructure;

namespace Munientry.Api.Tests
{
    public class ProbationViolationBondApiTests : IClassFixture<MuniEntryWebApplicationFactory>
    {
        private readonly MuniEntryWebApplicationFactory _factory;

        public ProbationViolationBondApiTests(MuniEntryWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task PostProbationViolationBond_ReturnsDocx()
        {
            var client = _factory.CreateClient();
            var dto = new ProbationViolationBondDto
            {
                DefendantFirstName = "John",
                DefendantLastName = "Doe",
                CaseNumber = "2026-CR-012",
                AppearanceReason = "Probation violation",
                DefenseCounselName = "Jane Smith",
                DefenseCounselType = "Public Defender",
                DefenseCounselWaived = false,
                ProbableCauseFinding = "Yes",
                BondType = "Cash",
                BondAmount = "1000",
                NoAlcoholDrugs = false,
                Monitoring = false,
                MonitoringType = null,
                ComplyProtectionOrder = false,
                OtherConditions = false
            };
            var response = await client.PostAsJsonAsync("/api/v1/probationviolationbond", dto);
            response.EnsureSuccessStatusCode();
            Assert.Equal(
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                response.Content.Headers.ContentType?.MediaType);
            var bytes = await response.Content.ReadAsByteArrayAsync();
            Assert.True(bytes.Length > 0, "Response body should contain DOCX content.");
        }
    }
}
