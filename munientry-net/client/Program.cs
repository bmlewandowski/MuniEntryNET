using FluentValidation;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using Munientry.Client.Shared;
using Munientry.Client.Shared.Services;
using Munientry.Shared.Validation;
// Bring the generated App component into scope
using Munientry.Client;

// ENTRA ID - Step 2: Uncomment the using directive below.
// using Microsoft.Authentication.WebAssembly.Msal;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
// appsettings.json is loaded automatically by CreateDefault from wwwroot/.
// Load Docker environment overrides when present (also served from wwwroot/).
builder.Configuration.AddJsonFile("appsettings.Docker.json", optional: true);
builder.RootComponents.Add<App>("#app");
builder.Services.AddScoped<ApiHelper>();

// All four API clients are registered as typed HTTP clients so that the underlying
// BrowserHttpHandler (Blazor WASM) / SocketsHttpHandler (server) is managed by
// IHttpClientFactory rather than a new HttpClient being allocated on every API call.
// BaseAddress is resolved once from configuration when the client is first constructed.
builder.Services.AddHttpClient<CaseSearchApiClient>((sp, client) =>
    client.BaseAddress = new Uri(sp.GetRequiredService<ApiHelper>().GetApiBaseUrl()));
builder.Services.AddHttpClient<DailyListApiClient>((sp, client) =>
    client.BaseAddress = new Uri(sp.GetRequiredService<ApiHelper>().GetApiBaseUrl()));
builder.Services.AddHttpClient<ReportsApiClient>((sp, client) =>
    client.BaseAddress = new Uri(sp.GetRequiredService<ApiHelper>().GetApiBaseUrl()));
builder.Services.AddHttpClient<IEntryFormApiClient, EntryFormApiClient>((sp, client) =>
    client.BaseAddress = new Uri(sp.GetRequiredService<ApiHelper>().GetApiBaseUrl()));

// Shared HttpClient for direct usage (e.g. forms that inject HttpClient Http)
builder.Services.AddScoped(sp => new HttpClient());

// Register FluentValidation validators from the shared assembly so
// <FluentValidationValidator /> in each EditForm can enforce rules client-side.
// These are the same validators the API uses server-side (FluentValidationFilter),
// meaning validation rules are defined and enforced in exactly one place.
builder.Services.AddValidatorsFromAssemblyContaining<NotGuiltyPleaValidator>(lifetime: ServiceLifetime.Singleton);

// =====================================================================
// JUDICIAL OFFICER RESOLUTION
//
// JudicialOfficerSession is scoped (one per browser session) and holds
// the resolved JudicialOfficer — equivalent to mainwindow.judicial_officer
// in the legacy Python app.
//
// During development / testing (no Entra ID):
//   MockJudicialOfficerProvider reads "MockUser" from appsettings.json
//   and maps it to a JudicialOfficer from the Staff:JudicialOfficers section.
//   Change "MockUser" to "judge_2", "magistrate_1", "visiting_judge", etc.
//
// When Entra ID is enabled (follow the ENTRA ID steps above):
//   Comment out the MockJudicialOfficerProvider line below and uncomment
//   the EntraIdJudicialOfficerProvider line instead.
// =====================================================================
builder.Services.AddScoped<JudicialOfficerSession>();
builder.Services.AddScoped<IJudicialOfficerProvider, MockJudicialOfficerProvider>();
// ENTRA ID - Step 7: Replace the line above with the one below.
// builder.Services.AddScoped<IJudicialOfficerProvider, EntraIdJudicialOfficerProvider>();

// =====================================================================
// ENTRA ID AUTHENTICATION — cityofdelawareoh.gov accounts (@cityofdelawareoh.gov)
//
// To enable:
//   Step 1: Uncomment the MSAL package reference in Munientry.Client.csproj
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
