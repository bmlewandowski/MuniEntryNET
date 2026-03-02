# MuniEntry .NET — Security, Dependency & Architecture Review

> **Date:** March 1, 2026  
> **Scope:** Blazor WASM client + ASP.NET Core minimal API, isolated from the legacy Python application.  
> **Deployment target:** Local city government private Kubernetes cluster, Microsoft Entra ID authentication, SQL Server via stored procedures.

---

## Security

### Critical — Not Ready for Production

**1. Authentication is fully disabled.**  
The Entra ID wiring is commented out at every layer — the MSAL NuGet reference in `Munientry.Client.csproj`, the `using` directive and `AddMsalAuthentication` block in `client/Program.cs`, and `AuthorizeRouteView` in `App.razor`. Every form and every route is publicly accessible today.

**2. The API has zero authorization middleware — scaffolding now in place.**  
Even after the client enables MSAL and users receive tokens, the API must validate the bearer token independently. The full auth wiring has been added to the API in commented-out form, mirroring the client's prepared-but-disabled pattern:

- `Munientry.Api.csproj` — commented-out `Microsoft.Identity.Web` package reference (Step 1)
- `api/Program.cs` — commented-out `using Microsoft.Identity.Web;`, `AddAuthentication().AddMicrosoftIdentityWebApi(...)`, a fallback authorization policy (`RequireAuthenticatedUser` — protects all endpoints without needing `[Authorize]` on each one), `app.UseAuthentication()`, and `app.UseAuthorization()` (Steps 2, 4, 5)
- `api/appsettings.json` — commented-out `AzureAd` config block with `TenantId`, `ClientId`, and `Audience` placeholders and instructions for the Azure Portal (Step 3)

To enable: uncomment Steps 1–5 in order, fill in the `AzureAd` values, and run `dotnet restore`. Nothing else in the codebase needs to change — the fallback policy enforces authentication globally. **This item remains a blocker: the API is still fully unguarded until the scaffolding is uncommented.**

**3. CORS is fully open.**  
```csharp
policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()
```
Any origin on any network can call the API when it is exposed. In a Kubernetes ingress environment this must be scoped to the specific client hostname.

**4. `ASPNETCORE_ENVIRONMENT=Development` in docker-compose — health check blocker resolved.**  
This causes the OpenAPI schema (`/openapi/v1.json`) to be served — a full machine-readable map of every endpoint and DTO — on a production-bound cluster. The previous blocker (K8s probes depending on `/openapi/v1.json`) is now resolved (see item 17). The remaining step is a one-line change in `k8s/configmap.yaml`: set `ASPNETCORE_ENVIRONMENT` to `Production`. `docker-compose.yml` intentionally stays as `Development` for local work.

**5. ~~Raw exception messages surface to clients~~ — RESOLVED.**  
`GlobalExceptionHandler` (`api/Middleware/GlobalExceptionHandler.cs`) implements `IExceptionHandler` and is registered via `services.AddExceptionHandler<GlobalExceptionHandler>()` + `app.UseExceptionHandler()` in the middleware pipeline. It logs the full exception (type, message, stack trace) with the correlation `TraceId` to the server-side log, then returns a sanitized RFC 7807 Problem Details response containing only `title: "An error occurred processing your request."` and `detail: "Contact support with trace ID: {traceId}"`. All no-op `catch(Exception) { throw; }` blocks across the 9 endpoint files have been fully removed — DOCX generation now lives entirely in service classes, and any exception propagates automatically to the global handler. No internal paths, messages, or types reach the client.

**6. `TrustServerCertificate=True` in `appsettings.json`.**  
SQL Server TLS validation is skipped. On a private K8s cluster with an internal CA this is a specific, configurable risk, but it must be a deliberate decision and documented — not a leftover dev shortcut.

**7. `AllowedHosts: "*"` in `appsettings.json`.**  
Allows HOST header injection. Must be locked to the cluster's FQDNs before deployment.

**8. Secrets in `.env` and `appsettings.json`.**  
The docker-compose accepts `ENTRA_CLIENT_SECRET` and `AUTHORITY_COURT_CONNSTR` from a `.env` file with no enforcement that the `.env` is not committed. There is no reference to Kubernetes Secrets, a secrets CSI driver, or Azure Key Vault. For a government deployment, connection strings and OAuth secrets belong in K8s Secrets (sealed with Sealed Secrets or synced from a vault) — not in files on disk.

