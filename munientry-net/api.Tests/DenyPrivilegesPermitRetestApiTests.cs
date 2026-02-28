using System;
using System.Net;
using System.Net.Http.Json;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Threading.Tasks;
using Munientry.Poc.Api.Data;

public class DenyPrivilegesPermitRetestApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    public DenyPrivilegesPermitRetestApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Post_DenyPrivilegesPermitRetest_SavesEntry()
    {
        var client = _factory.CreateClient();
        var dto = new DenyPrivilegesPermitRetestDto
        {
            DefendantFirstName = "Alex",
            DefendantLastName = "Smith",
            CaseNumber = "22TRC99999",
            EntryDate = DateTime.UtcNow,
            EntryType = "DenyDrivingPrivileges",
            HardTimeNotPassed = true,
            PermanentIdCard = false,
            OutOfStateLicense = false,
            PetitionIncomplete = false,
            NoInsurance = true,
            NoEmployerInfo = false,
            NoJurisdiction = false,
            NoPayPlan = false,
            ProhibitedActivities = false,
            LicenseExpirationDate = DateTime.UtcNow.AddYears(1),
            PrivilegesGrantDate = DateTime.UtcNow.AddDays(-30),
            NufcDate = DateTime.UtcNow.AddDays(-10)
        };
        var resp = await client.PostAsJsonAsync("/api/denyprivilegespermitretest", dto);
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }
}
