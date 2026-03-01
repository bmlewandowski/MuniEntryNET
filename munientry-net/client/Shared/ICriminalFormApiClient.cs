using System.Net.Http;
using System.Threading.Tasks;

namespace Munientry.Client.Shared
{
    public interface ICriminalFormApiClient
    {
        Task<HttpResponseMessage> PostAsync<TDto>(string endpoint, TDto dto);
    }
}
