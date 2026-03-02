using System;
using System.Net;
using System.Net.Http.Json;
using Xunit;
using Munientry.Api.Tests.Infrastructure;
using System.Threading.Tasks;
using Munientry.Shared.Dtos;

public class GeneralNoticeOfHearingApiTests : IClassFixture<MuniEntryWebApplicationFactory>
{
    private readonly MuniEntryWebApplicationFactory _factory;
    public GeneralNoticeOfHearingApiTests(MuniEntryWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Post_GeneralNoticeOfHearing_SavesEntry()
    {
        var client = _factory.CreateClient();
        var dto = new GeneralNoticeOfHearingDto
        {
            CaseNumber = "22TRC77777",
            EntryDate = DateTime.UtcNow,
            DefendantFirstName = "Chris",
            DefendantLastName = "Lee",
            HearingDate = DateTime.UtcNow.AddDays(3),
            HearingTime = "9:00 AM",
            AssignedCourtroom = "C",
            DefenseCounselName = "Taylor",
            InterpreterRequired = false,
            LanguageRequired = ""
        };
        var resp = await client.PostAsJsonAsync("/api/v1/generalnoticeofhearing", dto);
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }
}
