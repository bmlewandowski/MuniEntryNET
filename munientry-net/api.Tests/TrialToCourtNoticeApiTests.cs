using System;
using System.Net;
using System.Net.Http.Json;
using Xunit;
using Munientry.Api.Tests.Infrastructure;
using System.Threading.Tasks;
using Munientry.Shared.Dtos;

public class TrialToCourtNoticeApiTests : IClassFixture<MuniEntryWebApplicationFactory>
{
    private readonly MuniEntryWebApplicationFactory _factory;
    public TrialToCourtNoticeApiTests(MuniEntryWebApplicationFactory factory)
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
        var resp = await client.PostAsJsonAsync("/api/v1/trialtocourt", dto);
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }
}
