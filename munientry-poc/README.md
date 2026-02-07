# MuniEntry POC (Blazor/Docker Compose)

This is a proof-of-concept rewrite of the MuniEntry application using modern technologies:
- **Blazor WebAssembly** for the client UI
- **ASP.NET Core Minimal API** for the backend
- **Docker Compose** for local development and deployment

## Project Structure

- `client/` — Blazor WebAssembly front-end
- `api/` — ASP.NET Core Minimal API backend
- `api/Templates/` — All DOCX templates migrated from the legacy system
- `api.Tests/` — xUnit integration tests for the API
- `docker-compose.yml` — Multi-container orchestration

## Features
- Sidebar navigation with categorized forms
- End-to-end implementations for multiple court forms
- API endpoints for prefill, DOCX document generation, and direct SQL insert
- All legacy DOCX templates available for mapping
- Swagger/OpenAPI enabled for API testing at `/swagger`
- Automated integration tests

## Completed Forms (as of Feb 7, 2026)
- Trial To Court Notice
- Final Jury Notice
- Bond Hearing
- Probation Violation Bond
- Time To Pay Order
- Juror Payment
- General Notice of Hearing
- Terms of Community Control / Notice of Community Control Violation Hearing

## API Endpoints
- `/api/trialtocourt` (POST)
- `/api/finaljurynotice` (POST)
- `/api/bondhearing` (POST)
- `/api/probationviolationbond` (POST)
- `/api/timetopayorder` (POST)
- `/api/jurorpayment` (POST)
- `/api/generalnoticeofhearing` (POST)
- `/api/communitycontroltermsnotices` (POST)

## Progress
- All completed forms have matching Blazor pages, DTOs, API endpoints, service/database logic, and automated tests.
- Navigation links match implemented forms.
- Test coverage is automated and up-to-date.

## Getting Started

1. **Build and run with Docker Compose:**
   ```sh
   docker-compose up --build
   ```
2. Access the client at [http://localhost:3000](http://localhost:3000)
3. Access the API (Swagger UI) at [http://localhost:5000/swagger](http://localhost:5000/swagger)

## Adding New Forms and Templates
- Add a new `.razor` page in `client/Pages/<Category>/`
- Add corresponding DTOs and API endpoints in `api/`
- Place new or updated DOCX templates in `api/Templates/`
- Update navigation in the client as needed
- Map DTO fields to DOCX placeholders in the API endpoint

## Authentication (Entra ID / MSAL)
- Authentication is not yet wired up.
- When ready, use `Microsoft.Authentication.WebAssembly.Msal` on the client and `Microsoft.Identity.Web` on the API.
- Configure Entra App registration and environment variables as needed (`ENTRA_CLIENT_ID`, `ENTRA_TENANT_ID`, `ENTRA_CLIENT_SECRET`).

## Testing
- Run tests in `api.Tests/` using your preferred .NET test runner

## License
MIT or as specified by the project owner.

# MuniEntry POC (standalone)

This is a minimal, standalone Proof-of-Concept scaffold for a web-based MuniEntry rewrite. It is intentionally kept outside the main project and does not modify any existing files.

Contents:
- `api/` - minimal ASP.NET Core Web API (net8.0) with example endpoints
- `client/` - tiny static SPA served by nginx (calls the API)
- `docker-compose.yml` - local compose to run API + client

Quick start (requires Docker and Docker Compose):

```bash
cd munientry-poc
docker-compose up --build
```

Open the client at http://localhost:3000 and API at http://localhost:5000/api/health

Entra ID placeholders
- `docker-compose.yml` contains environment variable placeholders for `ENTRA_CLIENT_ID`, `ENTRA_TENANT_ID`, and `ENTRA_CLIENT_SECRET`.
- See `api/README.md` and `client/README.md` for notes on adding MSAL and Entra ID configuration.

Next steps to move to Kubernetes:
- Push built images to a registry
- Create `k8s/` manifests (Deployment, Service, Ingress) or a Helm chart referencing the same images
- Use Kubernetes Secrets or an external secret provider for Entra credentials

This scaffold is minimal by design — tell me if you want a Blazor client instead of the static client, or MSAL wiring included now.
