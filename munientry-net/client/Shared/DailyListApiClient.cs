using System.Net.Http.Json;
using Munientry.Client.Shared.Models;

namespace Munientry.Client.Shared
{
    /// <summary>
    /// Client-side service for calling GET /api/dailylist/{listType}/{date}.
    /// Invokes one of the 6 daily case list stored procedures via the API —
    /// equivalent to the Python get_daily_case_list() / DAILY_CASE_LIST_STORED_PROCS
    /// pattern in munientry/sqlserver/crim_getters.py.
    ///
    /// Valid listType values: arraignments, slated, pleas, pcvh_fcvh, final_pretrial, trials_to_court
    /// </summary>
    public class DailyListApiClient
    {
        private readonly ApiHelper _apiHelper;

        public DailyListApiClient(ApiHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public async Task<List<DailyListResultDto>> GetDailyListAsync(string listType, DateOnly date)
        {
            try
            {
                var apiBase = _apiHelper.GetApiBaseUrl();
                using var http = new HttpClient { BaseAddress = new Uri(apiBase) };
                var result = await http.GetFromJsonAsync<List<DailyListResultDto>>(
                    $"dailylist/{listType}/{date:yyyy-MM-dd}");;
                return result ?? new List<DailyListResultDto>();
            }
            catch
            {
                return new List<DailyListResultDto>();
            }
        }
    }
}
