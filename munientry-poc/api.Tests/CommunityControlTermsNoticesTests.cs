using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using Munientry.Poc.Api.Data;

namespace Munientry.Api.Tests
{
    public class CommunityControlTermsNoticesTests
    {
        [Fact]
        public async Task CommunityControlTermsNotices_Post_Works()
        {
            using var app = new TestApp();
            var client = app.CreateClient();
            var dto = new CommunityControlTermsNoticesDto
            {
                CaseNumber = "2024-CR-12345",
                EntryDate = System.DateTime.Today,
                DefendantFirstName = "John",
                DefendantLastName = "Doe",
                HearingDate = System.DateTime.Today.AddDays(7),
                HearingTime = "10:00 AM",
                AssignedCourtroom = "Courtroom 1",
                DefenseCounselName = "Jane Smith",
                ViolationType = "Technical",
                InterpreterRequired = false,
                LanguageRequired = ""
            };
            var response = await client.PostAsJsonAsync("/api/communitycontroltermsnotices", dto);
            response.EnsureSuccessStatusCode();
        }
    }
}
