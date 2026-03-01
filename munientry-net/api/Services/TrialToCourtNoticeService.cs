using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Munientry.Api.Data;

namespace Munientry.Api.Services
{
    public class TrialToCourtNoticeService : ITrialToCourtNoticeService
    {
        private readonly IConfiguration _config;
        public TrialToCourtNoticeService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SaveToDatabaseAsync(TrialToCourtNoticeDto dto)
        {
            var connStr = _config.GetConnectionString("AuthorityCourt");
            using var conn = new SqlConnection(connStr);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO case_events (case_number, event_type_id, event_location_id, case_event_date, case_event_time, def_last_name, def_first_name)
SELECT @CaseNumber, et.event_type_id, el.location_id, @EventDate, @EventTime, @DefLastName, @DefFirstName
FROM event_types as et, event_locations as el
WHERE et.event_type_name = 'Trial to Court' AND el.location_name = @Location;";
            cmd.Parameters.AddWithValue("@CaseNumber", dto.CaseNumber ?? "");
            cmd.Parameters.AddWithValue("@EventDate", dto.TrialToCourtDate?.ToString("yyyy-MM-dd") ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@EventTime", dto.TrialToCourtTime ?? "");
            cmd.Parameters.AddWithValue("@DefLastName", dto.DefendantLastName ?? "");
            cmd.Parameters.AddWithValue("@DefFirstName", dto.DefendantFirstName ?? "");
            cmd.Parameters.AddWithValue("@Location", dto.AssignedCourtroom ?? "");
            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
