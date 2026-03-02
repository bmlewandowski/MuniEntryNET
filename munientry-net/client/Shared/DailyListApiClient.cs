using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Munientry.Shared.Dtos;

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
        private readonly HttpClient _http;
        private readonly ILogger<DailyListApiClient> _logger;

        public DailyListApiClient(HttpClient http, ILogger<DailyListApiClient> logger)
        {
            _http = http;
            _logger = logger;
        }

        public async Task<List<DailyListResultDto>> GetDailyListAsync(string listType, DateOnly date)
        {
            try
            {
                var result = await _http.GetFromJsonAsync<List<DailyListResultDto>>(
                    $"dailylist/{listType}/{date:yyyy-MM-dd}");
                return result ?? new List<DailyListResultDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DailyListApiClient.GetDailyListAsync failed for {ListType} on {Date}.", listType, date);
                return new List<DailyListResultDto>();
            }
        }
    }
}
