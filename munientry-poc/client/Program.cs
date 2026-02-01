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

// Load config for API base URL
var config = new ConfigurationBuilder()
    .AddJsonFile("wwwroot/appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile("wwwroot/appsettings.Docker.json", optional: true, reloadOnChange: true)
    .Build();
builder.Services.AddSingleton<IConfiguration>(config);

builder.Services.AddScoped(sp => new HttpClient());

await builder.Build().RunAsync();
