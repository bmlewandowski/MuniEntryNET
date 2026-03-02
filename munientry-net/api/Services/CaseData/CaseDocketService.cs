using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Munientry.Api.Options;
using Munientry.Shared.Dtos;

namespace Munientry.Api.Services
{
    /// <summary>
    /// Executes [reports].[DMCMuniEntryCaseDocket] to retrieve the docket history for a case.
    /// Replaces the legacy get_case_docket_query used by CrimCaseDocket.get_docket()
    /// in crim_getters.py — previously loaded docket entries into case dialogs.
    /// </summary>
    public class CaseDocketService : ICaseDocketService
    {
        private readonly string _connectionString;

        public CaseDocketService(IOptions<AuthorityCourtOptions> options)
        {
            _connectionString = options.Value.ConnectionString;
        }

        public async Task<List<CaseDocketEntryDto>> GetCaseDocketAsync(string caseNumber)
        {
            var results = new List<CaseDocketEntryDto>();

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("[reports].[DMCMuniEntryCaseDocket]", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@CaseNumber", SqlDbType.NVarChar, 20).Value = caseNumber;

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                results.Add(new CaseDocketEntryDto
                {
                    Date   = reader["Date"] == DBNull.Value ? null : Convert.ToDateTime(reader["Date"]),
                    Remark = reader["Remark"]?.ToString(),
                });
            }

            return results;
        }
    }
}
