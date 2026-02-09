using munientry_poc.client.Shared.Models;
using System.Net.Http.Json;

namespace munientry_poc.client.Shared
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
            var response = await _httpClient.PostAsJsonAsync("/api/schedulingentry", dto);
            return response.IsSuccessStatusCode;
        }
    }
}
