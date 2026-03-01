using Munientry.Api.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Munientry.Api.Services
{
    public class TimeToPayOrderService : ITimeToPayOrderService
    {
        private readonly IConfiguration _config;
        public TimeToPayOrderService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SaveToDatabaseAsync(TimeToPayOrderDto dto)
        {
            var connStr = _config.GetConnectionString("AuthorityCourt");
            using var conn = new SqlConnection(connStr);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO TimeToPayOrderEntries
                (CaseNumber, EntryDate, DefendantFirstName, DefendantLastName, AppearanceDate)
                VALUES (@CaseNumber, @EntryDate, @DefendantFirstName, @DefendantLastName, @AppearanceDate)";
            cmd.Parameters.AddWithValue("@CaseNumber", dto.CaseNumber);
            cmd.Parameters.AddWithValue("@EntryDate", dto.EntryDate);
            cmd.Parameters.AddWithValue("@DefendantFirstName", dto.DefendantFirstName);
            cmd.Parameters.AddWithValue("@DefendantLastName", dto.DefendantLastName);
            cmd.Parameters.AddWithValue("@AppearanceDate", dto.AppearanceDate);
            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
