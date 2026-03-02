using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Munientry.Shared.Dtos;
using Munientry.Api.Tests.Fakes;
using Munientry.Api.Tests.Infrastructure;
using Xunit;

namespace Munientry.Api.Tests
{
    public class DrivingCaseApiTests : IClassFixture<MuniEntryWebApplicationFactory>
    {
        private readonly MuniEntryWebApplicationFactory _factory;

        public DrivingCaseApiTests(MuniEntryWebApplicationFactory factory)
        {
            _factory = factory;
        }

        // --- Happy path: known case found ---

        [Fact]
        public async Task GetDrivingCase_KnownCase_Returns200WithDto()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync($"/api/v1/drivingcase/{FakeDrivingCaseService.KnownCaseNumber}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<DrivingCaseInfoDto>();
            Assert.NotNull(result);
            Assert.Equal(FakeDrivingCaseService.KnownCaseNumber, result.CaseNumber);
        }

        [Fact]
        public async Task GetDrivingCase_KnownCase_ReturnsDefendantInfo()
        {
            var client = _factory.CreateClient();

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
            var client = _factory.CreateClient();

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
            var client = _factory.CreateClient();

            var response = await client.GetAsync($"/api/v1/drivingcase/{FakeDrivingCaseService.UnknownCaseNumber}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        // --- DTO shape: all expected fields present in response ---

        [Fact]
        public async Task GetDrivingCase_KnownCase_AllDtoFieldsPopulated()
        {
            var client = _factory.CreateClient();

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
