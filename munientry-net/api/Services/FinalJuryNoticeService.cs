using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Munientry.Api.Data;

namespace Munientry.Api.Services
{
    public class FinalJuryNoticeService : IFinalJuryNoticeService
    {
        private readonly IConfiguration _config;
        public FinalJuryNoticeService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SaveToDatabaseAsync(FinalJuryNoticeDto dto)
        {
            var connStr = _config.GetConnectionString("AuthorityCourt");
            using var conn = new SqlConnection(connStr);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO case_events (case_number, event_type_id, event_location_id, case_event_date, def_last_name, def_first_name, defense_counsel_name)
SELECT @CaseNumber, et.event_type_id, el.location_id, @EntryDate, @DefLastName, @DefFirstName, @DefenseCounselName
FROM event_types as et, event_locations as el
WHERE et.event_type_name = 'Final Jury Notice' AND el.location_name = @Location;";
            cmd.Parameters.AddWithValue("@CaseNumber", dto.CaseNumber ?? "");
            cmd.Parameters.AddWithValue("@EntryDate", dto.EntryDate?.ToString("yyyy-MM-dd") ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@DefLastName", dto.DefendantLastName ?? "");
            cmd.Parameters.AddWithValue("@DefFirstName", dto.DefendantFirstName ?? "");
            cmd.Parameters.AddWithValue("@DefenseCounselName", dto.DefenseCounselName ?? "");
            cmd.Parameters.AddWithValue("@Location", "Courtroom"); // Adjust as needed
            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
