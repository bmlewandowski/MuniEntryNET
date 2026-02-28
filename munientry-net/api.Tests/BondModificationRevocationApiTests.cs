using System.Net.Http.Json;
using System.Threading.Tasks;
using Munientry.Poc.Api.Data;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Munientry.Poc.Api.Tests
{
    public class BondModificationRevocationApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public BondModificationRevocationApiTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task PostBondModificationRevocation_ReturnsDocx()
        {
            var client = _factory.CreateClient();
            var dto = new BondModificationRevocationDto
            {
                DefendantFirstName = "John",
                DefendantLastName = "Doe",
                CaseNumber = "2026-CR-002",
                EntryDate = System.DateTime.Today,
                DecisionOnBond = "Modify",
                BondType = "Surety",
                BondAmount = "2500",
                DefenseCounselName = "Jane Smith"
            };
            var response = await client.PostAsJsonAsync("/api/bondmodificationrevocation", dto);
            response.EnsureSuccessStatusCode();
            Assert.Equal(
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                response.Content.Headers.ContentType?.MediaType);
            var bytes = await response.Content.ReadAsByteArrayAsync();
            Assert.True(bytes.Length > 0, "Response body should contain DOCX content.");
        }
    }
}
