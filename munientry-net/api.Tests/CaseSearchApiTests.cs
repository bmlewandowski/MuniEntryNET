using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Munientry.Shared.Dtos;
using Munientry.Api.Tests.Fakes;
using Munientry.Api.Tests.Infrastructure;
using Xunit;

namespace Munientry.Api.Tests
{
    public class CaseSearchApiTests : IClassFixture<MuniEntryWebApplicationFactory>
    {
        private readonly MuniEntryWebApplicationFactory _factory;

        public CaseSearchApiTests(MuniEntryWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetCaseSearch_KnownCase_Returns200WithChargeList()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync($"/api/v1/case/search/{FakeCaseSearchService.KnownCaseNumber}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var results = await response.Content.ReadFromJsonAsync<List<CaseSearchResultDto>>();
            Assert.NotNull(results);
            Assert.Single(results);
            Assert.Equal(FakeCaseSearchService.KnownCaseNumber, results[0].CaseNumber);
            Assert.Equal("OVI", results[0].Charge);
        }

        [Fact]
        public async Task GetCaseSearch_UnknownCase_Returns404()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync($"/api/v1/case/search/{FakeCaseSearchService.UnknownCaseNumber}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetCaseSearch_KnownCase_ReturnsDefendantName()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync($"/api/v1/case/search/{FakeCaseSearchService.KnownCaseNumber}");
            var results = await response.Content.ReadFromJsonAsync<List<CaseSearchResultDto>>();

            Assert.NotNull(results);
            var row = results[0];
            Assert.Equal("Jane", row.DefFirstName);
            Assert.Equal("Doe", row.DefLastName);
        }

        [Fact]
        public async Task GetCaseSearch_KnownCase_ReturnsCorrectChargeDetails()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync($"/api/v1/case/search/{FakeCaseSearchService.KnownCaseNumber}");
            var results = await response.Content.ReadFromJsonAsync<List<CaseSearchResultDto>>();

            Assert.NotNull(results);
            var row = results[0];
            Assert.Equal("4511.19", row.Statute);
            Assert.Equal("M1", row.DegreeCode);
            Assert.True(row.MovingBool);
        }
    }
}
