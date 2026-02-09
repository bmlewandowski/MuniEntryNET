using System;
using System.Net;
using System.Net.Http.Json;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Threading.Tasks;
using Munientry.Poc.Api.Data;

public class TimeToPayOrderApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    public TimeToPayOrderApiTests(WebApplicationFactory<Program> factory)
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
        var resp = await client.PostAsJsonAsync("/api/timetopayorder", dto);
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }
}
