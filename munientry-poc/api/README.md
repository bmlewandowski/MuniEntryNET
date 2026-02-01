# API README

This minimal ASP.NET Core API is a placeholder for the real MuniEntry backend.

Entra ID / MSAL notes:
- This scaffold does not wire up authentication yet.
- When ready, use `Microsoft.Identity.Web` to protect the API and validate access tokens.
- Keep values in environment variables (`ENTRA_CLIENT_ID`, `ENTRA_TENANT_ID`, `ENTRA_CLIENT_SECRET`) and avoid hardcoding.

Local run (if you have .NET SDK):

```bash
cd api
dotnet run
```

Or with Docker Compose (recommended for isolation):

```bash
cd ..
docker-compose up --build
```