**9. Court data is PII with compliance exposure.**  
Defendant names, case numbers, charge details, and offense dates flow through unencrypted HTTP (`http://api/api/v1/` in `appsettings.Docker.json`) within the cluster. East-west traffic between the nginx pod and the API pod is in plaintext. For a criminal court system this likely has Ohio Public Records law and potentially CJIS policy implications.

---

## Dependencies

**10. ~~Namespace inconsistency~~ — RESOLVED.**  
All namespaces have been unified to `Munientry.Api.*` (API/data), `Munientry.Client.*` (Blazor client), and `Munientry.Api.Tests` (test project). Project files renamed to `Munientry.Api.csproj` and `Munientry.Client.csproj`. The DI registrations in `api/Program.cs` now use a single consistent namespace root.

**11. ~~Most API services have no interface~~ — RESOLVED.**  
All 33 services now have a matching `IXxxService` interface in `api/Services/` (28 original + 5 new: `ICriminalSealingEntryService`, `ICompetencyEvaluationService`, `IFailureToAppearService`, `ICriminalFreeformEntryService`, `ITrialSentencingService`). Each interface file declares exactly the public method(s) of its implementation. All 28 classes implement their interface (`: IXxxService`). `ServiceRegistration.cs` registers every service as an interface→implementation pair (`AddScoped<IXxx, Xxx>()`). The three endpoint lambda parameters that injected concrete types (`DrivingPrivilegesService`, `TrialToCourtNoticeService`, `TimeToPayOrderService`) have been updated to inject their interfaces. The inline `ILeapAdmissionPleaService` declaration that was embedded inside `LeapAdmissionPleaService.cs` has been extracted to its own file. The `IFiscalJournalEntryService` declaration that was also inlined inside `FiscalJournalEntryService.cs` has likewise been extracted to `IFiscalJournalEntryService.cs`. All services are now mockable in unit tests without a database.

**12. ~~`net9.0` is Standard Term Support (EOL May 2026)~~ — RESOLVED.**  
All three projects (`Munientry.Api`, `Munientry.Client`, `api.Tests`) and both Dockerfiles have been upgraded to `net10.0` (LTS). ASP.NET Core / Blazor package references (`Microsoft.AspNetCore.OpenApi`, `Microsoft.AspNetCore.Components.WebAssembly`, `Microsoft.Authentication.WebAssembly.Msal`, `Microsoft.AspNetCore.Mvc.Testing`) have been updated to `10.0.0`. The former `Munientry.DocxTemplating` library project has been collapsed into the API — `DocxTemplateProcessor.cs` now lives directly in `api/` under the `Munientry.Api` namespace.

**13. Compression is disabled for an nginx limitation.**  
`BlazorEnableCompression=false` in the `.csproj` was a workaround for the K8s/nginx pod not having the Brotli module. In a production K8s cluster, TLS termination and gzip are handled at the ingress controller level (nginx-ingress or Traefik). This should be re-evaluated; a 10MB+ uncompressed WASM payload is a real load-time problem for court users on constrained city hall networks.

**14. The MSAL NuGet dependency is commented out, not absent.**  
If the package reference line in the `.csproj` is accidentally left commented through a CI pipeline that does `dotnet restore`, the auth code that references it will cause a build failure at the worst time. Resolve the enable/disable decision and commit the final state.

---

## Architecture

**15. ~~`Program.cs` in the API is 1,389 lines of inline endpoint handlers~~ — RESOLVED.**  
Decomposed into `api/ServiceRegistration.cs` (all DI registrations, called via `builder.Services.AddMuniEntryServices()`) and 9 static extension-method files under `api/Endpoints/`, each owning one domain area (`CaseData`, `Driving`, `CriminalPlea`, `Leap`, `Diversion`, `Bond`, `Probation`, `NoticesAndScheduling`, `Civil`, `Admin`). `Program.cs` is now 36 lines. To add a new endpoint: open the relevant `Endpoints/` file and add a `MapPost`/`MapGet` call. To add a new service: add its registration to `ServiceRegistration.cs`.

