using System.Net.Http.Json;
using Munientry.Client.Shared.Models;

namespace Munientry.Client.Services
{
    public class ProbationViolationBondService
    {
        private readonly HttpClient _httpClient;
        public ProbationViolationBondService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> SubmitProbationViolationBondAsync(ProbationViolationBondDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/v1/probationviolationbond", dto);
            return response.IsSuccessStatusCode;
        }
    }
}
