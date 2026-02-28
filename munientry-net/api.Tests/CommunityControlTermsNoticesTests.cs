using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using Munientry.Poc.Api.Data;
namespace Munientry.Api.Tests
{
    public class CommunityControlTermsNoticesTests
    {
        private readonly Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory<Program> _factory = new();

        [Fact]
        public async Task CommunityControlTermsNotices_Post_Works()
        {
            var client = _factory.CreateClient();
            var dto = new CommunityControlTermsNoticesDto
            {
                CaseNumber = "2026-CR-00003",
                EntryDate = System.DateTime.Today,
                DefendantFirstName = "Bob",
                DefendantLastName = "Jones",
                HearingTime = "9:00 AM",
                AssignedCourtroom = "A",
                DefenseCounselName = "Attorney Smith",
                ViolationType = "Probation Violation",
                LanguageRequired = "None"
            };
            var response = await client.PostAsJsonAsync("/api/communitycontroltermsnotices", dto);
            response.EnsureSuccessStatusCode();
        }
    }
}
