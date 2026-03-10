# Entra ID Authentication Setup

This document describes how to enable Microsoft Entra ID (formerly Azure AD) authentication in the MuniEntry Blazor application so that users sign in with their `@cityofdelawareoh.gov` accounts.

Authentication is fully wired up in the codebase but **disabled by default**. All code is present and commented out so the app builds and runs without authentication. Enabling it requires only the steps below.

> **⚠️ Important:** These steps enable authentication on the **Blazor client only**. The API has its own parallel commented-out scaffolding (`Munientry.Api.csproj` package reference, `api/Program.cs` middleware, `api/appsettings.json` config block) that **must also be uncommented** for the API to validate bearer tokens. An authenticated client sending tokens to an unguarded API is not secure. See [SECURITY_ARCHITECTURE_REVIEW.md](SECURITY_ARCHITECTURE_REVIEW.md) item 2 for the API-side steps.

---

## Prerequisites

- Access to the City of Delaware's Azure Portal / Entra ID tenant.
- Permission to create or view App Registrations.

---

## Step 1 — Register the App in Azure Portal

1. Go to [portal.azure.com](https://portal.azure.com) → **Entra ID** → **App Registrations** → **New registration**.
2. Set a name, e.g. `MuniEntry Blazor Client`.
3. Under **Supported account types**, choose **Accounts in this organizational directory only** (single tenant).
4. Under **Redirect URI**, select platform **Single-page application (SPA)** and set the URI to:
   - Local dev: `https://localhost/authentication/login-callback`
   - Docker / production: `https://<your-host>/authentication/login-callback`
5. Click **Register**.
6. From the Overview page, copy:
   - **Application (client) ID** → this is your `ClientId`
   - **Directory (tenant) ID** → this is your `TenantId`

---

## Step 2 — Fill in the Configuration

Open both files below and replace the placeholder values with the IDs copied in Step 1.

### `client/wwwroot/appsettings.json` — local / dev
```json
"AzureAd": {
  "Authority": "https://login.microsoftonline.com/REPLACE_WITH_TENANT_ID",
  "ClientId": "REPLACE_WITH_CLIENT_ID",
  "ValidateAuthority": true
}
```

### `client/wwwroot/appsettings.Docker.json` — Docker / production
```json
"AzureAd": {
  "Authority": "https://login.microsoftonline.com/REPLACE_WITH_TENANT_ID",
  "ClientId": "REPLACE_WITH_CLIENT_ID",
  "ValidateAuthority": true
}
```

---

## Step 3 — Uncomment the NuGet Package

In `client/Munientry.Client.csproj`, uncomment the MSAL package reference:

```xml
<!-- Before -->
<!-- <PackageReference Include="Microsoft.Authentication.WebAssembly.Msal" Version="10.0.0" /> -->

<!-- After -->
<PackageReference Include="Microsoft.Authentication.WebAssembly.Msal" Version="10.0.0" />
```

---

## Step 4 — Uncomment the Using Directives

### `client/Program.cs`
Near the top, uncomment:
```csharp
using Microsoft.Authentication.WebAssembly.Msal;
```

### `client/_Imports.razor`
Uncomment both lines:
```razor
@using Microsoft.AspNetCore.Components.Authorization
@using Microsoft.AspNetCore.Components.WebAssembly.Authentication
```

---

## Step 5 — Uncomment the MSAL Service Registration

In `client/Program.cs`, uncomment the `AddMsalAuthentication` block:

```csharp
builder.Services.AddMsalAuthentication(options =>
{
    builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
    options.ProviderOptions.Authentication.LoginMode = "redirect";
    options.ProviderOptions.DefaultAccessTokenScopes.Add("openid");
    options.ProviderOptions.DefaultAccessTokenScopes.Add("profile");
    options.ProviderOptions.DefaultAccessTokenScopes.Add("email");
});
```

---

## Step 6 — Swap the Router in App.razor

In `client/App.razor`:

1. **Remove** (or comment out) the active `<Router>` block at the top.
2. **Uncomment** the `<CascadingAuthenticationState>` block below it.

The commented block wraps the router in `CascadingAuthenticationState` and uses `AuthorizeRouteView`, which enforces `[Authorize]` on any page that has that attribute.

---

## Step 7 — Uncomment the Authentication Page

In `client/Pages/Authentication.razor`, remove the outer `@* ... *@` Razor comment to activate the MSAL redirect handler route (`/authentication/{action}`).

---

## Protecting Individual Pages

Once authentication is active, add `@attribute [Authorize]` near the top of any `.razor` page to restrict it to signed-in users:

```razor
@page "/some-form"
@attribute [Authorize]
```

Pages **without** `[Authorize]` remain publicly accessible.

---

## File Reference Summary

| File | Purpose |
|---|---|
| `client/Munientry.Client.csproj` | NuGet package reference for MSAL (Step 3) |
| `client/wwwroot/appsettings.json` | Entra ID TenantId / ClientId for local dev (Step 2) |
| `client/wwwroot/appsettings.Docker.json` | Entra ID TenantId / ClientId for Docker (Step 2) |
| `client/Program.cs` | MSAL service registration + using directive (Steps 4–5) |
| `client/_Imports.razor` | Auth-related global using directives (Step 4) |
| `client/App.razor` | Router swap from `RouteView` to `AuthorizeRouteView` (Step 6) |
| `client/Pages/Authentication.razor` | MSAL login/logout redirect callback page (Step 7) |

---

## Judicial Officer Resolution

### Background

In the legacy Python app, staff names were stored in `config.ini` under `[staff]`, and the current machine's hostname (via `socket.gethostname()`) was used to work out which user was logged in. The user then clicked a radio button on the main window to set `mainwindow.judicial_officer`. That `JudicialOfficer` object was passed into every dialog, stamped onto the entry model, and rendered into the Word document signature block.

In the Blazor app this entire mechanism is replaced by Entra ID — the logged-in user IS the judicial officer. The lookup table that was `config.ini [staff]` is now `Staff:JudicialOfficers` in `appsettings.json`.

---

### Architecture

```
Entra ID login (or MockUser setting during dev)
    ↓
IJudicialOfficerProvider.GetCurrentOfficerAsync()
    ↓
JudicialOfficerSession.SetJudicialOfficer(officer)   ← scoped, one per browser session
    ↓
AppInitializer.razor (wraps the whole app in MainLayout)
    ├─ visiting judge? → shows name-entry modal (mirrors Python QInputDialog)
    └─ otherwise → unlocks the app body
    ↓
FormPageBase.StampJudicialOfficer()                  ← called before every form submit
    ↓
DTO.JudicialOfficerFirstName / LastName / Type → API → Word template signature block
```

---

### Staff Configuration

Staff names are declared in `client/wwwroot/appsettings.json` under the `Staff:JudicialOfficers` section, directly mirroring `config.ini [staff]`:

```json
"Staff": {
  "JudicialOfficers": {
    "judge_1": { "FirstName": "Mark",    "LastName": "Fowler",  "OfficerType": "Judge" },
    "judge_2": { "FirstName": "Kyle",    "LastName": "Rohrer",  "OfficerType": "Judge" },
    "magistrate_1": { "FirstName": "Amanda", "LastName": "Bunner",  "OfficerType": "Magistrate" },
    "magistrate_2": { "FirstName": "Kevin",  "LastName": "Pelanda", "OfficerType": "Magistrate" },
    "magistrate_3": { "FirstName": "Justin", "LastName": "Kudela",  "OfficerType": "Magistrate" },
    "visiting_judge": {
      "FirstName": "Visiting", "LastName": "Judge",
      "OfficerType": "Visiting Judge", "IsVisiting": true
    }
  }
}
```

When Entra ID is enabled, the `EntraIdJudicialOfficerProvider` maps the user's UPN (e.g. `mark.fowler@cityofdelawareoh.gov`) to a key in this section. Add a matching entry per staff member and use the UPN as the key.

---

### Testing Without Entra ID (Mock Mode)

While Entra ID is disabled the app uses `MockJudicialOfficerProvider`, which reads the `MockUser` key from `appsettings.json`:

```json
"MockUser": "judge_1"
```

Change this value to test different officers without touching any C# code:

| Value | Result |
|---|---|
| `"judge_1"` | Mark Fowler — Judge |
| `"judge_2"` | Kyle Rohrer — Judge |
| `"magistrate_1"` | Amanda Bunner — Magistrate |
| `"visiting_judge"` | Triggers the visiting-judge name-entry modal |
| `""` (empty) | Unknown user — app loads but officer banner shows warning |

---

### New Files Added for Judicial Officer Resolution

| File | Purpose |
|---|---|
| `client/Shared/Models/JudicialOfficer.cs` | Immutable record; mirrors Python `JudicialOfficer(Person)` |
| `client/Shared/Services/JudicialOfficerSession.cs` | Scoped state; mirrors `mainwindow.judicial_officer` |
| `client/Shared/Services/IJudicialOfficerProvider.cs` | Interface allowing mock ↔ Entra ID swap |
| `client/Shared/Services/MockJudicialOfficerProvider.cs` | Dev/test implementation; reads `MockUser` from config |
| `client/Shared/AppInitializer.razor` | Startup wrapper; handles visiting-judge modal |

Modified:

| File | Change |
|---|---|
| `client/wwwroot/appsettings.json` | Added `MockUser` + `Staff:JudicialOfficers` section |
| `client/Program.cs` | Registered `JudicialOfficerSession` (Scoped) + `MockJudicialOfficerProvider` |
| `client/_Imports.razor` | Added `Shared.Models` and `Shared.Services` namespaces |
| `client/Shared/MainLayout.razor` | Wrapped body in `<AppInitializer>`, added officer banner |
| `client/Shared/FormPageBase.cs` | Added `StampJudicialOfficer()` called before every submit |

---

### Step 7 — Wire Up EntraIdJudicialOfficerProvider

After completing Steps 1–6 (MSAL client auth enabled):

1. Create `client/Shared/Services/EntraIdJudicialOfficerProvider.cs` implementing `IJudicialOfficerProvider`. Resolve the UPN from the MSAL `AuthenticationState` claims principal (claim type `preferred_username` or `upn`) and look it up in `Staff:JudicialOfficers`.

2. In `client/Program.cs`, swap the registration:
```csharp
// Remove / comment out:
builder.Services.AddScoped<IJudicialOfficerProvider, MockJudicialOfficerProvider>();

// Add:
builder.Services.AddScoped<IJudicialOfficerProvider, EntraIdJudicialOfficerProvider>();
```

Everything else — `AppInitializer`, `JudicialOfficerSession`, `StampJudicialOfficer`, the visiting-judge modal — works identically in both modes.

---

## API-Side Auth (Required — Not in Steps Above)

After completing Steps 1–7 above, the client will request and receive Entra ID tokens. The API must independently validate those tokens or it remains unguarded.

The API already has all scaffolding in place in commented-out form. To enable:

1. **`api/Munientry.Api.csproj`** — uncomment the `Microsoft.Identity.Web` package reference and run `dotnet restore`.
2. **`api/appsettings.json`** — uncomment the `AzureAd` config block and fill in the same `TenantId` and `ClientId` from Step 1 above. Set `Audience` to `api://YOUR_CLIENT_ID`.
3. **`api/Program.cs`** — uncomment the `using Microsoft.Identity.Web;` directive, the `AddAuthentication().AddMicrosoftIdentityWebApi(...)` call, the fallback authorization policy (`RequireAuthenticatedUser`), and `app.UseAuthentication()` / `app.UseAuthorization()`.

The fallback authorization policy protects all endpoints globally — no `[Authorize]` attribute is needed on individual endpoints.

See [SECURITY_ARCHITECTURE_REVIEW.md](SECURITY_ARCHITECTURE_REVIEW.md) items 1–2 for full context and remaining role/claim design work.
