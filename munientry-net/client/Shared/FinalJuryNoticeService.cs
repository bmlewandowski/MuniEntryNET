using munientry_poc.client.Shared.Models;
using System.Net.Http.Json;

namespace munientry_poc.client.Shared
{
    public class FinalJuryNoticeService
    {
        private readonly HttpClient _httpClient;
        public FinalJuryNoticeService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> CreateFinalJuryNoticeAsync(FinalJuryNoticeDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/finaljurynotice", dto);
            return response.IsSuccessStatusCode;
        }
    }
}
