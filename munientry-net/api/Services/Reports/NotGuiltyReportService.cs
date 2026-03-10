using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Munientry.Api.Options;
using Munientry.Shared.Dtos;
using Polly;
using Polly.Registry;

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
    public class NotGuiltyReportService : SqlServiceBase, INotGuiltyReportService
    {
        public NotGuiltyReportService(
            IOptions<AuthorityCourtOptions> options,
            ResiliencePipelineProvider<string> pipelineProvider)
            : base(options, pipelineProvider) { }

        public Task<List<NotGuiltyReportResultDto>> GetNotGuiltyReportAsync(DateTime eventDate) =>
            ExecuteSpListAsync(
                "[reports].[DMCMuniEntryNotGuiltyReport]",
                cmd => cmd.Parameters.Add("@EventDate", SqlDbType.Date).Value = eventDate.Date,
                reader => new NotGuiltyReportResultDto
                {
                    CaseNumber  = reader["CaseNumber"]?.ToString(),
                    DefFullName = reader["DefFullName"]?.ToString(),
                    Remark      = reader["Remark"]?.ToString(),
                });
    }
}
