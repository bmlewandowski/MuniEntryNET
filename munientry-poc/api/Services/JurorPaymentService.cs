using Munientry.Poc.Api.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Munientry.Poc.Api.Services
{
    public class JurorPaymentService
    {
        private readonly IConfiguration _config;
        public JurorPaymentService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SaveToDatabaseAsync(JurorPaymentDto dto)
        {
            var connStr = _config.GetConnectionString("AuthorityCourt");
            using var conn = new SqlConnection(connStr);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO JurorPaymentEntries
                (CaseNumber, EntryDate, DefendantFirstName, DefendantLastName, TrialDate, TrialLength, JurorsReported, JurorsSeated, JurorsNotSeated, JurorsPayNotSeated, JurorsPaySeated, JuryPanelTotalPay)
                VALUES (@CaseNumber, @EntryDate, @DefendantFirstName, @DefendantLastName, @TrialDate, @TrialLength, @JurorsReported, @JurorsSeated, @JurorsNotSeated, @JurorsPayNotSeated, @JurorsPaySeated, @JuryPanelTotalPay)";
            cmd.Parameters.AddWithValue("@CaseNumber", dto.CaseNumber);
            cmd.Parameters.AddWithValue("@EntryDate", dto.EntryDate);
            cmd.Parameters.AddWithValue("@DefendantFirstName", dto.DefendantFirstName);
            cmd.Parameters.AddWithValue("@DefendantLastName", dto.DefendantLastName);
            cmd.Parameters.AddWithValue("@TrialDate", dto.TrialDate);
            cmd.Parameters.AddWithValue("@TrialLength", dto.TrialLength);
            cmd.Parameters.AddWithValue("@JurorsReported", dto.JurorsReported);
            cmd.Parameters.AddWithValue("@JurorsSeated", dto.JurorsSeated);
            cmd.Parameters.AddWithValue("@JurorsNotSeated", dto.JurorsNotSeated);
            cmd.Parameters.AddWithValue("@JurorsPayNotSeated", dto.JurorsPayNotSeated);
            cmd.Parameters.AddWithValue("@JurorsPaySeated", dto.JurorsPaySeated);
            cmd.Parameters.AddWithValue("@JuryPanelTotalPay", dto.JuryPanelTotalPay);
            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
