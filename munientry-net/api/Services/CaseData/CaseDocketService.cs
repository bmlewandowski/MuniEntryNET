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
    /// Executes [reports].[DMCMuniEntryCaseDocket] to retrieve the docket history for a case.
    /// Replaces the legacy get_case_docket_query used by CrimCaseDocket.get_docket()
    /// in crim_getters.py — previously loaded docket entries into case dialogs.
    /// </summary>
    public class CaseDocketService : SqlServiceBase, ICaseDocketService
    {
        public CaseDocketService(
            IOptions<AuthorityCourtOptions> options,
            ResiliencePipelineProvider<string> pipelineProvider)
            : base(options, pipelineProvider) { }

        public Task<List<CaseDocketEntryDto>> GetCaseDocketAsync(string caseNumber) =>
            ExecuteSpListAsync(
                "[reports].[DMCMuniEntryCaseDocket]",
                cmd => cmd.Parameters.Add("@CaseNumber", SqlDbType.NVarChar, 20).Value = caseNumber,
                reader => new CaseDocketEntryDto
                {
                    Date   = reader["Date"] == DBNull.Value ? null : Convert.ToDateTime(reader["Date"]),
                    Remark = reader["Remark"]?.ToString(),
                });
    }
}
