using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using Munientry.Poc.Api.Data;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Munientry.Poc.Api.Tests
{
    public class CommunityControlTermsApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        public CommunityControlTermsApiTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task PostCommunityControlTerms_ReturnsDocx()
        {
            var client = _factory.CreateClient();
            var dto = new CommunityControlTermsDto
            {
                DefendantFirstName = "Test",
                DefendantLastName = "User",
                CaseNumber = "CASE456",
                EntryDate = System.DateTime.Today,
                TermOfControl = "1 year",
                ReportFrequency = "daily (Monday through Friday)",
                ReportToJail = false,
                JailReportDate = null,
                JailReportTime = null,
                JailDaysToServe = null,
                InterlockVehicles = false,
                NoAlcoholOrdered = false,
                GpsExclusion = false,
                ScramOrdered = false,
                ScramDays = null,
                NoContact = false,
                NoContactWith = null,
                Antitheft = false,
                AlcoholTreatment = false,
                DomesticViolenceProgram = false,
                AngerManagement = false,
                MentalHealthTreatment = false,
                DriverInterventionProgram = false,
                CommunityService = false,
                CommunityServiceHours = null,
                PayRestitution = false,
                SpecializedDocket = false,
                SpecializedDocketType = null
            };
            var response = await client.PostAsJsonAsync("/api/communitycontrolterms", dto);
            response.EnsureSuccessStatusCode();
            Assert.Equal(
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                response.Content.Headers.ContentType?.MediaType);
            var bytes = await response.Content.ReadAsByteArrayAsync();
            Assert.True(bytes.Length > 0, "Response body should contain DOCX content.");
        }
    }
}
