namespace Munientry.Api.Data
{
    /// <summary>
    /// Result row from any of the 6 daily case list stored procedures:
    ///   [reports].[DMCMuniEntryArraignment]
    ///   [reports].[DMCMuniEntrySlated]
    ///   [reports].[DMCMuniEntryPleas]
    ///   [reports].[DMCMuniEntryPrelimCommContViolHearings]
    ///   [reports].[DMCMuniEntryFinalPreTrials]
    ///   [reports].[DMCMuniEntryBenchTrials]
    /// </summary>
    public class DailyListResultDto
    {
        public string? Time { get; set; }
        public string? CaseNumber { get; set; }
        public string? DefFullName { get; set; }
        public string? SubCaseNumber { get; set; }
        public string? Charge { get; set; }
        public string? EventId { get; set; }
        public string? JudgeId { get; set; }
        public string? DefenseCounsel { get; set; }
    }

    /// <summary>
    /// Maps the friendly list-type names used in the UI to their stored procedure names.
    /// </summary>
    public static class DailyListStoredProcs
    {
        private static readonly Dictionary<string, string> Map = new(StringComparer.OrdinalIgnoreCase)
        {
            { "arraignments",    "[reports].[DMCMuniEntryArraignment]" },
            { "slated",          "[reports].[DMCMuniEntrySlated]" },
            { "pleas",           "[reports].[DMCMuniEntryPleas]" },
            { "pcvh_fcvh",       "[reports].[DMCMuniEntryPrelimCommContViolHearings]" },
            { "final_pretrial",  "[reports].[DMCMuniEntryFinalPreTrials]" },
            { "trials_to_court", "[reports].[DMCMuniEntryBenchTrials]" },
        };

        public static string? GetProcName(string listType) =>
            Map.TryGetValue(listType, out var proc) ? proc : null;

        public static IEnumerable<string> ValidTypes => Map.Keys;
    }
}
