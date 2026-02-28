using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Munientry.Poc.Api.Data;

namespace Munientry.Poc.Api.Services
{
    public class LeapValidSentencingService
    {
        private readonly IConfiguration _config;
        public LeapValidSentencingService(IConfiguration config)
        {
            _config = config;
        }

        public async Task AddLeapValidSentencingAsync(LeapValidSentencingDto dto)
        {
            var connStr = _config.GetConnectionString("AuthorityCourt");
            using var conn = new SqlConnection(connStr);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO leap_valid_sentencing (
                defendant_first_name, defendant_last_name, case_number, date, defense_counsel_name, defense_counsel_type, defense_counsel_waived, appearance_reason, charges
            ) VALUES (
                @DefendantFirstName, @DefendantLastName, @CaseNumber, @Date, @DefenseCounselName, @DefenseCounselType, @DefenseCounselWaived, @AppearanceReason, @Charges
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
            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
