using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Munientry.Shared.Dtos;
using Munientry.Api.Tests.Infrastructure;
using Xunit;

namespace Munientry.Api.Tests
{
    public class NotGuiltyReportApiTests : IClassFixture<MuniEntryWebApplicationFactory>
    {
        private readonly MuniEntryWebApplicationFactory _factory;
        private const string DateWithData   = "2026-02-28";
        private const string DateWithNoData = "2026-01-01";

        public NotGuiltyReportApiTests(MuniEntryWebApplicationFactory factory)
        {
            _factory = factory;
        }

        // --- Happy path: date with matching cases ---

        [Fact]
        public async Task GetNotGuiltyReport_DateWithData_Returns200WithList()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync($"/api/v1/reports/not-guilty/{DateWithData}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var results = await response.Content.ReadFromJsonAsync<List<NotGuiltyReportResultDto>>();
            Assert.NotNull(results);
            Assert.Equal(2, results.Count);
        }

        [Fact]
        public async Task GetNotGuiltyReport_DateWithData_ReturnsCaseNumbers()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync($"/api/v1/reports/not-guilty/{DateWithData}");
            var results = await response.Content.ReadFromJsonAsync<List<NotGuiltyReportResultDto>>();

            Assert.NotNull(results);
            Assert.Contains(results, r => r.CaseNumber == "2026-CR-010");
            Assert.Contains(results, r => r.CaseNumber == "2026-CR-011");
        }

        [Fact]
        public async Task GetNotGuiltyReport_DateWithData_ReturnsRemarks()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync($"/api/v1/reports/not-guilty/{DateWithData}");
            var results = await response.Content.ReadFromJsonAsync<List<NotGuiltyReportResultDto>>();

            Assert.NotNull(results);
            Assert.All(results, r => Assert.NotNull(r.Remark));
        }

        [Fact]
        public async Task GetNotGuiltyReport_DateWithData_ReturnsDefendantNames()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync($"/api/v1/reports/not-guilty/{DateWithData}");
            var results = await response.Content.ReadFromJsonAsync<List<NotGuiltyReportResultDto>>();

            Assert.NotNull(results);
            Assert.Contains(results, r => r.DefFullName == "Smith, John");
        }

        // --- Date with no matching cases still returns 200 + empty list ---

        [Fact]
        public async Task GetNotGuiltyReport_DateWithNoData_Returns200EmptyList()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync($"/api/v1/reports/not-guilty/{DateWithNoData}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var results = await response.Content.ReadFromJsonAsync<List<NotGuiltyReportResultDto>>();
            Assert.NotNull(results);
            Assert.Empty(results);
        }

        // --- Invalid date format → 400, or 404 when the path segment is empty ---

        [Theory]
        [InlineData("notadate")]
        [InlineData("28-02-2026")]    // wrong format
        public async Task GetNotGuiltyReport_InvalidDate_Returns400(string badDate)
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync($"/api/v1/reports/not-guilty/{badDate}");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory]
        [InlineData("")]              // empty segment → route not matched → 404
        [InlineData("2026/02/28")]   // slashes split into multiple path segments → route not matched → 404
        public async Task GetNotGuiltyReport_UnroutablePath_Returns404(string badDate)
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync($"/api/v1/reports/not-guilty/{badDate}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
