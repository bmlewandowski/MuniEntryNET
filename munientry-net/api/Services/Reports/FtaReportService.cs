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
    /// Executes [reports].[DMCMuniEntryFTAReport] to retrieve case numbers eligible for
    /// Failure to Appear warrants from a given arraignment date.
    /// Replaces the legacy batch_fta_query used by run_batch_fta_process() in batch_menu.py.
    ///
    /// The SP applies the same FTA eligibility rules as the legacy query:
    ///   - Mandatory appearance cases
    ///   - Criminal cases (CaseType = 3)
    ///   - Traffic cases with out-of-state non-compact license (MI, TN, GA, MA, WI, NV),
    ///     CDL/commercial vehicle, or No OL offense
    ///
    /// In the legacy app, batch_fta_query received both event_date and next_day (event_date +1
    /// day) and the next-day filter was part of the inline SQL. The SP handles this internally.
    ///
    /// SP parameter: @EventDate DATE
    ///
    /// The caller receives the list and can generate one DOCX per case number using
    /// DMCMuniEntryCaseSearch to pre-populate the Batch_Failure_To_Appear_Arraignment_Template.
    /// </summary>
    public class FtaReportService : SqlServiceBase, IFtaReportService
    {
        public FtaReportService(
            IOptions<AuthorityCourtOptions> options,
            ResiliencePipelineProvider<string> pipelineProvider)
            : base(options, pipelineProvider) { }

        public Task<List<FtaReportResultDto>> GetFtaReportAsync(DateTime eventDate) =>
            ExecuteSpListAsync(
                "[reports].[DMCMuniEntryFTAReport]",
                cmd => cmd.Parameters.Add("@EventDate", SqlDbType.Date).Value = eventDate.Date,
                reader => new FtaReportResultDto
                {
                    CaseNumber = reader["CaseNumber"]?.ToString(),
                });
    }
}
