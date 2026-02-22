using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Munientry.Poc.Api.Data;

namespace Munientry.Poc.Api.Services
{
    /// <summary>
    /// Executes the [reports].[DMCMuniEntryCaseSearch] stored procedure to retrieve
    /// all charges and defendant info for a given case number.
    /// Replaces the legacy general_case_search_query in crim_sql_server_queries.py.
    /// </summary>
    public class CaseSearchService
    {
        private readonly string _connectionString;

        public CaseSearchService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("AuthorityCourt")
                ?? throw new InvalidOperationException("Missing connection string 'AuthorityCourt'.");
        }

        public async Task<List<CaseSearchResultDto>> SearchCaseAsync(string caseNumber)
        {
            var results = new List<CaseSearchResultDto>();

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("[reports].[DMCMuniEntryCaseSearch]", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@CaseNumber", caseNumber);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                results.Add(new CaseSearchResultDto
                {
                    SubCaseNumber    = reader["SubCaseNumber"]?.ToString(),
                    CaseNumber       = reader["CaseNumber"]?.ToString(),
                    SubCaseId        = Convert.ToInt32(reader["SubCaseID"]),
                    Charge           = reader["Charge"]?.ToString(),
                    ViolationId      = Convert.ToInt32(reader["ViolationID"]),
                    ViolationDetailId = Convert.ToInt32(reader["ViolationDetailID"]),
                    Statute          = reader["Statute"]?.ToString(),
                    DegreeCode       = reader["DegreeCode"]?.ToString(),
                    DefFirstName     = reader["DefFirstName"]?.ToString(),
                    DefLastName      = reader["DefLastName"]?.ToString(),
                    MovingBool       = reader["MovingBool"] != DBNull.Value && Convert.ToBoolean(reader["MovingBool"]),
                    ViolationDate    = reader["ViolationDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ViolationDate"]),
                    EndDate          = reader["EndDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["EndDate"]),
                    FraInFile        = reader["FraInFile"]?.ToString(),
                    DefenseCounsel   = reader["DefenseCounsel"]?.ToString(),
                    PubDef           = reader["PubDef"] != DBNull.Value && Convert.ToBoolean(reader["PubDef"]),
                });
            }

            return results;
        }
    }
}
