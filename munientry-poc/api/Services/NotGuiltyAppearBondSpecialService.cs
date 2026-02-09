using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Munientry.Poc.Api.Data;

namespace Munientry.Poc.Api.Services
{
    public class NotGuiltyAppearBondSpecialService
    {
        private readonly IConfiguration _config;
        public NotGuiltyAppearBondSpecialService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SaveToDatabaseAsync(NotGuiltyAppearBondSpecialDto dto)
        {
            var connStr = _config.GetConnectionString("AuthorityCourt");
            using var conn = new SqlConnection(connStr);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO not_guilty_appear_bond_special (
                defendant_first_name, defendant_last_name, case_number, bond_type, bond_amount, bond_modification_decision, no_contact_name, custodial_supervision_supervisor, admin_license_suspension_objection, admin_license_suspension_disposition, admin_license_suspension_explanation, vehicle_make_model, vehicle_license_plate, tow_to_residence, motion_to_return_vehicle, state_opposes, disposition_motion_to_return, vacate_residence, residence_address, exclusive_possession_to, surrender_weapons, surrender_weapons_date
            ) VALUES (
                @DefendantFirstName, @DefendantLastName, @CaseNumber, @BondType, @BondAmount, @BondModificationDecision, @NoContactName, @CustodialSupervisionSupervisor, @AdminLicenseSuspensionObjection, @AdminLicenseSuspensionDisposition, @AdminLicenseSuspensionExplanation, @VehicleMakeModel, @VehicleLicensePlate, @TowToResidence, @MotionToReturnVehicle, @StateOpposes, @DispositionMotionToReturn, @VacateResidence, @ResidenceAddress, @ExclusivePossessionTo, @SurrenderWeapons, @SurrenderWeaponsDate
            );";
            cmd.Parameters.AddWithValue("@DefendantFirstName", dto.DefendantFirstName ?? "");
            cmd.Parameters.AddWithValue("@DefendantLastName", dto.DefendantLastName ?? "");
            cmd.Parameters.AddWithValue("@CaseNumber", dto.CaseNumber ?? "");
            cmd.Parameters.AddWithValue("@BondType", dto.BondType ?? "");
            cmd.Parameters.AddWithValue("@BondAmount", dto.BondAmount ?? "");
            cmd.Parameters.AddWithValue("@BondModificationDecision", dto.BondModificationDecision ?? "");
            cmd.Parameters.AddWithValue("@NoContactName", dto.NoContactName ?? "");
            cmd.Parameters.AddWithValue("@CustodialSupervisionSupervisor", dto.CustodialSupervisionSupervisor ?? "");
            cmd.Parameters.AddWithValue("@AdminLicenseSuspensionObjection", dto.AdminLicenseSuspensionObjection ?? "");
            cmd.Parameters.AddWithValue("@AdminLicenseSuspensionDisposition", dto.AdminLicenseSuspensionDisposition ?? "");
            cmd.Parameters.AddWithValue("@AdminLicenseSuspensionExplanation", dto.AdminLicenseSuspensionExplanation ?? "");
            cmd.Parameters.AddWithValue("@VehicleMakeModel", dto.VehicleMakeModel ?? "");
            cmd.Parameters.AddWithValue("@VehicleLicensePlate", dto.VehicleLicensePlate ?? "");
            cmd.Parameters.AddWithValue("@TowToResidence", (object?)dto.TowToResidence ?? System.DBNull.Value);
            cmd.Parameters.AddWithValue("@MotionToReturnVehicle", (object?)dto.MotionToReturnVehicle ?? System.DBNull.Value);
            cmd.Parameters.AddWithValue("@StateOpposes", dto.StateOpposes ?? "");
            cmd.Parameters.AddWithValue("@DispositionMotionToReturn", dto.DispositionMotionToReturn ?? "");
            cmd.Parameters.AddWithValue("@VacateResidence", (object?)dto.VacateResidence ?? System.DBNull.Value);
            cmd.Parameters.AddWithValue("@ResidenceAddress", dto.ResidenceAddress ?? "");
            cmd.Parameters.AddWithValue("@ExclusivePossessionTo", dto.ExclusivePossessionTo ?? "");
            cmd.Parameters.AddWithValue("@SurrenderWeapons", (object?)dto.SurrenderWeapons ?? System.DBNull.Value);
            cmd.Parameters.AddWithValue("@SurrenderWeaponsDate", dto.SurrenderWeaponsDate ?? "");
            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
