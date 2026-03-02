using System.IO.Compression;
using Munientry.Shared.Dtos;

using Munientry.Api.Services;

namespace Munientry.Api.Endpoints;

/// <summary>
/// GET endpoints for batch/operational reports backed by the [reports].[DMCMuniEntry*] SPs.
/// All are read-only — no writes occur.
///
/// Legacy equivalents (munientry/mainmenu/reports/ and mainmenu/batch_menu.py):
///   /reports/not-guilty/{date}            ← run_not_guilty_report()    → DMCMuniEntryNotGuiltyReport
///   /reports/events/{eventCode}/{date}    ← run_event_type_report()    → DMCMuniEntryEventReport
///   /reports/fta/{date}                   ← run_batch_fta_process()    → DMCMuniEntryFTAReport (list)
///   /reports/fta/entry/{caseNumber}/{date} ← create_entry()            → single DOCX
///   /reports/fta/batch/{date}             ← create_fta_entries()       → ZIP of all DOCXs
/// </summary>
internal static class ReportsEndpoints
{
    private const string FtaTemplate = "Batch_Failure_To_Appear_Arraignment_Template.docx";

    internal static IEndpointRouteBuilder MapReportsEndpoints(this IEndpointRouteBuilder app)
    {
        // Not Guilty Report — cases with a NG plea or continuance journal entry for a given date.
        // Replaces: not_guilty_report_query(event_date) → displayed in an on-screen table.
        // date format: yyyy-MM-dd
        app.MapGet("/reports/not-guilty/{date}", async (string date, INotGuiltyReportService service) =>
        {
            if (!DateTime.TryParse(date, out var eventDate))
                return Results.BadRequest("Invalid date format. Use yyyy-MM-dd.");
            var result = await service.GetNotGuiltyReportAsync(eventDate);
            return Results.Ok(result);
        });

        // Event Type Report — all cases scheduled for a specific event type on a given date.
        // Replaces: event_type_report_query(report_date, event_codes) → displayed in an on-screen table.
        // eventCode: string passed directly to the SP @EventCode parameter — confirm valid codes with DBA.
        //   The legacy Python passed event IDs e.g. "('27','28','77','169')" for Arraignments.
        //   The SP may accept a friendly name or a numeric code; verify with the DBA before production use.
        // date format: yyyy-MM-dd
        app.MapGet("/reports/events/{eventCode}/{date}", async (string eventCode, string date, IEventReportService service) =>
        {
            if (!DateTime.TryParse(date, out var eventDate))
                return Results.BadRequest("Invalid date format. Use yyyy-MM-dd.");
            var result = await service.GetEventReportAsync(eventCode, eventDate);
            return Results.Ok(result);
        });

        // FTA Report — returns FTA-eligible case numbers from a given arraignment date.
        // Replaces: batch_fta_query(event_date, next_day) — the SP handles next-day logic internally.
        // date format: yyyy-MM-dd
        app.MapGet("/reports/fta/{date}", async (string date, IFtaReportService service) =>
        {
            if (!DateTime.TryParse(date, out var eventDate))
                return Results.BadRequest("Invalid date format. Use yyyy-MM-dd.");
            var result = await service.GetFtaReportAsync(eventDate);
            return Results.Ok(result);
        });

        // Single FTA DOCX — generates one FTA warrant entry for a specific case number and date.
        // Replaces: create_entry(case_data, event_date) → Batch_Failure_To_Appear_Arraignment_Template.docx
        // Template fields: case_number, def_first_name, def_last_name, case_event_date, warrant_rule
        //   warrant_rule derived from case number type code (chars 2-4):
        //     CRB → "Criminal Rule 4" (Criminal Rule B cases)
        //     All others → "Traffic Rule 7" (TRC, TRD, TRE traffic variants)
        // date format: yyyy-MM-dd
        app.MapGet("/reports/fta/entry/{caseNumber}/{date}",
            async (string caseNumber, string date, ICaseSearchService caseSearch) =>
        {
            if (!DateTime.TryParse(date, out var eventDate))
                return Results.BadRequest("Invalid date format. Use yyyy-MM-dd.");

            var caseResults = await caseSearch.SearchCaseAsync(caseNumber);
            if (caseResults.Count == 0)
                return Results.NotFound($"Case '{caseNumber}' not found.");

            var first = caseResults[0];
            var bytes = BuildFtaDocx(first.CaseNumber ?? caseNumber, first.DefFirstName ?? "",
                                     first.DefLastName ?? "", eventDate);
            var fileName = $"{caseNumber}_FTA_Arraignment.docx";
            return Results.File(bytes, DocxResult.MimeType, fileName);
        });

        // Batch FTA entries — generates all FTA DOCXs for the date and returns them as a ZIP.
        // Replaces: run_batch_fta_process() which saved files to BATCH_SAVE_PATH and opened the folder.
        // The ZIP contains one DOCX per eligible case ({caseNumber}_FTA_Arraignment.docx).
        // date format: yyyy-MM-dd
        app.MapGet("/reports/fta/batch/{date}",
            async (string date, IFtaReportService ftaService, ICaseSearchService caseSearch) =>
        {
            if (!DateTime.TryParse(date, out var eventDate))
                return Results.BadRequest("Invalid date format. Use yyyy-MM-dd.");

            var cases = await ftaService.GetFtaReportAsync(eventDate);
            if (cases.Count == 0)
                return Results.NotFound("No FTA-eligible cases found for the specified date.");

            using var zipStream = new MemoryStream();
            using (var zip = new ZipArchive(zipStream, ZipArchiveMode.Create, leaveOpen: true))
            {
                foreach (var c in cases)
                {
                    if (string.IsNullOrWhiteSpace(c.CaseNumber)) continue;

                    var caseResults = await caseSearch.SearchCaseAsync(c.CaseNumber);
                    var first = caseResults.FirstOrDefault();

                    var bytes = BuildFtaDocx(first?.CaseNumber ?? c.CaseNumber,
                                             first?.DefFirstName ?? "",
                                             first?.DefLastName ?? "",
                                             eventDate);

                    var entry = zip.CreateEntry($"{c.CaseNumber}_FTA_Arraignment.docx");
                    using var entryStream = entry.Open();
                    await entryStream.WriteAsync(bytes);
                }
            }

            var zipName = $"FTA_Batch_{eventDate:yyyyMMdd}.zip";
            return Results.File(zipStream.ToArray(), "application/zip", zipName);
        });

        return app;
    }

