using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Munientry.Api.Options;
using Munientry.Shared.Dtos;

namespace Munientry.Api.Services
{
    /// <summary>
    /// Executes the [reports].[DMCMuniEntryCaseSearch] stored procedure to retrieve
    /// all charges and defendant info for a given case number.
    /// Replaces the legacy general_case_search_query in crim_sql_server_queries.py.
    /// </summary>
    public class CaseSearchService : ICaseSearchService
    {
        private readonly string _connectionString;

        public CaseSearchService(IOptions<AuthorityCourtOptions> options)
        {
            _connectionString = options.Value.ConnectionString;
        }

        public async Task<List<CaseSearchResultDto>> SearchCaseAsync(string caseNumber)
        {
            var results = new List<CaseSearchResultDto>();

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("[reports].[DMCMuniEntryCaseSearch]", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@CaseNumber", SqlDbType.NVarChar, 20).Value = caseNumber;

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                results.Add(new CaseSearchResultDto
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

            return results;
        }
    }
}
