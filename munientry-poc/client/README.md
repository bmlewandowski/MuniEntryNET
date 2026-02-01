# Client README

This client is now a Blazor WebAssembly project (C#) instead of a static JS SPA.

Entra ID / MSAL notes:
- When adding Entra ID, use `Microsoft.Authentication.WebAssembly.Msal` on the client and `Microsoft.Identity.Web` on the API.
- Configure the SPA redirect URI in the Entra App registration to `http://localhost:3000` and set `ENTRA_CLIENT_ID` when testing.

Run with Docker Compose (from `munientry-poc` root):

```bash
docker-compose up --build client
```
