namespace Munientry.Shared.Dtos
{
    /// <summary>
    /// Result row from [reports].[DMCMuniEntryNotGuiltyReport].
    /// Replaces the legacy not_guilty_report_query in crim_sql_server_queries.py,
    /// which was used by run_not_guilty_report() / run_not_guilty_report_today()
    /// in daily_reports.py to display an on-screen table of cases that have a
    /// docket journal entry matching a Not Guilty plea or Continuance for a given date.
    ///
    /// SP parameter: @EventDate (MM/dd/yyyy)
    /// </summary>
    public class NotGuiltyReportResultDto
    {
        public string? CaseNumber { get; set; }
        public string? DefFullName { get; set; }
        public string? Remark { get; set; }
    }
}
