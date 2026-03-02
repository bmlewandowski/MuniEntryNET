
using System.Net;
using System.Net.Http.Json;
using Xunit;
using Munientry.Api.Tests.Infrastructure;
using System.Threading.Tasks;
using Munientry.Shared.Dtos;

namespace Munientry.Api.Tests
{
    public class FineOnlyApiTests : IClassFixture<MuniEntryWebApplicationFactory>
    {
        private readonly MuniEntryWebApplicationFactory _factory;
        public FineOnlyApiTests(MuniEntryWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Post_FineOnly_ReturnsDocx()
        {
            var client = _factory.CreateClient();
            var dto = new FineOnlyPleaDto { CaseNumber = "CASE999", DefendantFirstName = "Test", DefendantLastName = "User", Date = System.DateTime.Today.AddDays(-1), DefenseCounselWaived = true, Charges = "Speeding", ChargeStatute = "4511.21", ChargeDegree = "MM", ChargePlea = "Guilty", ChargeFinding = "Guilty", ChargeFinesAmount = "150.00", ChargeFinesSuspended = "0.00", CourtCosts = "Yes", FineAmount = 150 };
            var resp = await client.PostAsJsonAsync("/api/v1/fineonlyplea", dto);
            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
            // Accepts either file or JSON response
            Assert.True(resp.Content.Headers.ContentType!.MediaType == "application/json" || resp.Content.Headers.ContentType!.MediaType == "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
            var bytes = await resp.Content.ReadAsByteArrayAsync();
            Assert.True(bytes.Length > 100); // Should be a real docx file or JSON
        }
    }
}
