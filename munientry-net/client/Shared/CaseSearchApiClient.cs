using System.Net.Http.Json;
using Munientry.Client.Shared.Models;

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
        private readonly ApiHelper _apiHelper;

        public CaseSearchApiClient(ApiHelper apiHelper)
        {
            _apiHelper = apiHelper;
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
                var apiBase = _apiHelper.GetApiBaseUrl();
                using var http = new HttpClient { BaseAddress = new Uri(apiBase) };
                var result = await http.GetFromJsonAsync<List<CaseSearchResultDto>>(
                    $"case/search/{Uri.EscapeDataString(caseNumber)}");;
                return result ?? new List<CaseSearchResultDto>();
            }
            catch
            {
                return new List<CaseSearchResultDto>();
            }
        }
    }
}
