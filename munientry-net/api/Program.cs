using FluentValidation;
using Munientry.Api;
using Munientry.Api.Endpoints;
using Munientry.Api.Middleware;
using Munientry.Api.Validation;
using Munientry.Shared.Validation;
// ENTRA ID - Step 2: Uncomment the using directive below.
// using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddJsonConsole(opts =>
{
    opts.IncludeScopes = true;
    opts.TimestampFormat = "o";
});
builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        // TODO (security item #3): restrict to the cluster's client hostname before go-live.
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

// =====================================================================
// ENTRA ID AUTHENTICATION — cityofdelawareoh.gov accounts
//
// To enable:
//   Step 1: Uncomment Microsoft.Identity.Web in Munientry.Api.csproj and run dotnet restore
//   Step 2: Uncomment 'using Microsoft.Identity.Web;' at the top of this file
//   Step 3: Fill in TenantId, ClientId, and Audience in appsettings.json (AzureAd section)
//   Step 4: Uncomment the AddAuthentication block below
//   Step 5: Uncomment UseAuthentication() and UseAuthorization() in the middleware pipeline below
//
// The fallback policy approach used here protects every endpoint automatically —
// no [Authorize] attribute or .RequireAuthorization() call needed on individual endpoints.
// =====================================================================

// builder.Services.AddAuthentication()
//     .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));
// builder.Services.AddAuthorizationBuilder()
//     .SetFallbackPolicy(new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
//         .RequireAuthenticatedUser()
//         .Build());

builder.Services.AddHealthChecks();
builder.Services.AddMuniEntryServices(builder.Configuration);
// Register all IValidator<T> implementations discovered in the Munientry.Shared assembly.
// Validators live in shared/Validation/FormValidators.cs so the Blazor client can reference
// the same rules for client-side validation via Blazored.FluentValidation.
builder.Services.AddValidatorsFromAssemblyContaining<NotGuiltyPleaValidator>(lifetime: ServiceLifetime.Singleton);

var app = builder.Build();
// UseHostFiltering must precede UseCors so the AllowedHosts setting in
// appsettings.json actually takes effect (security item #7).
app.UseHostFiltering();
app.UseCors();
app.UseExceptionHandler();
app.UseMiddleware<AuditMiddleware>();
app.MapHealthChecks("/healthz");
// ENTRA ID - Step 5: Uncomment these two lines (order matters: Authentication before Authorization).
// app.UseAuthentication();
// app.UseAuthorization();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // available at /openapi/v1.json
}

// ── Versioned endpoint groups ─────────────────────────────────────────────────
// All API routes are prefixed /api/v1/ via the route group below.
// Each file in api/Endpoints/ owns one domain area; routes inside those files
// omit the /api prefix (e.g. "/case/search/{id}") because the group supplies it.
// To add a new endpoint: open the relevant Endpoints/ file and add a MapXxx call.
// To add a new version: duplicate the group block with a new prefix ("/api/v2").
var v1 = app.MapGroup("/api/v1").AddEndpointFilter<FluentValidationFilter>();
v1.MapCaseDataEndpoints();
v1.MapDrivingEndpoints();
v1.MapCriminalPleaEndpoints();
v1.MapLeapEndpoints();
v1.MapDiversionEndpoints();
v1.MapBondEndpoints();
v1.MapProbationEndpoints();
v1.MapNoticesAndSchedulingEndpoints();
v1.MapCivilEndpoints();
v1.MapAdminEndpoints();
v1.MapReportsEndpoints();

app.Run();

public partial class Program { }

