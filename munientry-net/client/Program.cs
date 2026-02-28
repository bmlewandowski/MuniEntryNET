using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Munientry.Poc.Client.Shared;
// Bring the generated App component into scope
using Munientry.Poc.Client;

// ENTRA ID - Step 2: Uncomment the using directive below.
// using Microsoft.Authentication.WebAssembly.Msal;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.Services.AddScoped<ApiHelper>();
builder.Services.AddScoped<DenyPrivilegesPermitRetestService>();
builder.Services.AddScoped<CaseSearchService>();

// Load config for API base URL
var config = new ConfigurationBuilder()
    .AddJsonFile("wwwroot/appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile("wwwroot/appsettings.Docker.json", optional: true, reloadOnChange: true)
    .Build();
builder.Services.AddSingleton<IConfiguration>(config);

// Shared HttpClient for direct usage (e.g. forms that inject HttpClient Http)
builder.Services.AddScoped(sp => new HttpClient());

// Typed API client used by FormPageBase<T> for form submissions.
// Base address is resolved at runtime so Docker / local environments are handled correctly.
builder.Services.AddScoped<ICriminalFormApiClient>(sp =>
{
    var helper = sp.GetRequiredService<ApiHelper>();
    var http = new HttpClient { BaseAddress = new Uri(helper.GetApiBaseUrl()) };
    return new CriminalFormApiClient(http);
});

// =====================================================================
// ENTRA ID AUTHENTICATION — cityofdelawareoh.gov accounts (@cityofdelawareoh.gov)
//
// To enable:
//   Step 1: Uncomment the MSAL package reference in Munientry.Poc.Client.csproj
//   Step 2: Uncomment the 'using Microsoft.Authentication.WebAssembly.Msal' line above
//   Step 3: Fill in TenantId and ClientId in wwwroot/appsettings.json (and appsettings.Docker.json)
//           - TenantId:  found in Entra ID > Overview in Azure Portal
//           - ClientId:  from the App Registration created for this Blazor WASM app
//           - In the App Registration, set the redirect URI to: https://<your-host>/authentication/login-callback
//             and ensure "Single-page application (SPA)" platform is selected.
//   Step 4: Uncomment the AddMsalAuthentication block below
//   Step 5: In App.razor, swap RouteView for AuthorizeRouteView (see instructions in that file)
//   Step 6: Add [Authorize] to any page you want to restrict (e.g. @attribute [Authorize])
// =====================================================================

// builder.Services.AddMsalAuthentication(options =>
// {
//     builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
//     // Restrict sign-in to @cityofdelawareoh.gov accounts only.
//     options.ProviderOptions.Authentication.LoginMode = "redirect";
//     options.ProviderOptions.DefaultAccessTokenScopes.Add("openid");
//     options.ProviderOptions.DefaultAccessTokenScopes.Add("profile");
//     options.ProviderOptions.DefaultAccessTokenScopes.Add("email");
// });

await builder.Build().RunAsync();
