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
    internal class FakeDrivingCaseService : IDrivingCaseService
    {
        public const string KnownCaseNumber   = "2026-TRC-050";
        public const string UnknownCaseNumber = "9999-TRC-000";

        public Task<DrivingCaseInfoDto?> GetDrivingCaseInfoAsync(string caseNumber)
        {
            if (caseNumber == KnownCaseNumber)
            {
                return Task.FromResult<DrivingCaseInfoDto?>(new DrivingCaseInfoDto
                {
                    CaseNumber        = KnownCaseNumber,
                    DefLastName       = "Johnson",
                    DefFirstName      = "Robert",
                    DefMiddleName     = "Lee",
                    DefSuffix         = null,
                    DefBirthDate      = "1985-06-15",
                    DefCity           = "Columbus",
                    DefState          = "OH",
                    DefZipcode        = "43215",
                    DefAddress        = "123 Main St",
                    DefLicenseNumber  = "OH12345678",
                    CaseAddress       = "I-270 NB MM 35",
                    CaseCity          = "Columbus",
                    CaseState         = "OH",
                    CaseZipcode       = "43215",
                });
            }

            return Task.FromResult<DrivingCaseInfoDto?>(null);
        }
    }

    // ---------------------------------------------------------------------------
    // Tests
    // ---------------------------------------------------------------------------
    public class DrivingCaseApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public DrivingCaseApiTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        private HttpClient CreateClientWithFakeService() =>
            _factory.WithWebHostBuilder(builder =>
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(IDrivingCaseService));
                    if (descriptor != null)
                        services.Remove(descriptor);
                    services.AddScoped<IDrivingCaseService, FakeDrivingCaseService>();
                }))
            .CreateClient();

        // --- Happy path: known case found ---

        [Fact]
        public async Task GetDrivingCase_KnownCase_Returns200WithDto()
        {
            var client = CreateClientWithFakeService();

            var response = await client.GetAsync($"/api/v1/drivingcase/{FakeDrivingCaseService.KnownCaseNumber}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<DrivingCaseInfoDto>();
            Assert.NotNull(result);
            Assert.Equal(FakeDrivingCaseService.KnownCaseNumber, result.CaseNumber);
        }

        [Fact]
        public async Task GetDrivingCase_KnownCase_ReturnsDefendantInfo()
        {
            var client = CreateClientWithFakeService();

            var response = await client.GetAsync($"/api/v1/drivingcase/{FakeDrivingCaseService.KnownCaseNumber}");
            var result = await response.Content.ReadFromJsonAsync<DrivingCaseInfoDto>();

            Assert.NotNull(result);
            Assert.Equal("Johnson", result.DefLastName);
            Assert.Equal("Robert",  result.DefFirstName);
            Assert.Equal("OH",      result.DefState);
        }

        [Fact]
        public async Task GetDrivingCase_KnownCase_ReturnsLicenseAndAddress()
        {
            var client = CreateClientWithFakeService();

            var response = await client.GetAsync($"/api/v1/drivingcase/{FakeDrivingCaseService.KnownCaseNumber}");
            var result = await response.Content.ReadFromJsonAsync<DrivingCaseInfoDto>();

            Assert.NotNull(result);
            Assert.Equal("OH12345678",    result.DefLicenseNumber);
            Assert.Equal("123 Main St",   result.DefAddress);
            Assert.Equal("I-270 NB MM 35", result.CaseAddress);
        }

        // --- Not found: unknown case returns 404 ---

        [Fact]
        public async Task GetDrivingCase_UnknownCase_Returns404()
        {
            var client = CreateClientWithFakeService();

            var response = await client.GetAsync($"/api/v1/drivingcase/{FakeDrivingCaseService.UnknownCaseNumber}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        // --- DTO shape: all expected fields present in response ---

        [Fact]
        public async Task GetDrivingCase_KnownCase_AllDtoFieldsPopulated()
        {
            var client = CreateClientWithFakeService();

            var response = await client.GetAsync($"/api/v1/drivingcase/{FakeDrivingCaseService.KnownCaseNumber}");
            var result = await response.Content.ReadFromJsonAsync<DrivingCaseInfoDto>();

            Assert.NotNull(result);
            Assert.NotNull(result.CaseNumber);
            Assert.NotNull(result.DefFirstName);
            Assert.NotNull(result.DefLastName);
            Assert.NotNull(result.DefBirthDate);
            Assert.NotNull(result.DefLicenseNumber);
            Assert.NotNull(result.DefCity);
            Assert.NotNull(result.DefState);
            Assert.NotNull(result.DefZipcode);
            Assert.NotNull(result.DefAddress);
            Assert.NotNull(result.CaseAddress);
            Assert.NotNull(result.CaseCity);
            Assert.NotNull(result.CaseState);
            Assert.NotNull(result.CaseZipcode);
        }
    }
}
