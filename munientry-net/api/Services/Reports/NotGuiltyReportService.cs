using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Munientry.Api.Options;
using Munientry.Shared.Dtos;

namespace Munientry.Api.Services
{
    /// <summary>
    /// Executes [reports].[DMCMuniEntryNotGuiltyReport] to retrieve cases where a journal entry
    /// matching a Not Guilty plea or Continuance exists for a given arraignment date.
    /// Replaces the legacy not_guilty_report_query used by run_not_guilty_report() and
    /// run_not_guilty_report_today() in daily_reports.py.
    ///
    /// SP parameter: @EventDate DATE
    /// </summary>
    public class NotGuiltyReportService : INotGuiltyReportService
    {
        private readonly string _connectionString;

        public NotGuiltyReportService(IOptions<AuthorityCourtOptions> options)
        {
            _connectionString = options.Value.ConnectionString;
        }

        public async Task<List<NotGuiltyReportResultDto>> GetNotGuiltyReportAsync(DateTime eventDate)
        {
            var results = new List<NotGuiltyReportResultDto>();

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("[reports].[DMCMuniEntryNotGuiltyReport]", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@EventDate", SqlDbType.Date).Value = eventDate.Date;

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                results.Add(new NotGuiltyReportResultDto
                {
                    CaseNumber  = reader["CaseNumber"]?.ToString(),
                    DefFullName = reader["DefFullName"]?.ToString(),
                    Remark      = reader["Remark"]?.ToString(),
                });
            }

            return results;
        }
    }
}
