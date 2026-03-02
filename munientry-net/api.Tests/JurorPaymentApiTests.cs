using System;
using System.Net;
using System.Net.Http.Json;
using Xunit;
using Munientry.Api.Tests.Infrastructure;
using System.Threading.Tasks;
using Munientry.Shared.Dtos;

public class JurorPaymentApiTests : IClassFixture<MuniEntryWebApplicationFactory>
{
    private readonly MuniEntryWebApplicationFactory _factory;
    public JurorPaymentApiTests(MuniEntryWebApplicationFactory factory)
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
        var resp = await client.PostAsJsonAsync("/api/v1/jurorpayment", dto);
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }
}
