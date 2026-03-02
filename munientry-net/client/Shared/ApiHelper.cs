using Microsoft.Extensions.Configuration;

namespace Munientry.Client.Shared
{
    public class ApiHelper
    {
        private readonly IConfiguration _config;

        public ApiHelper(IConfiguration config)
        {
            _config = config;
        }

        /// <summary>
        /// Returns the API base URL from configuration.
        /// Throws <see cref="InvalidOperationException"/> if ApiBaseUrl is not set —
        /// a misconfigured environment is caught at startup rather than producing a
        /// silent wrong-host 404 during a form submission.
        /// </summary>
        public string GetApiBaseUrl()
        {
            var url = _config["ApiBaseUrl"];
            if (string.IsNullOrEmpty(url))
                throw new InvalidOperationException(
                    "ApiBaseUrl is not configured. Ensure wwwroot/appsettings.json (or " +
                    "appsettings.Docker.json) contains a non-empty 'ApiBaseUrl' value.");
            return url;
        }
    }
}
