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
    internal class FakeCaseSearchService : ICaseSearchService
    {
        public const string KnownCaseNumber    = "2026-CR-001";
        public const string UnknownCaseNumber  = "9999-XX-999";

        public Task<List<CaseSearchResultDto>> SearchCaseAsync(string caseNumber)
        {
            if (caseNumber == KnownCaseNumber)
            {
                return Task.FromResult(new List<CaseSearchResultDto>
                {
                    new()
                    {
                        CaseNumber       = KnownCaseNumber,
                        SubCaseNumber    = "2026-CR-001-A",
                        SubCaseId        = 1,
                        Charge           = "OVI",
                        ViolationId      = 10,
                        ViolationDetailId = 20,
                        Statute          = "4511.19",
                        DegreeCode       = "M1",
                        DefFirstName     = "Jane",
                        DefLastName      = "Doe",
                        MovingBool       = true,
                        ViolationDate    = new DateTime(2026, 1, 15),
                        FraInFile        = "N/A",
                        DefenseCounsel   = "Smith, J.",
                        PubDef           = false,
                    }
                });
            }

            return Task.FromResult(new List<CaseSearchResultDto>());
        }
    }

    // ---------------------------------------------------------------------------
    // Tests
    // ---------------------------------------------------------------------------
    public class CaseSearchApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public CaseSearchApiTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        private HttpClient CreateClientWithFakeService() =>
            _factory.WithWebHostBuilder(builder =>
                builder.ConfigureServices(services =>
                {
                    // Remove the real ICaseSearchService and replace with the fake
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(ICaseSearchService));
                    if (descriptor != null)
                        services.Remove(descriptor);
                    services.AddScoped<ICaseSearchService, FakeCaseSearchService>();
                }))
            .CreateClient();

        [Fact]
        public async Task GetCaseSearch_KnownCase_Returns200WithChargeList()
        {
            var client = CreateClientWithFakeService();

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
            var client = CreateClientWithFakeService();

            var response = await client.GetAsync($"/api/v1/case/search/{FakeCaseSearchService.UnknownCaseNumber}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetCaseSearch_KnownCase_ReturnsDefendantName()
        {
            var client = CreateClientWithFakeService();

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
            var client = CreateClientWithFakeService();

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
