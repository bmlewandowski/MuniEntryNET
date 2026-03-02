using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Munientry.Api.Options;
using Munientry.Shared.Dtos;

namespace Munientry.Api.Services
{
    /// <summary>
    /// Executes [reports].[DMCMuniEntryEventReport] to retrieve a report of cases by event
    /// type and date.
    /// Replaces the legacy event_type_report_query used by run_event_type_report()
    /// in authoritycourt_reports.py — previously displayed an on-screen table by event type.
    ///
    /// SP parameters:
    ///   @EventCode VARCHAR  — event type identifier, e.g. 'FIN'
    ///   @EventDate DATE     — the date to query
    /// </summary>
    public class EventReportService : IEventReportService
    {
        private readonly string _connectionString;

        public EventReportService(IOptions<AuthorityCourtOptions> options)
        {
            _connectionString = options.Value.ConnectionString;
        }

        public async Task<List<EventReportResultDto>> GetEventReportAsync(string eventCode, DateTime eventDate)
        {
            var results = new List<EventReportResultDto>();

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("[reports].[DMCMuniEntryEventReport]", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@EventCode", SqlDbType.VarChar, 10).Value = eventCode;
            cmd.Parameters.Add("@EventDate", SqlDbType.Date).Value = eventDate.Date;

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                results.Add(new EventReportResultDto
                {
                    Time           = Safe(reader, "Time"),
                    CaseNumber     = Safe(reader, "CaseNumber"),
                    DefFullName    = Safe(reader, "DefFullName"),
                    SubCaseNumber  = Safe(reader, "SubCaseNumber"),
                    Charge         = Safe(reader, "Charge"),
                    EventId        = Safe(reader, "EventID"),
                    JudgeId        = Safe(reader, "JudgeID"),
                    DefenseCounsel = Safe(reader, "DefenseCounsel"),
                });
            }

            return results;
        }

        private static string? Safe(System.Data.IDataReader r, string col)
        {
            try { return r[col] == DBNull.Value ? null : r[col]?.ToString(); }
            catch { return null; }
        }
    }
}
