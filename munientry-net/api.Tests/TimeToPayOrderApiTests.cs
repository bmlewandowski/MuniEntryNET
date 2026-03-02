using System;
using System.Net;
using System.Net.Http.Json;
using Xunit;
using Munientry.Api.Tests.Infrastructure;
using System.Threading.Tasks;
using Munientry.Shared.Dtos;

public class TimeToPayOrderApiTests : IClassFixture<MuniEntryWebApplicationFactory>
{
    private readonly MuniEntryWebApplicationFactory _factory;
    public TimeToPayOrderApiTests(MuniEntryWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Post_TimeToPayOrder_SavesEntry()
    {
        var client = _factory.CreateClient();
        var dto = new TimeToPayOrderDto
        {
            CaseNumber = "22TRC99999",
            EntryDate = DateTime.UtcNow,
            DefendantFirstName = "Alex",
            DefendantLastName = "Johnson",
            AppearanceDate = DateTime.UtcNow.AddDays(14)
        };
        var resp = await client.PostAsJsonAsync("/api/v1/timetopayorder", dto);
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }
}
