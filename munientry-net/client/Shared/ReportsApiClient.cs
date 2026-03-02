using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Munientry.Shared.Dtos;

namespace Munientry.Client.Shared
{
    /// <summary>
    /// Client-side service for all report and docket API calls.
    ///
    /// Endpoints consumed:
    ///   GET /api/v1/case/docket/{caseNumber}            → GetCaseDocketAsync
    ///   GET /api/v1/reports/not-guilty/{date}           → GetNotGuiltyReportAsync
    ///   GET /api/v1/reports/events/{eventCode}/{date}   → GetEventReportAsync
    ///   GET /api/v1/reports/fta/{date}                  → GetFtaReportAsync
    ///   GET /api/v1/reports/fta/entry/{caseNumber}/{date} → DownloadFtaEntryAsync
    ///   GET /api/v1/reports/fta/batch/{date}            → DownloadFtaBatchAsync
    ///
    /// Legacy Python equivalents:
    ///   CrimCaseDocket.get_docket()    in crim_getters.py
    ///   run_not_guilty_report()        in daily_reports.py
    ///   run_event_type_report()        in authoritycourt_reports.py
    ///   run_batch_fta_process() /
    ///   create_fta_entries()           in batch_menu.py
    /// </summary>
    public class ReportsApiClient
    {
        private readonly HttpClient _http;
        private readonly ILogger<ReportsApiClient> _logger;

        public ReportsApiClient(HttpClient http, ILogger<ReportsApiClient> logger)
        {
            _http = http;
            _logger = logger;
        }

        // ── Case Docket ───────────────────────────────────────────────────────

        /// <summary>
        /// Returns chronological docket entries for a case.
        /// Maps [reports].[DMCMuniEntryCaseDocket].
        /// </summary>
        public async Task<List<CaseDocketEntryDto>> GetCaseDocketAsync(string caseNumber)
        {
            try
            {
                var result = await _http.GetFromJsonAsync<List<CaseDocketEntryDto>>(
                    $"case/docket/{Uri.EscapeDataString(caseNumber)}");
                return result ?? new List<CaseDocketEntryDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ReportsApiClient.GetCaseDocketAsync failed for case {CaseNumber}.", caseNumber);
                return new List<CaseDocketEntryDto>();
            }
        }

        // ── Not Guilty Report ─────────────────────────────────────────────────

        /// <summary>
        /// Returns cases with a Not Guilty plea or continuance journal entry for the given date.
        /// Maps [reports].[DMCMuniEntryNotGuiltyReport].
        /// Columns: Case Number | Defendant Name | Docket Entry
        /// </summary>
        public async Task<List<NotGuiltyReportResultDto>> GetNotGuiltyReportAsync(DateOnly date)
        {
            try
            {
                var result = await _http.GetFromJsonAsync<List<NotGuiltyReportResultDto>>(
                    $"reports/not-guilty/{date:yyyy-MM-dd}");
                return result ?? new List<NotGuiltyReportResultDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ReportsApiClient.GetNotGuiltyReportAsync failed for {Date}.", date);
                return new List<NotGuiltyReportResultDto>();
            }
        }

        // ── Event Report ──────────────────────────────────────────────────────

        /// <summary>
        /// Returns cases scheduled for a specific event type on a given date.
        /// Maps [reports].[DMCMuniEntryEventReport].
        /// eventCode is passed directly to @EventCode — confirm valid values with the DBA.
        /// Columns: Time | Case Number | Defendant Name | Primary Charge | Attorney | Comments
        /// </summary>
        public async Task<List<EventReportResultDto>> GetEventReportAsync(string eventCode, DateOnly date)
        {
            try
            {
                var result = await _http.GetFromJsonAsync<List<EventReportResultDto>>(
                    $"reports/events/{Uri.EscapeDataString(eventCode)}/{date:yyyy-MM-dd}");
                return result ?? new List<EventReportResultDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ReportsApiClient.GetEventReportAsync failed for {EventCode} on {Date}.", eventCode, date);
                return new List<EventReportResultDto>();
            }
        }

        // ── FTA Report ────────────────────────────────────────────────────────

        /// <summary>
        /// Returns FTA-eligible case numbers for the given arraignment date.
        /// Maps [reports].[DMCMuniEntryFTAReport].
        /// Use the list to show a preview before calling DownloadFtaBatchAsync.
        /// </summary>
        public async Task<List<FtaReportResultDto>> GetFtaReportAsync(DateOnly date)
        {
            try
            {
                var result = await _http.GetFromJsonAsync<List<FtaReportResultDto>>(
                    $"reports/fta/{date:yyyy-MM-dd}");
                return result ?? new List<FtaReportResultDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ReportsApiClient.GetFtaReportAsync failed for {Date}.", date);
                return new List<FtaReportResultDto>();
            }
        }

        // ── FTA DOCX Download ─────────────────────────────────────────────────

        /// <summary>
        /// Downloads a single FTA warrant DOCX for one case.
        /// Replicates create_entry() in batch_menu.py (one file).
        /// </summary>
        public async Task<(bool Success, string? Error)> DownloadFtaEntryAsync(
            string caseNumber, DateOnly date, IJSRuntime js)
        {
            try
            {
                var response = await _http.GetAsync(
                    $"reports/fta/entry/{Uri.EscapeDataString(caseNumber)}/{date:yyyy-MM-dd}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = await TryReadProblemDetailAsync(response);
                    return (false, errorMessage);
                }

                var bytes = await response.Content.ReadAsByteArrayAsync();
                var fileName = $"{caseNumber}_FTA_Arraignment.docx";
                await js.InvokeVoidAsync("downloadFile", fileName,
                    "application/vnd.openxmlformats-officedocument.wordprocessingml.document", bytes);
                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ReportsApiClient.DownloadFtaEntryAsync failed for case {CaseNumber}.", caseNumber);
                return (false, "An unexpected error occurred. Please try again.");
            }
        }

        /// <summary>
        /// Downloads all FTA warrant DOCXs for the date as a single ZIP.
        /// Replicates run_batch_fta_process() / create_fta_entries() in batch_menu.py.
        /// </summary>
        public async Task<(bool Success, string? Error)> DownloadFtaBatchAsync(
            DateOnly date, IJSRuntime js)
        {
            try
            {
                var response = await _http.GetAsync($"reports/fta/batch/{date:yyyy-MM-dd}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = await TryReadProblemDetailAsync(response);
                    return (false, errorMessage);
                }

                var bytes = await response.Content.ReadAsByteArrayAsync();
                var fileName = $"FTA_Batch_{date:yyyyMMdd}.zip";
                await js.InvokeVoidAsync("downloadFile", fileName, "application/zip", bytes);
                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ReportsApiClient.DownloadFtaBatchAsync failed for {Date}.", date);
                return (false, "An unexpected error occurred. Please try again.");
            }
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        /// <summary>
        /// Reads the RFC 7807 Problem Details <c>detail</c> field from a non-success response.
        /// Falls back to a generic message if the body is absent or not valid JSON.
        /// Never surfaces raw server internals to the UI.
        /// </summary>
        private static async Task<string> TryReadProblemDetailAsync(HttpResponseMessage response)
        {
            try
            {
                var body = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(body))
                {
                    using var doc = JsonDocument.Parse(body);
                    if (doc.RootElement.TryGetProperty("detail", out var detail))
                        return detail.GetString() ?? "An error occurred. Please try again.";
                }
            }
            catch (Exception)
            {
                // Ignore parse failures — fall through to the generic message.
            }
            return "An error occurred. Please try again.";
        }

    }
}