    // ── Helpers ────────────────────────────────────────────────────────────────

    /// <summary>
    /// Fills Batch_Failure_To_Appear_Arraignment_Template.docx with per-case data.
    /// Matches the legacy create_entry() in batch_menu.py exactly.
    /// </summary>
    private static byte[] BuildFtaDocx(
        string caseNumber, string defFirstName, string defLastName, DateTime eventDate)
    {
        var templatePath = Path.Combine("Templates", "source", FtaTemplate);
        return DocxTemplateProcessor.FillTemplate(templatePath, new Dictionary<string, object>
        {
            ["case_number"]      = caseNumber,
            ["def_first_name"]   = defFirstName,
            ["def_last_name"]    = defLastName,
            ["case_event_date"]  = eventDate.ToString("MMMM dd, yyyy"),
            ["warrant_rule"]     = DeriveWarrantRule(caseNumber),
        });
    }

    /// <summary>
    /// Replicates set_warrant_rule() from batch_menu.py.
    /// Case numbers are 10 characters: 2-digit year + 3-char type code + 5-digit number.
    ///   e.g. "22CRB01234" → type code at [2..4] = "CRB" → Criminal Rule 4
    ///        "22TRC01234" → type code at [2..4] = "TRC" → Traffic Rule 7
    /// </summary>
    private static string DeriveWarrantRule(string caseNumber) =>
        caseNumber.Length >= 5 && caseNumber.Substring(2, 3).Equals("CRB", StringComparison.OrdinalIgnoreCase)
            ? "Criminal Rule 4"
            : "Traffic Rule 7";
}
