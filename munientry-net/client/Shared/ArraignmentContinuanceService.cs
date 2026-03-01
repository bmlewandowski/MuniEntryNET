using Munientry.Client.Shared.Models;
using System.Net.Http.Json;

namespace Munientry.Client.Shared
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
            var response = await _httpClient.PostAsJsonAsync("/api/v1/arraignmentcontinuance", dto);
            return response.IsSuccessStatusCode;
        }
    }
}
