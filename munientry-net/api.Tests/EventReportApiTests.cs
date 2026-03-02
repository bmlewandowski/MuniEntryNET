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
    public class EventReportApiTests : IClassFixture<MuniEntryWebApplicationFactory>
    {
        private readonly MuniEntryWebApplicationFactory _factory;
        private const string ValidDate = "2026-02-28";

        public EventReportApiTests(MuniEntryWebApplicationFactory factory)
        {
            _factory = factory;
        }

        // --- Happy path: known event code + date with data ---

        [Fact]
        public async Task GetEventReport_KnownCodeAndDate_Returns200WithList()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync(
                $"/api/v1/reports/events/{FakeEventReportService.KnownEventCode}/{ValidDate}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var results = await response.Content.ReadFromJsonAsync<List<EventReportResultDto>>();
            Assert.NotNull(results);
            Assert.Equal(2, results.Count);
        }

        [Fact]
        public async Task GetEventReport_KnownCodeAndDate_ReturnsCaseNumbers()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync(
                $"/api/v1/reports/events/{FakeEventReportService.KnownEventCode}/{ValidDate}");
            var results = await response.Content.ReadFromJsonAsync<List<EventReportResultDto>>();

            Assert.NotNull(results);
            Assert.Contains(results, r => r.CaseNumber == "2026-CR-010");
            Assert.Contains(results, r => r.CaseNumber == "2026-CR-015");
        }

        [Fact]
        public async Task GetEventReport_KnownCodeAndDate_ReturnsTimeField()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync(
                $"/api/v1/reports/events/{FakeEventReportService.KnownEventCode}/{ValidDate}");
            var results = await response.Content.ReadFromJsonAsync<List<EventReportResultDto>>();

            Assert.NotNull(results);
            Assert.Contains(results, r => r.Time == "09:00 AM");
        }

        [Fact]
        public async Task GetEventReport_KnownCodeAndDate_ReturnsChargeAndCounsel()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync(
                $"/api/v1/reports/events/{FakeEventReportService.KnownEventCode}/{ValidDate}");
            var results = await response.Content.ReadFromJsonAsync<List<EventReportResultDto>>();

            Assert.NotNull(results);
            var first = results[0];
            Assert.Equal("OVI", first.Charge);
            Assert.Equal("Jones, Mary", first.DefenseCounsel);
        }

        // --- Event code with no data → 200 + empty list ---

        [Fact]
        public async Task GetEventReport_EventCodeWithNoData_Returns200EmptyList()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync(
                $"/api/v1/reports/events/{FakeEventReportService.EmptyEventCode}/{ValidDate}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var results = await response.Content.ReadFromJsonAsync<List<EventReportResultDto>>();
            Assert.NotNull(results);
            Assert.Empty(results);
        }

        // --- Invalid date format → 400 ---

        [Theory]
        [InlineData("notadate")]
        [InlineData("28-02-2026")]
        public async Task GetEventReport_InvalidDate_Returns400(string badDate)
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync(
                $"/api/v1/reports/events/{FakeEventReportService.KnownEventCode}/{badDate}");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
