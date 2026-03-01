using System;
using System.Net;
using System.Net.Http.Json;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Threading.Tasks;
using Munientry.Api.Data;

public class FinalJuryNoticeApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    public FinalJuryNoticeApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Post_FinalJuryNotice_SavesEntry()
    {
        var client = _factory.CreateClient();
        var dto = new FinalJuryNoticeDto
        {
            CaseNumber = "22TRC54321",
            DefendantFirstName = "Jane",
            DefendantLastName = "Smith",
            EntryDate = DateTime.UtcNow,
            DefenseCounselName = "Johnson",
            FinalJuryDate = DateTime.UtcNow.AddDays(14),
            FinalJuryTime = "2:00 PM",
            AssignedCourtroom = "B",
            InterpreterRequired = false,
            LanguageRequired = "",
            DateConfirmedWithCounsel = false
        };
        var resp = await client.PostAsJsonAsync("/api/v1/finaljury", dto);
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }
}
