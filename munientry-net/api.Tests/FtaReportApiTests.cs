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
    public class FtaReportApiTests : IClassFixture<MuniEntryWebApplicationFactory>
    {
        private readonly MuniEntryWebApplicationFactory _factory;
        private const string DateWithData   = "2026-02-28";
        private const string DateWithNoData = "2026-01-01";

        public FtaReportApiTests(MuniEntryWebApplicationFactory factory)
        {
            _factory = factory;
        }

        // ── GET /reports/fta/{date} — case list ──────────────────────────────

        [Fact]
        public async Task GetFtaReport_DateWithData_Returns200WithCaseList()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync($"/api/v1/reports/fta/{DateWithData}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var results = await response.Content.ReadFromJsonAsync<List<FtaReportResultDto>>();
            Assert.NotNull(results);
            Assert.Equal(2, results.Count);
        }

        [Fact]
        public async Task GetFtaReport_DateWithData_ReturnsCaseNumbers()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync($"/api/v1/reports/fta/{DateWithData}");
            var results = await response.Content.ReadFromJsonAsync<List<FtaReportResultDto>>();

            Assert.NotNull(results);
            Assert.Contains(results, r => r.CaseNumber == FakeFtaReportService.CrbCase);
            Assert.Contains(results, r => r.CaseNumber == FakeFtaReportService.TrcCase);
        }

        [Fact]
        public async Task GetFtaReport_DateWithNoData_Returns200EmptyList()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync($"/api/v1/reports/fta/{DateWithNoData}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var results = await response.Content.ReadFromJsonAsync<List<FtaReportResultDto>>();
            Assert.NotNull(results);
            Assert.Empty(results);
        }

        [Theory]
        [InlineData("notadate")]
        [InlineData("28-02-2026")]
        public async Task GetFtaReport_InvalidDate_Returns400(string badDate)
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync($"/api/v1/reports/fta/{badDate}");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        // ── GET /reports/fta/entry/{caseNumber}/{date} — single DOCX ─────────

        [Fact]
        public async Task GetFtaEntry_ValidCaseAndDate_Returns200WithDocx()
        {
            var client = _factory.CreateClient();
            var url = $"/api/v1/reports/fta/entry/{FakeFtaReportService.CrbCase}/{DateWithData}";

            var response = await client.GetAsync(url);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                response.Content.Headers.ContentType?.MediaType);
        }

        [Fact]
        public async Task GetFtaEntry_ValidCaseAndDate_ReturnsBytesWithDocxMagicNumber()
        {
            var client = _factory.CreateClient();
            var url = $"/api/v1/reports/fta/entry/{FakeFtaReportService.TrcCase}/{DateWithData}";

            var response = await client.GetAsync(url);
            var bytes = await response.Content.ReadAsByteArrayAsync();

            // DOCX files are ZIP archives; magic bytes are PK (0x50 0x4B)
            Assert.NotEmpty(bytes);
            Assert.Equal(0x50, bytes[0]);
            Assert.Equal(0x4B, bytes[1]);
        }

        [Fact]
        public async Task GetFtaEntry_InvalidDate_Returns400()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync(
                $"/api/v1/reports/fta/entry/{FakeFtaReportService.CrbCase}/notadate");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        // ── GET /reports/fta/batch/{date} — ZIP of all DOCXs ─────────────────

        [Fact]
        public async Task GetFtaBatch_DateWithData_Returns200WithZip()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync($"/api/v1/reports/fta/batch/{DateWithData}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("application/zip", response.Content.Headers.ContentType?.MediaType);
        }

        [Fact]
        public async Task GetFtaBatch_DateWithData_ReturnsBytesWithZipMagicNumber()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync($"/api/v1/reports/fta/batch/{DateWithData}");
            var bytes = await response.Content.ReadAsByteArrayAsync();

            // ZIP archives start with PK (0x50 0x4B)
            Assert.NotEmpty(bytes);
            Assert.Equal(0x50, bytes[0]);
            Assert.Equal(0x4B, bytes[1]);
        }

        [Fact]
        public async Task GetFtaBatch_DateWithNoData_Returns404()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync($"/api/v1/reports/fta/batch/{DateWithNoData}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetFtaBatch_InvalidDate_Returns400()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/api/v1/reports/fta/batch/notadate");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
