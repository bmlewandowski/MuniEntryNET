using System.Net;
using System.Net.Http.Json;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Munientry.Poc.Api.Data;

public class TrialToCourtNoticeApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    public TrialToCourtNoticeApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Post_TrialToCourtNotice_SavesEntry()
    {
        var client = _factory.CreateClient();
        var dto = new TrialToCourtNoticeDto
        {
            CaseNumber = "22TRC12345",
            DefendantFirstName = "John",
            DefendantLastName = "Doe",
            EntryDate = DateTime.UtcNow,
            DefenseCounselName = "Smith",
            TrialToCourtDate = DateTime.UtcNow.AddDays(7),
            TrialToCourtTime = "10:00 AM",
            AssignedCourtroom = "A",
            InterpreterRequired = false,
            LanguageRequired = "",
            DateConfirmedWithCounsel = false
        };
        var resp = await client.PostAsJsonAsync("/api/trialtocourt", dto);
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }
}
