using System.Net;
using System.Net.Http.Json;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;

public class FineOnlyApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    public FineOnlyApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Get_FineOnly_ReturnsSample()
    {
        var client = _factory.CreateClient();
        var resp = await client.GetAsync("/api/fineonly/CASE123");
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        var dto = await resp.Content.ReadFromJsonAsync<FineOnlyDto>();
        Assert.NotNull(dto);
        Assert.Equal("CASE123", dto!.CaseNumber);
    }

    [Fact]
    public async Task Post_FineOnly_ReturnsDocx()
    {
        var client = _factory.CreateClient();
        var dto = new FineOnlyDto { CaseNumber = "CASE999", DefendantName = "Test User", Charge = "Speeding", FineAmount = 100 };
        var resp = await client.PostAsJsonAsync("/api/fineonly", dto);
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        Assert.Equal("application/vnd.openxmlformats-officedocument.wordprocessingml.document", resp.Content.Headers.ContentType!.MediaType);
        var bytes = await resp.Content.ReadAsByteArrayAsync();
        Assert.True(bytes.Length > 1000); // Should be a real docx file
    }
}

public class FineOnlyDto
{
    public string? CaseNumber { get; set; }
    public string? DefendantName { get; set; }
    public string? Charge { get; set; }
    public decimal? FineAmount { get; set; }
}
