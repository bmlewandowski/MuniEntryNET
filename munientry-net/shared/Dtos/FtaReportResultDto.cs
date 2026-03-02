namespace Munientry.Shared.Dtos
{
    /// <summary>
    /// Result row from [reports].[DMCMuniEntryFTAReport].
    /// Replaces the legacy batch_fta_query in crim_sql_server_queries.py,
    /// which was used by run_batch_fta_process() in batch_menu.py to retrieve
    /// FTA-eligible case numbers from a given arraignment date.
    ///
    /// The SP applies the same filtering logic as the legacy query:
    ///   - Mandatory appearance cases
    ///   - Criminal cases (CaseType=3)
    ///   - Traffic cases with out-of-state non-compact license (MI, TN, GA, MA, WI, NV),
    ///     commercial vehicle (CDL), or No OL offense (ViolationId=12368)
    ///
    /// SP parameter: @EventDate (MM/dd/yyyy)
    ///
    /// The caller receives this list and can generate one DOCX per case number.
    /// See Batch_Failure_To_Appear_Arraignment_Template.docx.
    /// In the legacy app the next-day calculation was done in Python before passing
    /// two dates to the query; the SP handles that internally.
    /// </summary>
    public class FtaReportResultDto
    {
        public string? CaseNumber { get; set; }
    }
}
