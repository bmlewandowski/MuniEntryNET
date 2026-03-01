using Munientry.Client.Shared.Models;
using System.Net.Http.Json;

namespace Munientry.Client.Shared
{
    public class SchedulingEntryService
    {
        private readonly HttpClient _httpClient;
        public SchedulingEntryService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> CreateSchedulingEntryAsync(SchedulingEntryDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/v1/schedulingentry", dto);
            return response.IsSuccessStatusCode;
        }
    }
}
