
using System.Net;
using System.Net.Http.Json;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Threading.Tasks;
using Munientry.Api.Data;

namespace Munientry.Api.Tests
{
    public class FineOnlyApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        public FineOnlyApiTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Get_FineOnly_ReturnsSample()
        {
            var client = _factory.CreateClient();
            var resp = await client.GetAsync("/api/v1/fineonly/CASE123");
            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
            var dto = await resp.Content.ReadFromJsonAsync<FineOnlyPleaDto>();
            Assert.NotNull(dto);
            Assert.Equal("CASE123", dto!.CaseNumber);
        }

        [Fact]
        public async Task Post_FineOnly_ReturnsDocx()
        {
            var client = _factory.CreateClient();
            var dto = new FineOnlyPleaDto { CaseNumber = "CASE999", DefendantFirstName = "Test", DefendantLastName = "User", Charges = "Speeding", ChargeStatute = "4511.21", ChargeDegree = "MM", ChargePlea = "Guilty", ChargeFinding = "Guilty", ChargeFinesAmount = "150.00", ChargeFinesSuspended = "0.00", CourtCosts = "Yes", FineAmount = 150 };
            var resp = await client.PostAsJsonAsync("/api/v1/fineonlyplea", dto);
            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
            // Accepts either file or JSON response
            Assert.True(resp.Content.Headers.ContentType!.MediaType == "application/json" || resp.Content.Headers.ContentType!.MediaType == "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
            var bytes = await resp.Content.ReadAsByteArrayAsync();
            Assert.True(bytes.Length > 100); // Should be a real docx file or JSON
        }
    }
}
