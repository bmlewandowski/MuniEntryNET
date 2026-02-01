using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;

namespace Munientry.Poc.Client.Shared
{
    public class ApiHelper
    {
        private readonly IConfiguration _config;
        private readonly NavigationManager _nav;
        public ApiHelper(IConfiguration config, NavigationManager nav)
        {
            _config = config;
            _nav = nav;
        }
        public string GetApiBaseUrl()
        {
            var url = _config["ApiBaseUrl"];
            return string.IsNullOrEmpty(url) ? _nav.BaseUri : url;
        }
    }
}
