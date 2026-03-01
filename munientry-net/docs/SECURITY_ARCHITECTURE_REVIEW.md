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
`GlobalExceptionHandler` (`api/Middleware/GlobalExceptionHandler.cs`) implements `IExceptionHandler` and is registered via `services.AddExceptionHandler<GlobalExceptionHandler>()` + `app.UseExceptionHandler()` in the middleware pipeline. It logs the full exception (type, message, stack trace) with the correlation `TraceId` to the server-side log, then returns a sanitized RFC 7807 Problem Details response containing only `title: "An error occurred processing your request."` and `detail: "Contact support with trace ID: {traceId}"`. All 36 catch blocks across the 9 endpoint files have been updated from `return Results.Problem($"DOCX generation failed: {ex.Message}")` to `throw;`, so exceptions propagate to the global handler. No internal paths, messages, or types reach the client.

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
All 28 previously concrete services now have a matching `IXxxService` interface in `api/Services/`. Each interface file declares exactly the public method(s) of its implementation. All 28 classes implement their interface (`: IXxxService`). `ServiceRegistration.cs` registers every service as an interface→implementation pair (`AddScoped<IXxx, Xxx>()`). The three endpoint lambda parameters that injected concrete types (`DrivingPrivilegesService`, `TrialToCourtNoticeService`, `TimeToPayOrderService`) have been updated to inject their interfaces. The inline `ILeapAdmissionPleaService` declaration that was embedded inside `LeapAdmissionPleaService.cs` has been extracted to its own file. The `IFiscalJournalEntryService` declaration that was also inlined inside `FiscalJournalEntryService.cs` has likewise been extracted to `IFiscalJournalEntryService.cs`. All services are now mockable in unit tests without a database.

**12. ~~`net9.0` is Standard Term Support (EOL May 2026)~~ — RESOLVED.**  
All four projects (`Munientry.Api`, `Munientry.Client`, `api.Tests`, `Munientry.DocxTemplating`) and both Dockerfiles have been upgraded to `net10.0` (LTS). ASP.NET Core / Blazor package references (`Microsoft.AspNetCore.OpenApi`, `Microsoft.AspNetCore.Components.WebAssembly`, `Microsoft.Authentication.WebAssembly.Msal`, `Microsoft.AspNetCore.Mvc.Testing`) have been updated to `10.0.0`.

**13. Compression is disabled for an nginx limitation.**  
`BlazorEnableCompression=false` in the `.csproj` was a workaround for the K8s/nginx pod not having the Brotli module. In a production K8s cluster, TLS termination and gzip are handled at the ingress controller level (nginx-ingress or Traefik). This should be re-evaluated; a 10MB+ uncompressed WASM payload is a real load-time problem for court users on constrained city hall networks.

**14. The MSAL NuGet dependency is commented out, not absent.**  
If the package reference line in the `.csproj` is accidentally left commented through a CI pipeline that does `dotnet restore`, the auth code that references it will cause a build failure at the worst time. Resolve the enable/disable decision and commit the final state.

---

## Architecture

**15. ~~`Program.cs` in the API is 1,389 lines of inline endpoint handlers~~ — RESOLVED.**  
Decomposed into `api/ServiceRegistration.cs` (all DI registrations, called via `builder.Services.AddMuniEntryServices()`) and 9 static extension-method files under `api/Endpoints/`, each owning one domain area (`CaseData`, `Driving`, `CriminalPlea`, `Leap`, `Diversion`, `Bond`, `Probation`, `NoticesAndScheduling`, `Civil`, `Admin`). `Program.cs` is now 36 lines. To add a new endpoint: open the relevant `Endpoints/` file and add a `MapPost`/`MapGet` call. To add a new service: add its registration to `ServiceRegistration.cs`.

**16. DB write logic is commented out across all endpoints.**  
```csharp
// service.InsertDrivingPrivileges(dto);
// --- End DB Save Logic ---
```
This pattern is present throughout `Program.cs`. The current system generates documents but writes nothing to the database. When stored procedure wiring begins, a consistent pattern (unit-of-work, structured error handling, transaction management) must be applied uniformly — not endpoint by endpoint.

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
