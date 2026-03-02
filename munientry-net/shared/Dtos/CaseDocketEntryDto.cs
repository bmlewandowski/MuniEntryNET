namespace Munientry.Shared.Dtos
{
    /// <summary>
    /// Result row from [reports].[DMCMuniEntryCaseDocket].
    /// Replaces the legacy get_case_docket_query in crim_sql_server_queries.py,
    /// which was used by CrimCaseDocket.get_docket() in crim_getters.py to display
    /// docket history in case dialogs.
    /// </summary>
    public class CaseDocketEntryDto
    {
        public DateTime? Date { get; set; }
        public string? Remark { get; set; }
    }
}
