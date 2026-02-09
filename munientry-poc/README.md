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

### Deny Privileges / Permit Retest Dialog (Criminal / Plea & Sentencing)

- **API Endpoint:** `POST /api/denyprivilegespermitretest`
- **DTO:** `DenyPrivilegesPermitRetestDto` (see api/Data/DenyPrivilegesPermitRetestDto.cs)
- **Service:** `DenyPrivilegesPermitRetestService` (see api/Services/DenyPrivilegesPermitRetestService.cs)
- **Blazor Form:** `client/Pages/Criminal/DenyPrivilegesPermitRetest.razor`
- **Integration Test:** `api.Tests/DenyPrivilegesPermitRetestApiTests.cs`

#### Example Request

```json
POST /api/denyprivilegespermitretest
{
   "DefendantFirstName": "Alex",
   "DefendantLastName": "Smith",
   "CaseNumber": "22TRC99999",
   "EntryDate": "2026-02-07T00:00:00Z",
   "EntryType": "DenyDrivingPrivileges",
   "HardTimeNotPassed": true,
   "PermanentIdCard": false,
   "OutOfStateLicense": false,
   "PetitionIncomplete": false,
   "NoInsurance": true,
   "NoEmployerInfo": false,
   "NoJurisdiction": false,
   "NoPayPlan": false,
   "ProhibitedActivities": false,
   "LicenseExpirationDate": "2027-02-07T00:00:00Z",
   "PrivilegesGrantDate": "2026-01-08T00:00:00Z",
   "NufcDate": "2026-01-28T00:00:00Z"
}
```

#### Description
Creates a new Deny Privileges / Permit Retest entry in the database. All fields are required unless otherwise noted. See the Blazor form for the full UI implementation and field mapping.
- Sidebar navigation with categorized forms
- End-to-end implementations for multiple court forms
- API endpoints for prefill, DOCX document generation, and direct SQL insert
- All legacy DOCX templates available for mapping
- Swagger/OpenAPI enabled for API testing at `/swagger`
- Automated integration tests








## Forms and Navigation Mapping (as of Feb 7, 2026)

- Trial To Court Notice (Scheduling)
- Final Jury Notice (Scheduling)
- Bond Hearing (Criminal / Plea & Sentencing)
- Probation Violation Bond (Probation / Community Control)
- Time To Pay Order (Admin / Courtroom Operations)
- Juror Payment (Admin / Courtroom Operations)
- General Notice of Hearing (Scheduling)
- Terms of Community Control / Notice of Community Control Violation Hearing (Probation / Community Control)
- **Community Control Terms Dialog (Probation / Community Control) [NEW, IMPLEMENTED]**
- **Scheduling Entry Dialog (Scheduling / Court Events) [NEW, IMPLEMENTED]**
- **Arraignment Continuance Dialog (Criminal / Plea & Sentencing) [NEW, IMPLEMENTED]**
- Notices & Freeform Civil (Notices / Freeform / Civil)
- Driving Privileges / CrimTraffic Helpers (Driving / Traffic)
- Fiscal Journal Entries Dialog (Admin / Courtroom Operations)
- Civil Freeform Entry Dialog (Notices / Freeform / Civil)
- **Fine Only Dialog (Criminal / Plea & Sentencing) [NEW, IMPLEMENTED]**
- **LEAP Sentencing Dialog (Criminal / Plea & Sentencing) [NEW, IMPLEMENTED]**





### Forms Yet to be Converted (from navigation)
   - **Diversion Dialog fully implemented and tested as of Feb 7, 2026.**
   - **Not Guilty Plea Dialog fully implemented and tested as of Feb 7, 2026.**
   - **Plea Only - Future Sentencing Dialog fully implemented and tested as of Feb 7, 2026.**
- Not Guilty Plea / Appear on Warrant / Bond Modification Special Bond Conditions Dialog (Criminal / Plea & Sentencing)
- Appear on Warrant (No Plea) Dialog (Criminal / Plea & Sentencing)
- Bond Modification/Revocation Dialog (Criminal / Plea & Sentencing)
- Community Service, License Suspension, Fingerprinting & Victim Notification, Immobilize/Impound and other Conditions Secondary Dialogs (Criminal / Plea & Sentencing)
- Arraignment / Failure to Appear / Bond Dialogs (Criminal / Plea & Sentencing)
- Criminal Sealing / Deny Privileges Dialogs (Criminal / Plea & Sentencing)
- Competency / Criminal Sealing / Juror Dialogs (Miscellaneous)
- Main application window (Miscellaneous)

### Navigation Forms Not Found in Legacy Python Dialogs
- If any of the above navigation forms do not correspond to a legacy Python dialog, please review and confirm if they are new additions or require new implementation.



## API Endpoints
- `/api/trialtocourt` (POST)
- `/api/finaljurynotice` (POST)
- `/api/bondhearing` (POST)
- `/api/probationviolationbond` (POST)
- `/api/timetopayorder` (POST)
- `/api/jurorpayment` (POST)
- `/api/generalnoticeofhearing` (POST)
- `/api/communitycontroltermsnotices` (POST)
- `/api/leapsentencing` (POST)
- `/api/communitycontrolterms` (POST)  <!-- NEW -->




- All completed forms have matching Blazor pages, DTOs, API endpoints, service/database logic, and automated tests.
- Navigation links match implemented forms.
- Test coverage is automated and up-to-date.
- **LEAP Sentencing Dialog fully implemented and tested as of Feb 7, 2026.**
- **LEAP Plea Admission Dialog fully implemented and tested as of [DATE].**
- **Community Control Terms Dialog fully implemented and tested as of Feb 7, 2026.**
- **LEAP Admission - Already Valid Dialog fully implemented and tested as of Feb 7, 2026.**
+- **Sentencing Only - Already Plead Dialog fully implemented and tested as of Feb 7, 2026.**
- All completed forms have matching Blazor pages, DTOs, API endpoints, service/database logic, and automated tests.
- Navigation links match implemented forms.
- Test coverage is automated and up-to-date.
- **LEAP Sentencing Dialog fully implemented and tested as of Feb 7, 2026.**
- **LEAP Plea Admission Dialog fully implemented and tested as of [DATE].**
- **Community Control Terms Dialog fully implemented and tested as of Feb 7, 2026.**

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

### Final Jury Notice of Hearing (Scheduling)

- **API Endpoint:** `POST /api/finaljury`
- **DTO:** `FinalJuryNoticeDto` (see api/Data/FinalJuryNoticeDto.cs)
- **Service:** `FinalJuryNoticeService` (see api/Services/FinalJuryNoticeService.cs)
- **Blazor Form:** `client/Pages/Scheduling/FinalJuryNoticeOfHearing.razor`
- **Integration Test:** `api.Tests/FinalJuryNoticeApiTests.cs`

#### Example Request

```json
POST /api/finaljury
{
   "CaseNumber": "22TRC54321",
   "DefendantFirstName": "Jane",
   "DefendantLastName": "Smith",
   "EntryDate": "2024-06-01T00:00:00Z",
   "DefenseCounselName": "Johnson",
   "FinalJuryDate": "2024-06-15T00:00:00Z",
   "FinalJuryTime": "2:00 PM",
   "AssignedCourtroom": "B",
   "InterpreterRequired": false,
   "LanguageRequired": "",
   "DateConfirmedWithCounsel": false
}
```

#### Description
Creates a new Final Jury Notice of Hearing entry in the database. All fields are required unless otherwise noted. See the Blazor form for the full UI implementation and field mapping.
