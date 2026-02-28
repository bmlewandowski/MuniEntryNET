using munientry_poc.client.Shared.Models;
using System.Net.Http.Json;

namespace munientry_poc.client.Shared
{
    public class ArraignmentContinuanceService
    {
        private readonly HttpClient _httpClient;
        public ArraignmentContinuanceService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> CreateArraignmentContinuanceAsync(ArraignmentContinuanceDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/arraignmentcontinuance", dto);
            return response.IsSuccessStatusCode;
        }
    }
}