**16. ~~DB write logic scaffolding removed — system is permanently read-only~~ — RESOLVED.**  
Earlier scaffolding contained commented-out lines such as:
```csharp
// service.InsertDrivingPrivileges(dto);
// --- End DB Save Logic ---
```
These have been removed. The client confirmed: **the legacy system had read-only access to SQL Server through stored procedures and some hard-coded queries — there were no writes.** The new Blazor application follows the same design. No write endpoints, write services, or write stored procedures will be introduced unless this design decision is explicitly revisited. The `AuthorityCourt` database connection is treated as read-only at the application layer; unit-of-work, transaction management, and rollback patterns are not applicable.

> **Post-review correction (March 2, 2026):** `FiscalJournalEntryService` was discovered to contain a live `INSERT INTO FiscalJournalEntries` statement — a write that survived the scaffolding removal pass. This has been fixed: the service now inherits `DocxServiceBase<FiscalJournalEntryDto>`, fills `Admin_Fiscal_Template.docx` (matching the legacy `AdminFiscalEntryCreator`), and returns DOCX bytes. The `POST /api/v1/fiscaljournalentry` endpoint now returns a DOCX file, consistent with all other entry endpoints. No database writes remain anywhere in the codebase.

**17. ~~No health/readiness endpoints exist~~ — RESOLVED.**  
`builder.Services.AddHealthChecks()` and `app.MapHealthChecks("/healthz")` have been added to `api/Program.cs` (no environment guard — available in both Development and Production). Both the `readinessProbe` and `livenessProbe` in `k8s/api/deployment.yaml` have been updated from `/openapi/v1.json` to `/healthz`. The temporary workaround comment has been removed. Item 4 (`ASPNETCORE_ENVIRONMENT`) can now be switched to `Production` in `k8s/configmap.yaml` without taking the pod down. A SQL Server ping check (`AddSqlServer(...)`) should be added to the health check registration when the connection string is stable in the cluster.

**18. ~~No Kubernetes manifests exist~~ — RESOLVED.**  
A full manifest set has been added under `munientry-net/k8s/` and is managed via Kustomize (`kubectl apply -k k8s/`). Includes: `Namespace`, `ConfigMap`, `Secret` template, `Deployment` + `Service` for both the API and Blazor client, and an nginx `Ingress` with path-based routing (`/api/v1/*` → API, `/*` → client). Resource `requests`/`limits` are set on both deployments. Remaining open items from the original list:
- TLS termination at the Ingress is not yet configured (cert-manager + internal CA needed before go-live)
- `secret.yaml` is a template only; a secrets management strategy (Sealed Secrets, Azure Key Vault CSI, or External Secrets Operator) has not been chosen
- `HorizontalPodAutoscaler` not yet added — revisit after load profiling of concurrent DOCX generation

**19. Blazor WASM means all security is API-enforced — with no role checks today.**  
WASM runs entirely in the browser; the Razor components are decompilable. The API must enforce row-level authorization (e.g., a clerk should not be able to search cases outside their assigned courtroom) entirely server-side. No role model, claims-based policy, or court staff/judge distinction is modeled anywhere in the codebase yet. This needs to be designed before Entra ID is enabled — not after.

**20. ~~No structured or audit logging~~ — RESOLVED.**  
Audit logging is now implemented without additional NuGet dependencies:

- `api/Middleware/AuditMiddleware.cs` — `IMiddleware` implementation that writes one structured record per request containing `TraceId`, `UserId` (Entra OID claim, falls back to `"anonymous"` until auth is enabled), `Method`, `Path`, `Status`, and `ElapsedMs`. Health-check probes (`/healthz`) are excluded from the audit log.
- `api/ServiceRegistration.cs` — `AuditMiddleware` registered as `AddTransient<AuditMiddleware>()`.
- `api/Program.cs` — `builder.Logging.AddJsonConsole(...)` added (structured NDJSON to stdout, consumed by Fluent Bit / log aggregator in K8s); `app.UseMiddleware<AuditMiddleware>()` inserted after `UseCors()` so every request is covered.
- `api/appsettings.json` — `Default` log level tightened to `Warning`; `Munientry.Api.Middleware` category pinned at `Information` so audit records are always emitted regardless of the default level.

W3C `TraceParent` correlation IDs are supplied automatically by ASP.NET Core per-request via `Activity.Current`. No code change is needed when Entra ID is enabled — `UserId` will populate from the OID claim automatically.

