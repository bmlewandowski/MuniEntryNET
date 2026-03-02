using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Munientry.Api.Options;
using Munientry.Shared.Dtos;

namespace Munientry.Api.Services
{
    /// <summary>
    /// Executes one of the 6 daily case list stored procedures to retrieve
    /// the cases scheduled for a given date and list type.
    ///
    /// Stored procedures:
    ///   arraignments    ? [reports].[DMCMuniEntryArraignment]
    ///   slated          ? [reports].[DMCMuniEntrySlated]
    ///   pleas           ? [reports].[DMCMuniEntryPleas]
    ///   pcvh_fcvh       ? [reports].[DMCMuniEntryPrelimCommContViolHearings]
    ///   final_pretrial  ? [reports].[DMCMuniEntryFinalPreTrials]
    ///   trials_to_court ? [reports].[DMCMuniEntryBenchTrials]
    /// </summary>
    public class DailyListService : IDailyListService
    {
        private readonly string _connectionString;
        private readonly bool _passDateParameter;

        public DailyListService(IOptions<AuthorityCourtOptions> dbOptions, IOptions<DailyListOptions> listOptions)
        {
            _connectionString = dbOptions.Value.ConnectionString;
            _passDateParameter = listOptions.Value.PassDateParameter;
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

            // PassDateParameter=true  ? forward the requested date to @ReportDate (dev/test).
            // PassDateParameter=false ? omit the parameter; SP uses GETDATE() internally (production).
            // Toggle via DailyList:PassDateParameter in appsettings.json / appsettings.Development.json.
            if (_passDateParameter)
                cmd.Parameters.Add("@ReportDate", SqlDbType.Date).Value = reportDate.Date;

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
                    Charge         = CourtDataCleaner.CleanOffenseName(SafeString(reader, "Charge")),
                    EventId        = SafeString(reader, "EventID"),
                    JudgeId        = SafeString(reader, "JudgeID"),
                    DefenseCounsel = CourtDataCleaner.CleanDefenseCounselName(SafeString(reader, "DefenseCounsel")),
                });
            }

            return results;
        }

        private static string? SafeString(IDataReader reader, string column)
        {
            var ordinal = reader.GetOrdinal(column);
            return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
        }
    }
}
