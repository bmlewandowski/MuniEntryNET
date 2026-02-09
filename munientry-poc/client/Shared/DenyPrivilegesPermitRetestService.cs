using System.Net.Http.Json;
using Munientry.Poc.Client.Shared.Models;

namespace Munientry.Poc.Client.Shared
{
    public class DenyPrivilegesPermitRetestService
    {
        private readonly HttpClient _http;
        public DenyPrivilegesPermitRetestService(HttpClient http)
        {
            _http = http;
        }
        public async Task SubmitAsync(DenyPrivilegesPermitRetestDto dto)
        {
            await _http.PostAsJsonAsync("/api/denyprivilegespermitretest", dto);
        }
    }
}
