using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Munientry.Client.Shared
{
    public class CriminalFormApiClient : ICriminalFormApiClient
    {
        private readonly HttpClient _http;

        public CriminalFormApiClient(HttpClient http) => _http = http;

        public Task<HttpResponseMessage> PostAsync<TDto>(string endpoint, TDto dto)
            => _http.PostAsJsonAsync(endpoint, dto);
    }
}
