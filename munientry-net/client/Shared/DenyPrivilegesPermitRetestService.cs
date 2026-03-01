using System.Net.Http.Json;
using Munientry.Client.Shared.Models;

namespace Munientry.Client.Shared
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
            await _http.PostAsJsonAsync("/api/v1/denyprivilegespermitretest", dto);
        }
    }
}
