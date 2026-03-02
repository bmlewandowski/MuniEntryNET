using System.Net.Http.Json;
using System.Threading.Tasks;
using Munientry.Shared.Dtos;
using Xunit;
using Munientry.Api.Tests.Infrastructure;

namespace Munientry.Api.Tests
{
    public class BondHearingApiTests : IClassFixture<MuniEntryWebApplicationFactory>
    {
        private readonly MuniEntryWebApplicationFactory _factory;

        public BondHearingApiTests(MuniEntryWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task PostBondHearing_ReturnsDocx()
        {
            var client = _factory.CreateClient();
            var dto = new BondHearingDto
            {
                DefendantFirstName = "John",
                DefendantLastName = "Doe",
                CaseNumber = "2026-CR-001",
                EntryDate = System.DateTime.Today,
                BondType = "Cash",
                BondAmount = "5000",
                DefenseCounselName = "Jane Smith"
            };
            var response = await client.PostAsJsonAsync("/api/v1/bondhearing", dto);
            response.EnsureSuccessStatusCode();
            Assert.Equal(
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                response.Content.Headers.ContentType?.MediaType);
            var bytes = await response.Content.ReadAsByteArrayAsync();
            Assert.True(bytes.Length > 0, "Response body should contain DOCX content.");
        }
    }
}
