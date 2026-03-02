namespace Munientry.Shared.Dtos
{
    /// <summary>
    /// Result row from [reports].[DMCMuniEntryEventReport].
    /// Replaces the legacy event_type_report_query in crim_sql_server_queries.py,
    /// which was used by run_event_type_report() in authoritycourt_reports.py to display
    /// an event-type table report for a given date and event code.
    ///
    /// SP parameters: @EventCode VARCHAR (e.g. 'FIN'), @EventDate (MM/dd/yyyy)
    /// </summary>
    public class EventReportResultDto
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
}
