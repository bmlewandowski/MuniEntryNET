using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Munientry.Api.Data;
using Munientry.Api.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Munientry.Api.Tests
{
    // ---------------------------------------------------------------------------
    // Fake service that returns controlled test data — no SQL Server needed
    // ---------------------------------------------------------------------------
    internal class FakeDailyListService : IDailyListService
    {
        public const string ListTypeWithData  = "arraignments";
        public const string ListTypeWithData2 = "pleas";
        public static readonly string[] AllValidTypes =
            { "arraignments", "slated", "pleas", "pcvh_fcvh", "final_pretrial", "trials_to_court" };

        public Task<List<DailyListResultDto>> GetDailyListAsync(string listType, DateTime reportDate)
        {
            // Mirror the real service: throw on unknown list type
            var procName = DailyListStoredProcs.GetProcName(listType);
            if (procName is null)
                throw new ArgumentException(
                    $"Unknown list type '{listType}'. Valid values: {string.Join(", ", DailyListStoredProcs.ValidTypes)}");

            if (listType.Equals(ListTypeWithData, StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(new List<DailyListResultDto>
                {
                    new()
                    {
                        Time           = "09:00 AM",
                        CaseNumber     = "2026-CR-010",
                        DefFullName    = "Smith, John",
                        SubCaseNumber  = "2026-CR-010-A",
                        Charge         = "OVI",
                        EventId        = "EVT-001",
                        JudgeId        = "JUDGE-01",
                        DefenseCounsel = "Jones, Mary",
                    },
                    new()
                    {
                        Time           = "09:30 AM",
                        CaseNumber     = "2026-CR-011",
                        DefFullName    = "Brown, Alice",
                        SubCaseNumber  = "2026-CR-011-A",
                        Charge         = "Reckless Driving",
                        EventId        = "EVT-002",
                        JudgeId        = "JUDGE-01",
                        DefenseCounsel = null,
                    },
                });
            }

            // Other valid list types return an empty list (no cases scheduled)
            return Task.FromResult(new List<DailyListResultDto>());
        }
    }

    // ---------------------------------------------------------------------------
    // Tests
    // ---------------------------------------------------------------------------
    public class DailyListApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private const string ValidDate = "2026-02-28";

        public DailyListApiTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        private HttpClient CreateClientWithFakeService() =>
            _factory.WithWebHostBuilder(builder =>
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(IDailyListService));
                    if (descriptor != null)
                        services.Remove(descriptor);
                    services.AddScoped<IDailyListService, FakeDailyListService>();
                }))
            .CreateClient();

        // --- Happy path: valid list type with cases ---

        [Fact]
        public async Task GetDailyList_ValidTypeWithCases_Returns200WithList()
        {
            var client = CreateClientWithFakeService();

            var response = await client.GetAsync($"/api/v1/dailylist/{FakeDailyListService.ListTypeWithData}/{ValidDate}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var results = await response.Content.ReadFromJsonAsync<List<DailyListResultDto>>();
            Assert.NotNull(results);
            Assert.Equal(2, results.Count);
        }

        [Fact]
        public async Task GetDailyList_ValidTypeWithCases_ReturnsCaseNumbers()
        {
            var client = CreateClientWithFakeService();

            var response = await client.GetAsync($"/api/v1/dailylist/{FakeDailyListService.ListTypeWithData}/{ValidDate}");
            var results = await response.Content.ReadFromJsonAsync<List<DailyListResultDto>>();

            Assert.NotNull(results);
            Assert.Contains(results, r => r.CaseNumber == "2026-CR-010");
            Assert.Contains(results, r => r.CaseNumber == "2026-CR-011");
        }

        // --- Happy path: valid list type with no cases (empty list) ---

        [Fact]
        public async Task GetDailyList_ValidTypeNoData_Returns200WithEmptyList()
        {
            var client = CreateClientWithFakeService();

            // "trials_to_court" returns empty in the fake; confirms endpoint still returns 200
            var response = await client.GetAsync($"/api/v1/dailylist/trials_to_court/{ValidDate}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var results = await response.Content.ReadFromJsonAsync<List<DailyListResultDto>>();
            Assert.NotNull(results);
            Assert.Empty(results);
        }

        // --- All six valid list types accepted ---

        [Theory]
        [InlineData("arraignments")]
        [InlineData("slated")]
        [InlineData("pleas")]
        [InlineData("pcvh_fcvh")]
        [InlineData("final_pretrial")]
        [InlineData("trials_to_court")]
        public async Task GetDailyList_AllValidListTypes_Return200(string listType)
        {
            var client = CreateClientWithFakeService();

            var response = await client.GetAsync($"/api/v1/dailylist/{listType}/{ValidDate}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        // --- List type case-insensitivity ---

        [Fact]
        public async Task GetDailyList_ListTypeUpperCase_Returns200()
        {
            var client = CreateClientWithFakeService();

            var response = await client.GetAsync($"/api/v1/dailylist/ARRAIGNMENTS/{ValidDate}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        // --- Error path: unknown list type ---

        [Fact]
        public async Task GetDailyList_UnknownListType_Returns400()
        {
            var client = CreateClientWithFakeService();

            var response = await client.GetAsync($"/api/v1/dailylist/bogus_type/{ValidDate}");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        // --- Error path: malformed date ---

        [Theory]
        [InlineData("not-a-date")]
        [InlineData("20260228")]      // no separators
        [InlineData("yesterday")]
        public async Task GetDailyList_InvalidDate_Returns400(string badDate)
        {
            var client = CreateClientWithFakeService();

            var response = await client.GetAsync($"/api/v1/dailylist/arraignments/{badDate}");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        // "28/02/2026" contains slashes so the router treats it as extra path segments
        // (no route match) ? 404. Confirms the endpoint is unreachable with that format.
        [Fact]
        public async Task GetDailyList_SlashSeparatedDate_IsNotAccessible()
        {
            var client = CreateClientWithFakeService();

            var response = await client.GetAsync("/api/v1/dailylist/arraignments/28/02/2026");

            Assert.NotEqual(HttpStatusCode.OK, response.StatusCode);
        }

        // --- DTO shape: all expected fields present ---

        [Fact]
        public async Task GetDailyList_ValidTypeWithCases_DtoFieldsPopulated()
        {
            var client = CreateClientWithFakeService();

            var response = await client.GetAsync($"/api/v1/dailylist/{FakeDailyListService.ListTypeWithData}/{ValidDate}");
            var results = await response.Content.ReadFromJsonAsync<List<DailyListResultDto>>();

            Assert.NotNull(results);
            var row = results[0];
            Assert.NotNull(row.Time);
            Assert.NotNull(row.DefFullName);
            Assert.NotNull(row.Charge);
            Assert.NotNull(row.EventId);
            Assert.NotNull(row.JudgeId);
        }
    }
}
