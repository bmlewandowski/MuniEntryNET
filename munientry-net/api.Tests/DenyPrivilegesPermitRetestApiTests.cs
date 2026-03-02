using System;
using System.Net;
using System.Net.Http.Json;
using Xunit;
using Munientry.Api.Tests.Infrastructure;
using System.Threading.Tasks;
using Munientry.Shared.Dtos;

public class DenyPrivilegesPermitRetestApiTests : IClassFixture<MuniEntryWebApplicationFactory>
{
    private readonly MuniEntryWebApplicationFactory _factory;
    public DenyPrivilegesPermitRetestApiTests(MuniEntryWebApplicationFactory factory)
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
            EntryType = EntryType.DenyDrivingPrivileges,
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
        var resp = await client.PostAsJsonAsync("/api/v1/denyprivilegespermitretest", dto);
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }
}
