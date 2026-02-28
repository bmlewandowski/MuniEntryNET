using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Munientry.Poc.Api.Data;

namespace Munientry.Poc.Api.Services
{
    public class DiversionPleaService
    {
        private readonly IConfiguration _config;
        public DiversionPleaService(IConfiguration config)
        {
            _config = config;
        }

        public async Task AddDiversionPleaAsync(DiversionPleaDto dto)
        {
            var connStr = _config.GetConnectionString("AuthorityCourt");
            using var conn = new SqlConnection(connStr);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO diversion_plea (
                defendant_first_name, defendant_last_name, case_number, date, defense_counsel_name, defense_counsel_type, defense_counsel_waived, appearance_reason, charges, diversion_type, diversion_completion_date, diversion_fine_pay_date, probation, pay_restitution, pay_restitution_to, pay_restitution_amount, other_conditions, other_conditions_text
            ) VALUES (
                @DefendantFirstName, @DefendantLastName, @CaseNumber, @Date, @DefenseCounselName, @DefenseCounselType, @DefenseCounselWaived, @AppearanceReason, @Charges, @DiversionType, @DiversionCompletionDate, @DiversionFinePayDate, @Probation, @PayRestitution, @PayRestitutionTo, @PayRestitutionAmount, @OtherConditions, @OtherConditionsText
            );";
            cmd.Parameters.AddWithValue("@DefendantFirstName", dto.DefendantFirstName ?? "");
            cmd.Parameters.AddWithValue("@DefendantLastName", dto.DefendantLastName ?? "");
            cmd.Parameters.AddWithValue("@CaseNumber", dto.CaseNumber ?? "");
            cmd.Parameters.AddWithValue("@Date", (object?)dto.Date ?? System.DBNull.Value);
            cmd.Parameters.AddWithValue("@DefenseCounselName", dto.DefenseCounselName ?? "");
            cmd.Parameters.AddWithValue("@DefenseCounselType", dto.DefenseCounselType ?? "");
            cmd.Parameters.AddWithValue("@DefenseCounselWaived", dto.DefenseCounselWaived);
            cmd.Parameters.AddWithValue("@AppearanceReason", dto.AppearanceReason ?? "");
            cmd.Parameters.AddWithValue("@Charges", dto.Charges ?? "");
            cmd.Parameters.AddWithValue("@DiversionType", dto.DiversionType ?? "");
            cmd.Parameters.AddWithValue("@DiversionCompletionDate", (object?)dto.DiversionCompletionDate ?? System.DBNull.Value);
            cmd.Parameters.AddWithValue("@DiversionFinePayDate", (object?)dto.DiversionFinePayDate ?? System.DBNull.Value);
            cmd.Parameters.AddWithValue("@Probation", dto.Probation);
            cmd.Parameters.AddWithValue("@PayRestitution", dto.PayRestitution);
            cmd.Parameters.AddWithValue("@PayRestitutionTo", dto.PayRestitutionTo ?? "");
            cmd.Parameters.AddWithValue("@PayRestitutionAmount", (object?)dto.PayRestitutionAmount ?? System.DBNull.Value);
            cmd.Parameters.AddWithValue("@OtherConditions", dto.OtherConditions);
            cmd.Parameters.AddWithValue("@OtherConditionsText", dto.OtherConditionsText ?? "");
            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
