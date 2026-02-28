using System.Net.Http.Json;
using System.Threading.Tasks;
using Munientry.Poc.Api.Data;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Munientry.Poc.Api.Tests
{
    public class NotGuiltyAppearBondSpecialApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public NotGuiltyAppearBondSpecialApiTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task PostNotGuiltyAppearBondSpecial_ReturnsDocx()
        {
            var client = _factory.CreateClient();
            var dto = new NotGuiltyAppearBondSpecialDto
            {
                DefendantFirstName = "John",
                DefendantLastName = "Doe",
                CaseNumber = "2026-CR-009",
                BondType = "Cash",
                BondAmount = "1000",
                BondModificationDecision = null,
                NoContactName = "Jane Doe",
                CustodialSupervisionSupervisor = "Officer Smith",
                AdminLicenseSuspensionObjection = "No",
                AdminLicenseSuspensionDisposition = "Pending",
                AdminLicenseSuspensionExplanation = "N/A",
                VehicleMakeModel = "Toyota Camry",
                VehicleLicensePlate = "ABC1234",
                TowToResidence = false,
                MotionToReturnVehicle = false,
                StateOpposes = "No",
                DispositionMotionToReturn = "Granted",
                VacateResidence = false,
                ResidenceAddress = "123 Main St",
                ExclusivePossessionTo = "John Doe",
                SurrenderWeapons = false,
                SurrenderWeaponsDate = "2026-02-07"
            };
            var response = await client.PostAsJsonAsync("/api/notguiltyappearbondspecial", dto);
            response.EnsureSuccessStatusCode();
            Assert.Equal(
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                response.Content.Headers.ContentType?.MediaType);
            var bytes = await response.Content.ReadAsByteArrayAsync();
            Assert.True(bytes.Length > 0, "Response body should contain DOCX content.");
        }
    }
}
