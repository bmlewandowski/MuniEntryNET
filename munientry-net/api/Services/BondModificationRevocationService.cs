using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Munientry.Poc.Api.Data;

namespace Munientry.Poc.Api.Services
{
    public class BondModificationRevocationService
    {
        private readonly IConfiguration _config;
        public BondModificationRevocationService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SaveToDatabaseAsync(BondModificationRevocationDto dto)
        {
            var connStr = _config.GetConnectionString("AuthorityCourt");
            using var conn = new SqlConnection(connStr);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO bond_modification_revocation (case_number, entry_date, defendant_first_name, defendant_last_name, decision_on_bond, bond_type, bond_amount, defense_counsel_name)
VALUES (@CaseNumber, @EntryDate, @DefendantFirstName, @DefendantLastName, @DecisionOnBond, @BondType, @BondAmount, @DefenseCounselName);";
            cmd.Parameters.AddWithValue("@CaseNumber", dto.CaseNumber ?? "");
            cmd.Parameters.AddWithValue("@EntryDate", dto.EntryDate?.ToString("yyyy-MM-dd") ?? (object)System.DBNull.Value);
            cmd.Parameters.AddWithValue("@DefendantFirstName", dto.DefendantFirstName ?? "");
            cmd.Parameters.AddWithValue("@DefendantLastName", dto.DefendantLastName ?? "");
            cmd.Parameters.AddWithValue("@DecisionOnBond", dto.DecisionOnBond ?? "");
            cmd.Parameters.AddWithValue("@BondType", dto.BondType ?? "");
            cmd.Parameters.AddWithValue("@BondAmount", dto.BondAmount ?? "");
            cmd.Parameters.AddWithValue("@DefenseCounselName", dto.DefenseCounselName ?? "");
            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