**21. ~~No API versioning strategy~~ — RESOLVED.**  
All API routes are now versioned under `/api/v1/` using ASP.NET Core's `MapGroup("/api/v1")` in `api/Program.cs`. Endpoint files under `api/Endpoints/` register routes without the `/api` prefix (e.g. `"/case/search/{caseNumber}"`) — the version prefix is supplied exclusively by the group, making future `/api/v2` introduction a single new `MapGroup` block. The Blazor client's `ApiBaseUrl` in both `appsettings.json` (`http://localhost:5000/api/v1/`) and `appsettings.Docker.json` (`http://api/api/v1/`) has been updated. All four standalone client services, both `*ApiClient` HTTP wrappers, the four `ApiEndpoint` page overrides that previously hard-coded the `api/` segment, and all 39 API test files have been updated to the versioned paths. The K8s nginx `Ingress` path rule remains `path: /api` (prefix match), which continues to route all current and future version segments to the API service without change.

**22. ~~`CaseSearchService` and `DailyListService` exist in both the client `Shared/` folder and the API `Services/` folder with identical names~~ — RESOLVED.**  
The client-side classes have been renamed `CaseSearchApiClient` and `DailyListApiClient` (files, class names, DI registrations, and all inject sites updated). The API-side `CaseSearchService` / `DailyListService` names are unchanged. The naming now unambiguously signals the role: `*ApiClient` classes are HTTP wrappers in the Blazor client; `*Service` classes are SQL Server implementations in the API.

**24. ~~Redundant per-page form model alias in all Razor pages~~ — RESOLVED.**  
33 Razor form pages each declared a private computed property that simply aliased the `Model` property inherited from `FormPageBase<TDto>` — e.g. `private NotGuiltyPleaDto pleaForm => Model;`, `private JailCcPleaDto jailCcModel => Model;`, etc. Every `@bind-Value` and `<EditForm Model="...">` attribute in the page markup then referenced the alias instead of `Model` directly. The alias was removed from all 33 pages; all markup references were updated to use `Model.` uniformly. The three Admin pages (`JurorPayment`, `TimeToPayOrder`, `WorkflowsFiscal`) that do not inherit `FormPageBase` were left unchanged — they own their own `private XxxDto formModel = new();` field, which is correct. Build: 0 errors, 0 warnings after the change.

**P3. ~~Shared DTO project to avoid client/server drift~~ — RESOLVED.**  
`api/Data/` and `client/Shared/Models/` each duplicated all 45+ form DTOs in separate namespaces (`Munientry.Api.Data` and `Munientry.Client.Shared.Models`). Any property type change on one side would silently diverge from the other — the legacy code already had drift (`bool?` vs `bool` on checkbox fields, a `string?` vs `EntryType` enum for `DenyPrivilegesPermitRetestDto.EntryType`, and missing `[PastDate]`/`[FutureDate]` validation attributes on the API side).

A new `Munientry.Shared` class library project (`shared/Munientry.Shared.csproj`, `net10.0`) now holds the single canonical copy:
- `shared/Dtos/` — 46 merged DTO files under the `Munientry.Shared.Dtos` namespace (45 form DTOs + `EntryType.cs` enum)
- `shared/Validation/` — `PastDateAttribute.cs` and `FutureDateAttribute.cs` under `Munientry.Shared.Validation`

All three projects (`Munientry.Api`, `Munientry.Client`, `api.Tests`) reference `Munientry.Shared` via `<ProjectReference>`. The old `api/Data/` (45 files) and `client/Shared/Models/` (46 files) and `client/Shared/Validation/` (2 files) directories have been deleted. `client/_Imports.razor` now declares `@using Munientry.Shared.Dtos` and `@using Munientry.Shared.Validation` globally. Drift between server and client DTO shapes is no longer possible — there is one file per DTO. All 123 API tests pass (0 failures).

**P4. ~~Shared fake services in test fixtures~~ — RESOLVED.**  
The seven SQL-querying services (`ICaseSearchService`, `IDailyListService`, `ICaseDocketService`, `IDrivingCaseService`, `IEventReportService`, `IFtaReportService`, `INotGuiltyReportService`) previously each had their `FakeXxx` implementation class inlined at the top of the single test file that needed it. A secondary `FakeCaseSearchServiceForFta` was also duplicated in `FtaReportApiTests.cs` to serve the FTA DOCX endpoint, producing two diverging implementations of the same interface. Every test class that replaced a service repeated an identical 6-line `WithWebHostBuilder` + `services.SingleOrDefault` / `Remove` / `AddScoped` block, giving 13 such helpers across 7 files.

