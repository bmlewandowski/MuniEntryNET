using System;
using System.Net;
using System.Net.Http.Json;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Threading.Tasks;
using Munientry.Poc.Api.Data;

public class JurorPaymentApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    public JurorPaymentApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Post_JurorPayment_SavesEntry()
    {
        var client = _factory.CreateClient();
        var dto = new JurorPaymentDto
        {
            CaseNumber = "22TRC88888",
            EntryDate = DateTime.UtcNow,
            DefendantFirstName = "Sam",
            DefendantLastName = "Williams",
            TrialDate = DateTime.UtcNow.AddDays(1),
            TrialLength = "One Day",
            JurorsReported = 10,
            JurorsSeated = 9,
            JurorsNotSeated = 1,
            JurorsPayNotSeated = 25,
            JurorsPaySeated = 40,
            JuryPanelTotalPay = 65
        };
        var resp = await client.PostAsJsonAsync("/api/jurorpayment", dto);
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }
}
