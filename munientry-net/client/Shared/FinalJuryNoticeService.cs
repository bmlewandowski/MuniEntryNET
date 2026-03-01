using Munientry.Client.Shared.Models;
using System.Net.Http.Json;

namespace Munientry.Client.Shared
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
            var response = await _httpClient.PostAsJsonAsync("/api/v1/finaljurynotice", dto);
            return response.IsSuccessStatusCode;
        }
    }
}