- `api.Tests/Fakes/` — one file per fake service (`FakeCaseSearchService.cs`, `FakeDailyListService.cs`, `FakeCaseDocketService.cs`, `FakeDrivingCaseService.cs`, `FakeEventReportService.cs`, `FakeFtaReportService.cs`, `FakeNotGuiltyReportService.cs`). `FakeCaseSearchService` now covers all known case numbers used across the test suite (CaseSearch tests + FTA entry/batch tests), eliminating `FakeCaseSearchServiceForFta`.
- `api.Tests/Infrastructure/MuniEntryWebApplicationFactory.cs` — subclasses `WebApplicationFactory<Program>` and overrides `ConfigureWebHost` to swap all seven SQL services for their fakes in a single `ReplaceService<TService, TImpl>` helper. All tests get a pre-wired, SQL-free client with `_factory.CreateClient()` — no per-test `WithWebHostBuilder` needed.
- All 7 affected test files updated: inline fake class removed, `CreateClientWithFakeService()` / `CreateClientWithFakeServices()` / `CreateClientWithFakeFtaOnly()` helpers removed, fixture changed from `IClassFixture<WebApplicationFactory<Program>>` to `IClassFixture<MuniEntryWebApplicationFactory>`, and all `CreateClient*()` calls replaced with `_factory.CreateClient()`. DOCX-only test files were not changed — they never called SQL-querying services and continue to work identically.

Build: 0 errors, 0 warnings. Adding a new SQL-querying service now requires one file in `Fakes/` and one `ReplaceService` line in the factory — no changes to any test class.

**P2. ~~`AddWithValue` — explicit SQL type/length preferred~~ — RESOLVED.**  
`AddWithValue` infers `SqlDbType` from the CLR type at runtime, which causes implicit SQL Server type conversions that can defeat index seeks and generate misleading execution-plan parameter sniffing. All 17 `AddWithValue` calls across the 8 SQL-querying services have been replaced with explicit `.Add(name, SqlDbType, size).Value = ...` calls (or `.Add(name, SqlDbType)` with `.Precision`/`.Scale` for `Decimal`):

