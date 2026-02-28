using Munientry.Poc.Api.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Munientry.Poc.Api.Services
{
    public class GeneralNoticeOfHearingService
    {
        private readonly IConfiguration _config;
        public GeneralNoticeOfHearingService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SaveToDatabaseAsync(GeneralNoticeOfHearingDto dto)
        {
            var connStr = _config.GetConnectionString("AuthorityCourt");
            using var conn = new SqlConnection(connStr);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO GeneralNoticeOfHearingEntries
                (CaseNumber, EntryDate, DefendantFirstName, DefendantLastName, HearingDate, HearingTime, AssignedCourtroom, DefenseCounselName, InterpreterRequired, LanguageRequired)
                VALUES (@CaseNumber, @EntryDate, @DefendantFirstName, @DefendantLastName, @HearingDate, @HearingTime, @AssignedCourtroom, @DefenseCounselName, @InterpreterRequired, @LanguageRequired)";
            cmd.Parameters.AddWithValue("@CaseNumber", dto.CaseNumber);
            cmd.Parameters.AddWithValue("@EntryDate", dto.EntryDate);
            cmd.Parameters.AddWithValue("@DefendantFirstName", dto.DefendantFirstName);
            cmd.Parameters.AddWithValue("@DefendantLastName", dto.DefendantLastName);
            cmd.Parameters.AddWithValue("@HearingDate", dto.HearingDate);
            cmd.Parameters.AddWithValue("@HearingTime", dto.HearingTime);
            cmd.Parameters.AddWithValue("@AssignedCourtroom", dto.AssignedCourtroom);
            cmd.Parameters.AddWithValue("@DefenseCounselName", dto.DefenseCounselName ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@InterpreterRequired", dto.InterpreterRequired);
            cmd.Parameters.AddWithValue("@LanguageRequired", dto.LanguageRequired ?? (object)DBNull.Value);
            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
