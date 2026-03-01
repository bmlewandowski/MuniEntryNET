using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using Munientry.Api.Data;

namespace Munientry.Api.Tests
{
    public class NoticesFreeformCivilTests
    {
        private readonly Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory<Program> _factory = new();

        [Fact]
        public async Task NoticesFreeformCivil_Post_Works()
        {
            var client = _factory.CreateClient();
            var dto = new NoticesFreeformCivilDto
            {
                CaseNumber = "2026-CV-00001",
                EntryDate = System.DateTime.Today,
                DefendantFirstName = "Alice",
                DefendantLastName = "Smith",
                NoticeText = "This is a freeform civil notice."
            };
            var response = await client.PostAsJsonAsync("/api/v1/noticesfreeformcivil", dto);
            response.EnsureSuccessStatusCode();
        }
    }
}
