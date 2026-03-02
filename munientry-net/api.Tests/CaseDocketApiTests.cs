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
    public class CaseDocketApiTests : IClassFixture<MuniEntryWebApplicationFactory>
    {
        private readonly MuniEntryWebApplicationFactory _factory;

        public CaseDocketApiTests(MuniEntryWebApplicationFactory factory)
        {
            _factory = factory;
        }

        // --- Happy path: case with docket entries ---

        [Fact]
        public async Task GetCaseDocket_KnownCase_Returns200WithEntries()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync($"/api/v1/case/docket/{FakeCaseDocketService.KnownCaseNumber}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var results = await response.Content.ReadFromJsonAsync<List<CaseDocketEntryDto>>();
            Assert.NotNull(results);
            Assert.Equal(3, results.Count);
        }

        [Fact]
        public async Task GetCaseDocket_KnownCase_ReturnsRemarkText()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync($"/api/v1/case/docket/{FakeCaseDocketService.KnownCaseNumber}");
            var results = await response.Content.ReadFromJsonAsync<List<CaseDocketEntryDto>>();

            Assert.NotNull(results);
            Assert.Contains(results, r => r.Remark != null && r.Remark.Contains("Not Guilty Plea"));
            Assert.Contains(results, r => r.Remark != null && r.Remark.Contains("Final Pretrial"));
        }

        [Fact]
        public async Task GetCaseDocket_KnownCase_ReturnsDates()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync($"/api/v1/case/docket/{FakeCaseDocketService.KnownCaseNumber}");
            var results = await response.Content.ReadFromJsonAsync<List<CaseDocketEntryDto>>();

            Assert.NotNull(results);
            Assert.All(results, r => Assert.NotNull(r.Date));
        }

        // --- Unknown case → 404 ---

        [Fact]
        public async Task GetCaseDocket_UnknownCase_Returns404()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync($"/api/v1/case/docket/{FakeCaseDocketService.UnknownCaseNumber}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
