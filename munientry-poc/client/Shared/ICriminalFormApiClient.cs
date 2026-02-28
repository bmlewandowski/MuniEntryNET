using System.Net.Http;
using System.Threading.Tasks;

namespace Munientry.Poc.Client.Shared
{
    public interface ICriminalFormApiClient
    {
        Task<HttpResponseMessage> PostAsync<TDto>(string endpoint, TDto dto);
    }
}
