using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using Munientry.Poc.Client.Shared.Models;

namespace api.Tests
{
    public class AppearOnWarrantNoPleaApiTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public AppearOnWarrantNoPleaApiTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Post_AppearOnWarrantNoPlea_ReturnsOk()
        {
            var dto = new AppearOnWarrantNoPleaDto
            {
                DefendantFirstName = "John",
                DefendantLastName = "Doe",
                CaseNumber = "2026-CR-0001",
                BondType = "Cash",
                BondAmount = "1000",
                NoContactName = "Jane Doe",
                CustodialSupervisionSupervisor = "Officer Smith",
                AdminLicenseSuspensionObjection = "No",
                AdminLicenseSuspensionDisposition = "Pending",
                AdminLicenseSuspensionExplanation = "N/A",
                VehicleMakeModel = "Toyota Camry",
                VehicleLicensePlate = "ABC1234",
                TowToResidence = true,
                MotionToReturnVehicle = false,
                StateOpposes = "No",
                DispositionMotionToReturn = "Granted",
                VacateResidence = false,
                ResidenceAddress = "123 Main St",
                ExclusivePossessionTo = "John Doe",
                SurrenderWeapons = false,
                SurrenderWeaponsDate = "2026-02-07"
            };

            var response = await _client.PostAsJsonAsync("/api/appearonwarrantnoplea", dto);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<dynamic>();
            Assert.Equal("saved", (string)result.status);
        }
    }
}
