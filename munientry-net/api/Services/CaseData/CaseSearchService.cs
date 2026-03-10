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
    /// Executes the [reports].[DMCMuniEntryCaseSearch] stored procedure to retrieve
    /// all charges and defendant info for a given case number.
    /// Replaces the legacy general_case_search_query in crim_sql_server_queries.py.
    /// </summary>
    public class CaseSearchService : SqlServiceBase, ICaseSearchService
    {
        public CaseSearchService(
            IOptions<AuthorityCourtOptions> options,
            ResiliencePipelineProvider<string> pipelineProvider)
            : base(options, pipelineProvider) { }

        public Task<List<CaseSearchResultDto>> SearchCaseAsync(string caseNumber) =>
            ExecuteSpListAsync(
                "[reports].[DMCMuniEntryCaseSearch]",
                cmd => cmd.Parameters.Add("@CaseNumber", SqlDbType.NVarChar, 20).Value = caseNumber,
                reader => new CaseSearchResultDto
                {
                    SubCaseNumber     = reader["SubCaseNumber"]?.ToString(),
                    CaseNumber        = reader["CaseNumber"]?.ToString(),
                    SubCaseId         = Convert.ToInt32(reader["SubCaseID"]),
                    Charge            = CourtDataCleaner.CleanOffenseName(reader["Charge"]?.ToString()),
                    ViolationId       = Convert.ToInt32(reader["ViolationID"]),
                    ViolationDetailId = Convert.ToInt32(reader["ViolationDetailID"]),
                    Statute           = CourtDataCleaner.CleanStatuteName(reader["Statute"]?.ToString()),
                    DegreeCode        = reader["DegreeCode"]?.ToString(),
                    DefFirstName      = reader["DefFirstName"]?.ToString(),
                    DefLastName       = CourtDataCleaner.CleanLastName(reader["DefLastName"]?.ToString()),
                    MovingBool        = reader["MovingBool"] != DBNull.Value && Convert.ToBoolean(reader["MovingBool"]),
                    ViolationDate     = reader["ViolationDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ViolationDate"]),
                    EndDate           = reader["EndDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["EndDate"]),
                    FraInFile         = reader["FraInFile"]?.ToString(),
                    DefenseCounsel    = CourtDataCleaner.CleanDefenseCounselName(reader["DefenseCounsel"]?.ToString()),
                    PubDef            = reader["PubDef"] != DBNull.Value && Convert.ToBoolean(reader["PubDef"]),
                });
    }
}