- `@CaseNumber` (3 services: `CaseSearchService`, `DrivingCaseService`, `CaseDocketService`) → `SqlDbType.NVarChar, 20`
- `@ReportDate` (`DailyListService`) → `SqlDbType.Date`
- `@EventCode` (`EventReportService`; SP documentation specifies `VARCHAR`) → `SqlDbType.VarChar, 10`
- `@EventDate` (3 services: `EventReportService`, `FtaReportService`, `NotGuiltyReportService`) → `SqlDbType.Date`
- 9 `FiscalJournalEntryService` parameters: `NVarChar` with explicit lengths for all `string?` fields; `Decimal` with `Precision = 18, Scale = 2` for `DisbursementAmount`. `using System.Data;` was added to that file (it had been omitted since `AddWithValue` doesn't require `SqlDbType`).

The `Date` type (not `DateTime`) matches the `DATE`-typed SP parameters and prevents the driver from sending a time component, eliminating a latent "no rows returned on date X" bug class. Build: 0 errors, 0 warnings.

**P5. ~~`DailyList:PassDateParameter` — use Options pattern instead of raw `IConfiguration`~~ — RESOLVED.**  
`DailyListService` previously injected `IConfiguration` and called `.GetValue<bool>("DailyList:PassDateParameter")` directly — coupling the service to the full configuration graph and making it impossible to unit-test without constructing a real or mocked `IConfiguration`.

Changes made:
- `api/Options/DailyListOptions.cs` — new strongly-typed options class with a single `bool PassDateParameter` property, XML-documented with the production/dev semantics.
- `api/ServiceRegistration.cs` — method signature updated to `AddMuniEntryServices(this IServiceCollection services, IConfiguration configuration)`; `services.Configure<DailyListOptions>(configuration.GetSection("DailyList"))` added at the top of the registration block.
- `api/Program.cs` — call updated to `builder.Services.AddMuniEntryServices(builder.Configuration)`.
- `api/Services/DailyListService.cs` — constructor updated to inject `IOptions<DailyListOptions> listOptions`; `_passDateParameter` is now assigned from `options.Value.PassDateParameter`. The inline comment block explaining the flag has been moved to the `DailyListOptions` XML doc. `IConfiguration` was still present at this point for the connection string; it was removed entirely in P6 below.

`appsettings.json` (`false`) and `appsettings.Development.json` (`true`) are unchanged — the JSON keys bind automatically. `FakeDailyListService` is unaffected. Build: 0 errors, 0 warnings.

**P6. ~~Connection string DRY violation — `IConfiguration` injected into 7 SQL services~~ — RESOLVED.**  
All 7 SQL-querying services (`CaseSearchService`, `DailyListService`, `CaseDocketService`, `DrivingCaseService`, `EventReportService`, `FtaReportService`, `NotGuiltyReportService`) previously injected `IConfiguration` and read the connection string via `_config.GetConnectionString("AuthorityCourt")`. If the key name or source ever changed, every service would need updating individually.

Changes made:
- `api/Options/AuthorityCourtOptions.cs` — new strongly-typed options class with a single `string ConnectionString` property.
- `api/ServiceRegistration.cs` — `services.Configure<AuthorityCourtOptions>(opts => opts.ConnectionString = configuration.GetConnectionString("AuthorityCourt") ?? throw ...)` registered once with a null-guard; `IConfiguration` is no longer passed into `ServiceRegistration`.
- All 7 SQL services — constructor updated to inject `IOptions<AuthorityCourtOptions>`; `IConfiguration` removed entirely. `DailyListService` now takes both `IOptions<AuthorityCourtOptions>` and `IOptions<DailyListOptions>` with no `IConfiguration` dependency at all.

Build: 0 errors, 0 warnings. 123/123 tests pass.

**P7. ~~`SchedulingEntryService` hardcoded judge names~~ — RESOLVED.**  
`SchedulingEntryService.GenerateDocx` selected a DOCX template via a `switch` on `dto.JudicialOfficer?.Trim().ToLower()` with literal arms `"fowler"`, `"hemmeter"`, and a default of `"rohrer"`. Adding or renaming a judge required a code change and redeployment.

Changes made:
- `api/Options/SchedulingOptions.cs` — new strongly-typed options class with `Dictionary<string, string> JudgeTemplates` (OrdinalIgnoreCase) and `string DefaultTemplate`.
- `api/appsettings.json` — new `"Scheduling"` section with `DefaultTemplate` and `JudgeTemplates` entries for Fowler, Hemmeter, and Rohrer. Adding or renaming a judge is now a config edit with no code change.
- `api/ServiceRegistration.cs` — `services.Configure<SchedulingOptions>(configuration.GetSection("Scheduling"))` added.
- `api/Services/SchedulingEntryService.cs` — replaced the `switch` with a constructor that injects `IOptions<SchedulingOptions>` and a `TryGetValue` lookup falling back to `DefaultTemplate`.

Build: 0 errors, 0 warnings. 123/123 tests pass.

**P8. ~~`DocxTemplateProcessor` no template caching~~ — RESOLVED.**  
`FillTemplate(string templatePath, ...)` previously called `File.OpenRead(templatePath)` on every request, reading each `.docx` template from disk for every form submission. Templates are static files that never change while the process is running.

Changes made:
- `api/DocxTemplateProcessor.cs` — added a `static readonly ConcurrentDictionary<string, byte[]> _templateCache` keyed by absolute path (`Path.GetFullPath`). The `FillTemplate(string, ...)` path overload now calls `_templateCache.GetOrAdd(path, static p => File.ReadAllBytes(p))` — the disk read happens exactly once per template per process lifetime. The cached `byte[]` is wrapped in `new MemoryStream(bytes, writable: false)` (no copy) before being passed to the stream overload's ZIP processing. The four backward-compatible overloads delegate unchanged.

With ~40 distinct templates in the deployment, the first 40 requests each warm one entry; all subsequent requests for those templates pay only a dictionary lookup and a `MemoryStream` construction. Build: 0 errors, 0 warnings. 123/123 tests pass.

**P9. ~~Raw `ex.Message` to UI in `FormPageBase`~~ — RESOLVED.**  
`FormPageBase<TModel>` exposed raw exception messages to the browser UI in three places:
- `LoadCaseDataAsync` catch — `$"Failed to load case: {ex.Message}"` surfaced internal exception text (e.g. DNS/TLS errors, IP addresses, stack hints).
- `HandleValidSubmit` non-success branch — `$"Error: {response.ReasonPhrase}"` surfaced the HTTP reason phrase.
- `HandleValidSubmit` catch — `ex.Message` surfaced the raw exception directly.

Changes made to `client/Shared/FormPageBase.cs`:
- `LoadCaseDataAsync` catch — replaced with a fixed string: `"Failed to load case data. Please try again."`
- `HandleValidSubmit` non-success branch — now calls a new `ReadErrorMessageAsync(response)` helper that reads the RFC 7807 `detail` field from the API's structured Problem Details response body (already sanitized by `GlobalExceptionHandler` — contains only `"Contact support with trace ID: {traceId}"`). Falls back to `"An error occurred. Please try again."` if the body is absent or unparseable.
- `HandleValidSubmit` catch — replaced with `"An unexpected error occurred. Please try again."`
- Added `using System.Net.Http` and `using System.Text.Json`; added private static `ReadErrorMessageAsync(HttpResponseMessage)` helper.

No internal paths, exception types, or server addresses reach the browser. The trace ID in the Problem Details `detail` field gives support staff a correlation handle without leaking internals. Build: 0 errors, 0 warnings.

**P10. ~~Silent `catch {}` in API clients~~ — RESOLVED.**  
Six bare `catch { return empty list; }` blocks across `CaseSearchApiClient`, `DailyListApiClient`, and `ReportsApiClient` swallowed all failures silently — a network error and "no data" were indistinguishable to the caller. Two additional `catch (Exception ex)` blocks in `ReportsApiClient.DownloadFtaEntryAsync` and `DownloadFtaBatchAsync` returned `ex.Message` raw to the UI. `DownloadFtaBatchAsync`’s non-success branch also forwarded the raw server response body to the UI.

Changes made:
- `client/Shared/CaseSearchApiClient.cs` — added `ILogger<CaseSearchApiClient>` constructor parameter; bare `catch` → `catch (Exception ex)` with `_logger.LogError(ex, …)` before returning the empty list.
- `client/Shared/DailyListApiClient.cs` — same pattern: `ILogger<DailyListApiClient>` added; logged and return empty list.
- `client/Shared/ReportsApiClient.cs` — added `ILogger<ReportsApiClient>` constructor parameter; all four list-returning `catch` blocks now log with context (case number / list type / date). Both `DownloadFtaEntryAsync` and `DownloadFtaBatchAsync` catch blocks now log then return `"An unexpected error occurred. Please try again."` instead of `ex.Message`. Both non-success branches now call a new private `TryReadProblemDetailAsync(response)` helper (reads the RFC 7807 `detail` field; falls back to a generic message) instead of forwarding raw status codes or response bodies. Added `using System.Text.Json` and `using Microsoft.Extensions.Logging`.

All three clients are registered via `AddHttpClient<T>()` — `ILogger<T>` is injected automatically by the DI container with no registration change needed. Logging goes to the browser console in Blazor WASM (Fluent Bit / the K8s log aggregator does not receive client-side logs). Build: 0 errors, 0 warnings.

**P11. ~~`Services/` flat folder — 89 files in a single directory~~ — RESOLVED.**  
`api/Services/` contained 89 files (44 interface/implementation pairs across all domains plus `DocxServiceBase.cs`, `IDocxService.cs`, `ChargeListBuilder.cs`) all at the same level, making ownership and navigation difficult.

Changes made:
- Created 12 domain subfolders inside `api/Services/` matching the existing `api/Endpoints/` groupings: `Admin/`, `Bond/`, `CaseData/`, `Civil/`, `Common/`, `CriminalPlea/`, `Diversion/`, `Driving/`, `Leap/`, `NoticesAndScheduling/`, `Probation/`, `Reports/`.
- Moved all 89 files into the appropriate subfolder via filesystem move only — no file contents were modified.
- All files retain `namespace Munientry.Api.Services;` (C# does not require namespace to match folder path), so `ServiceRegistration.cs`, all `Endpoints/*.cs` files, and all test files required **zero changes**. The `*.csproj` glob `<Compile Include="**\*.cs" />` picks up the new paths automatically.

Build: 0 errors, 0 warnings. Tests: 123/123 passed.

**P12. ~~`ICriminalFormApiClient` misleading name~~ — RESOLVED.**  
`ICriminalFormApiClient` / `CriminalFormApiClient` served as the generic HTTP POST wrapper injected into all 35 entry forms via `FormPageBase<TModel>` — including probation, driving, admin, civil, notices, and scheduling forms. The "Criminal" prefix implied the client was specific to criminal forms, making it harder to understand the base class contract.

Changes made:
- `client/Shared/ICriminalFormApiClient.cs` → renamed to `IEntryFormApiClient.cs`; interface name updated to `IEntryFormApiClient`.
- `client/Shared/CriminalFormApiClient.cs` → renamed to `EntryFormApiClient.cs`; class name updated to `EntryFormApiClient : IEntryFormApiClient`.
- `client/Shared/FormPageBase.cs` — `[Inject] protected ICriminalFormApiClient ApiClient` → `IEntryFormApiClient ApiClient`. XML doc comment updated from "criminal/probation/driving entry forms" to "all entry forms (criminal, probation, driving, admin, civil, notices, scheduling)".
- `client/Program.cs` — `AddHttpClient<ICriminalFormApiClient, CriminalFormApiClient>` → `AddHttpClient<IEntryFormApiClient, EntryFormApiClient>`. No other registration changes — the DI lifetime and `BaseAddress` wiring are identical.

Build: 0 errors. 123/123 tests pass.

**23. ~~Template paths fragile — `Path.Combine("Templates", "source", "*.docx")` could break if CWD changes~~ — NOT APPLICABLE.**  
The concern was that relative paths in service classes (e.g. `Path.Combine("Templates", "source", "Foo_Template.docx")`) would resolve incorrectly if the process working directory differed from the publish output. This does not apply: `Munientry.Api.csproj` marks all template files with `<Content Include="Templates\source\*.docx" CopyToOutputDirectory="PreserveNewest" />`, so `dotnet publish` places them at `/app/Templates/source/` alongside the DLL. The Dockerfile sets `WORKDIR /app` and the entrypoint is `dotnet Munientry.Api.dll` — the working directory at runtime is always `/app`. The relative path resolves correctly without any `IWebHostEnvironment.ContentRootPath` indirection. No code change is required.

---

## Priority Order for Enablement

| Priority | Item |
|---|---|
| 1 | Enable Entra ID on the API (JWT bearer validation) — *gating blocker for all else* |
| 2 | Restrict CORS to the known cluster hostname |
| 3 | ~~Add health/readiness endpoints for K8s probes~~ — DONE (see item 17) |
| 4 | Write K8s manifests with proper `Secret` references |
| 5 | ~~Replace `Results.Problem(ex.Message)` with structured error handling~~ — DONE (see item 5) |
| 6 | Design role/claim model before wiring stored procedures |
| 7 | ~~Add audit logging middleware~~ — DONE (see item 20) |
| 8 | ~~Decompose `Program.cs` into endpoint groups~~ — DONE (see item 15) |
| 9 | ~~Upgrade to `net10.0`~~ — DONE (see item 12) |
| 10 | ~~Add API versioning (`/api/v1/` prefix)~~ — DONE (see item 21) |
| 11 | Re-enable Blazor compression via K8s ingress gzip |
| — | ~~Remove DB write scaffolding; confirm read-only design~~ — DONE (see item 16) |
| — | ~~Shared DTO project to eliminate client/server drift~~ — DONE (see item P3) |
| — | ~~Shared fake services in test fixtures~~ — DONE (see item P4) |
| — | ~~`AddWithValue` — explicit SQL type/length~~ — DONE (see item P2) |
| — | ~~`DailyList:PassDateParameter` — Options pattern~~ — DONE (see item P5) |
| — | ~~Connection string DRY violation — `AuthorityCourtOptions`~~ — DONE (see item P6) |
| — | ~~`SchedulingEntryService` hardcoded judge names — `SchedulingOptions`~~ — DONE (see item P7) |
| — | ~~`DocxTemplateProcessor` no template caching~~ — DONE (see item P8) |
| — | ~~Raw `ex.Message` to UI in `FormPageBase`~~ — DONE (see item P9) |
| — | ~~Silent `catch {}` in API clients~~ — DONE (see item P10) |
| — | ~~`Services/` flat folder, 89 files~~ — DONE (see item P11) |
| — | ~~`ICriminalFormApiClient` misleading name~~ — DONE (see item P12) |
