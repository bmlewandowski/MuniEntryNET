using Munientry.Poc.Api.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Munientry.Poc.Api.Services
{
    public class ProbationViolationBondService
    {
        private readonly IConfiguration _config;
        public ProbationViolationBondService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SaveToDatabaseAsync(ProbationViolationBondDto dto)
        {
            var connStr = _config.GetConnectionString("DefaultConnection");
            using var conn = new SqlConnection(connStr);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO ProbationViolationBondEntries
                (DefendantFirstName, DefendantLastName, CaseNumber, AppearanceReason, DefenseCounselName, DefenseCounselType, DefenseCounselWaived, ProbableCauseFinding, BondType, BondAmount, NoAlcoholDrugs, Monitoring, MonitoringType, ComplyProtectionOrder, OtherConditions)
                VALUES (@DefendantFirstName, @DefendantLastName, @CaseNumber, @AppearanceReason, @DefenseCounselName, @DefenseCounselType, @DefenseCounselWaived, @ProbableCauseFinding, @BondType, @BondAmount, @NoAlcoholDrugs, @Monitoring, @MonitoringType, @ComplyProtectionOrder, @OtherConditions)";
            cmd.Parameters.AddWithValue("@DefendantFirstName", dto.DefendantFirstName);
            cmd.Parameters.AddWithValue("@DefendantLastName", dto.DefendantLastName);
            cmd.Parameters.AddWithValue("@CaseNumber", dto.CaseNumber);
            cmd.Parameters.AddWithValue("@AppearanceReason", dto.AppearanceReason ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@DefenseCounselName", dto.DefenseCounselName ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@DefenseCounselType", dto.DefenseCounselType ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@DefenseCounselWaived", dto.DefenseCounselWaived);
            cmd.Parameters.AddWithValue("@ProbableCauseFinding", dto.ProbableCauseFinding ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@BondType", dto.BondType ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@BondAmount", dto.BondAmount ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@NoAlcoholDrugs", dto.NoAlcoholDrugs);
            cmd.Parameters.AddWithValue("@Monitoring", dto.Monitoring);
            cmd.Parameters.AddWithValue("@MonitoringType", dto.MonitoringType ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@ComplyProtectionOrder", dto.ComplyProtectionOrder);
            cmd.Parameters.AddWithValue("@OtherConditions", dto.OtherConditions);
            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
