# Entra ID Authentication Setup

This document describes how to enable Microsoft Entra ID (formerly Azure AD) authentication in the MuniEntry POC Blazor application so that users sign in with their `@cityofdelawareoh.gov` accounts.

Authentication is fully wired up in the codebase but **disabled by default**. All code is present and commented out so the app builds and runs without authentication. Enabling it requires only the steps below.

---

## Prerequisites

- Access to the City of Delaware's Azure Portal / Entra ID tenant.
- Permission to create or view App Registrations.

---

## Step 1 — Register the App in Azure Portal

1. Go to [portal.azure.com](https://portal.azure.com) → **Entra ID** → **App Registrations** → **New registration**.
2. Set a name, e.g. `MuniEntry POC Blazor Client`.
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

In `client/Munientry.Poc.Client.csproj`, uncomment the MSAL package reference:

```xml
<!-- Before -->
<!-- <PackageReference Include="Microsoft.Authentication.WebAssembly.Msal" Version="9.0.0" /> -->

<!-- After -->
<PackageReference Include="Microsoft.Authentication.WebAssembly.Msal" Version="9.0.0" />
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
| `client/Munientry.Poc.Client.csproj` | NuGet package reference for MSAL (Step 3) |
| `client/wwwroot/appsettings.json` | Entra ID TenantId / ClientId for local dev (Step 2) |
| `client/wwwroot/appsettings.Docker.json` | Entra ID TenantId / ClientId for Docker (Step 2) |
| `client/Program.cs` | MSAL service registration + using directive (Steps 4–5) |
| `client/_Imports.razor` | Auth-related global using directives (Step 4) |
| `client/App.razor` | Router swap from `RouteView` to `AuthorizeRouteView` (Step 6) |
| `client/Pages/Authentication.razor` | MSAL login/logout redirect callback page (Step 7) |
