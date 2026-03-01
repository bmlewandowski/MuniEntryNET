using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Munientry.Api.Data;

namespace Munientry.Api.Services
{
    /// <summary>
    /// Executes one of the 6 daily case list stored procedures to retrieve
    /// the cases scheduled for a given date and list type.
    ///
    /// Stored procedures:
    ///   arraignments    → [reports].[DMCMuniEntryArraignment]
    ///   slated          → [reports].[DMCMuniEntrySlated]
    ///   pleas           → [reports].[DMCMuniEntryPleas]
    ///   pcvh_fcvh       → [reports].[DMCMuniEntryPrelimCommContViolHearings]
    ///   final_pretrial  → [reports].[DMCMuniEntryFinalPreTrials]
    ///   trials_to_court → [reports].[DMCMuniEntryBenchTrials]
    /// </summary>
    public class DailyListService : IDailyListService
    {
        private readonly string _connectionString;

        public DailyListService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("AuthorityCourt")
                ?? throw new InvalidOperationException("Missing connection string 'AuthorityCourt'.");
        }

        public async Task<List<DailyListResultDto>> GetDailyListAsync(string listType, DateTime reportDate)
        {
            var procName = DailyListStoredProcs.GetProcName(listType);
            if (procName is null)
                throw new ArgumentException(
                    $"Unknown list type '{listType}'. Valid values: {string.Join(", ", DailyListStoredProcs.ValidTypes)}");

            var results = new List<DailyListResultDto>();

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(procName, conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@ReportDate", reportDate.Date);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                results.Add(new DailyListResultDto
                {
                    Time           = SafeString(reader, "Time"),
                    CaseNumber     = SafeString(reader, "CaseNumber"),
                    DefFullName    = SafeString(reader, "DefFullName"),
                    SubCaseNumber  = SafeString(reader, "SubCaseNumber"),
                    Charge         = SafeString(reader, "Charge"),
                    EventId        = SafeString(reader, "EventID"),
                    JudgeId        = SafeString(reader, "JudgeID"),
                    DefenseCounsel = SafeString(reader, "DefenseCounsel"),
                });
            }

            return results;
        }

        private static string? SafeString(IDataReader reader, string column)
        {
            try { return reader[column] == DBNull.Value ? null : reader[column]?.ToString(); }
            catch { return null; }
        }
    }
}
