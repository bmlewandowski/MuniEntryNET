using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using Munientry.Poc.Api.Data;

namespace Munientry.Api.Tests
{
    public class DrivingPrivilegesTests
    {
        private readonly Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory<Program> _factory = new();

        [Fact]
        public async Task DrivingPrivileges_Post_Works()
        {
            var client = _factory.CreateClient();
            var dto = new DrivingPrivilegesDto
            {
                DefendantFirstName = "Bob",
                DefendantLastName = "Johnson",
                CaseNumber = "2026-TR-00001",
                PleaTrialDate = System.DateTime.Today,
                AppearanceReason = "Hearing",
                DefenseCounselName = "Jane Smith",
                DefenseCounselType = "Private",
                DefenseCounselWaived = false,
                Offense = "Speeding",
                Statute = "S-101",
                Degree = "M1",
                Plea = "Guilty",
                Finding = "Guilty",
                Fines = 100.00m,
                FinesSuspended = 0.00m,
                CourtCosts = "Yes",
                AbilityToPay = "Yes",
                BalanceDueDate = System.DateTime.Today.AddDays(30),
                PayToday = 50.00m,
                MonthlyPay = 25.00m,
                CreditForJail = false,
                JailTimeCredit = 0,
                LicenseSuspension = false,
                CommunityService = false,
                OtherConditions = false
            };
            var response = await client.PostAsJsonAsync("/api/drivingprivileges", dto);
            response.EnsureSuccessStatusCode();
        }
    }
}
