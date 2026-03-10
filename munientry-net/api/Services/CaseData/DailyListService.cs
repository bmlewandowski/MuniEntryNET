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
    /// Executes one of the 6 daily case list stored procedures to retrieve
    /// the cases scheduled for the current court day.
    ///
    /// These SPs accept no date parameter — they filter by date internally:
    ///   Production:  WHERE ce.EventDate = CAST(GETDATE() AS Date)  (today only)
    ///   Test server: WHERE ce.EventDate > '01/01/2026'  (relaxed by DBA for dev)
    ///
    /// Stored procedures:
    ///   arraignments    → [reports].[DMCMuniEntryArraignment]
    ///   slated          → [reports].[DMCMuniEntrySlated]
    ///   pleas           → [reports].[DMCMuniEntryPleas]
    ///   pcvh_fcvh       → [reports].[DMCMuniEntryPrelimCommContViolHearings]
    ///   final_pretrial  → [reports].[DMCMuniEntryFinalPreTrials]
    ///   trials_to_court → [reports].[DMCMuniEntryBenchTrials]
    /// </summary>
    public class DailyListService : SqlServiceBase, IDailyListService
    {
        public DailyListService(
            IOptions<AuthorityCourtOptions> dbOptions,
            ResiliencePipelineProvider<string> pipelineProvider)
            : base(dbOptions, pipelineProvider) { }

        public async Task<List<DailyListResultDto>> GetDailyListAsync(string listType)
        {
            var procName = DailyListStoredProcs.GetProcName(listType);
            if (procName is null)
                throw new ArgumentException(
                    $"Unknown list type '{listType}'. Valid values: {string.Join(", ", DailyListStoredProcs.ValidTypes)}");

            return await ExecuteSpListAsync(
                procName,
                _ => { },
                reader => new DailyListResultDto
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

        private static string? SafeString(IDataReader reader, string column)
        {
            var ordinal = reader.GetOrdinal(column);
            return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
        }
    }
}
