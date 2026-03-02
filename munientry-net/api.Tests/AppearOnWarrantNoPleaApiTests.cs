using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using Munientry.Shared.Dtos;
using Munientry.Api.Tests.Infrastructure;

namespace api.Tests
{
    public class AppearOnWarrantNoPleaApiTests : IClassFixture<MuniEntryWebApplicationFactory>
    {
        private readonly MuniEntryWebApplicationFactory _factory;

        public AppearOnWarrantNoPleaApiTests(MuniEntryWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Post_AppearOnWarrantNoPlea_ReturnsDocx()
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

            var client = _factory.CreateClient();
            var response = await client.PostAsJsonAsync("/api/v1/appearonwarrantnoplea", dto);
            response.EnsureSuccessStatusCode();
            Assert.Equal(
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                response.Content.Headers.ContentType?.MediaType);
            var bytes = await response.Content.ReadAsByteArrayAsync();
            Assert.True(bytes.Length > 0, "Response body should contain DOCX content.");
        }
    }
}
