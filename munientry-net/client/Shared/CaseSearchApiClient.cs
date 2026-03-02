using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Munientry.Shared.Dtos;

namespace Munientry.Client.Shared
{
    /// <summary>
    /// Client-side service for calling GET /api/case/search/{caseNumber}.
    /// Invokes the [reports].[DMCMuniEntryCaseSearch] stored procedure via the API
    /// to pre-populate criminal entry dialog fields — equivalent to the Python
    /// BaseCmsLoader / CrimCmsLoader pattern in munientry/loaders/cms_case_loaders.py.
    /// </summary>
    public class CaseSearchApiClient
    {
        private readonly HttpClient _http;
        private readonly ILogger<CaseSearchApiClient> _logger;

        public CaseSearchApiClient(HttpClient http, ILogger<CaseSearchApiClient> logger)
        {
            _http = http;
            _logger = logger;
        }

        /// <summary>
        /// Returns one row per charge on the case.
        /// Defendant name, defense counsel, and FRA status are consistent across all rows —
        /// use the first row for those fields and iterate all rows for the charges list.
        /// Returns an empty list if the case is not found or the call fails.
        /// </summary>
        public async Task<List<CaseSearchResultDto>> SearchCaseAsync(string caseNumber)
        {
            try
            {
                var result = await _http.GetFromJsonAsync<List<CaseSearchResultDto>>(
                    $"case/search/{Uri.EscapeDataString(caseNumber)}");
                return result ?? new List<CaseSearchResultDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CaseSearchApiClient.SearchCaseAsync failed for case {CaseNumber}.", caseNumber);
                return new List<CaseSearchResultDto>();
            }
        }
    }
}
