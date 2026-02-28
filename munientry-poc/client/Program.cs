using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Munientry.Poc.Client.Shared;
// Bring the generated App component into scope
using Munientry.Poc.Client;

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

await builder.Build().RunAsync();
