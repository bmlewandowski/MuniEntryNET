using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Munientry.Api.Data;

namespace Munientry.Api.Services
{
    public class BondHearingService : IBondHearingService
    {
        private readonly IConfiguration _config;
        public BondHearingService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SaveToDatabaseAsync(BondHearingDto dto)
        {
            var connStr = _config.GetConnectionString("AuthorityCourt");
            using var conn = new SqlConnection(connStr);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO bond_hearings (case_number, entry_date, defendant_first_name, defendant_last_name, bond_type, bond_amount, defense_counsel_name)
VALUES (@CaseNumber, @EntryDate, @DefendantFirstName, @DefendantLastName, @BondType, @BondAmount, @DefenseCounselName);";
            cmd.Parameters.AddWithValue("@CaseNumber", dto.CaseNumber ?? "");
            cmd.Parameters.AddWithValue("@EntryDate", dto.EntryDate?.ToString("yyyy-MM-dd") ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@DefendantFirstName", dto.DefendantFirstName ?? "");
            cmd.Parameters.AddWithValue("@DefendantLastName", dto.DefendantLastName ?? "");
            cmd.Parameters.AddWithValue("@BondType", dto.BondType ?? "");
            cmd.Parameters.AddWithValue("@BondAmount", dto.BondAmount ?? "");
            cmd.Parameters.AddWithValue("@DefenseCounselName", dto.DefenseCounselName ?? "");
            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
