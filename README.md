# MuniEntry

Municipal court entry software for the City of Delaware, OH.

This repository contains two distinct components:

| Component | Path | Description |
|---|---|---|
| **Legacy Python App** | `munientry/` | PyQt6 desktop app — original production system |
| **Blazor / .NET Port** | `munientry-net/` | Blazor WASM + .NET minimal API — active rewrite |

---

## Legacy Python App

### Running

```sh
python MuniEntry_app.py
```

Requires Python 3.11+, PyQt6, and a network connection to the SQL Server `AuthorityCourt` database.
Install dependencies with:

```sh
pip install -r requirements.txt
```

### Legacy Python Documentation

The Sphinx-generated HTML documentation lives in `docs/` (root level):

| File | Contents |
|---|---|
| [`docs/index.html`](docs/index.html) | Documentation home |
| [`docs/Build.html`](docs/Build.html) | Build / packaging notes |
| [`docs/Databases.html`](docs/Databases.html) | Database connection overview |
| [`docs/Loaders.html`](docs/Loaders.html) | CMS case loader patterns |
| [`docs/Menu.html`](docs/Menu.html) | Main menu / report functions |
| [`docs/Data.html`](docs/Data.html) | Data models and cleaners |
| [`docs/Deployment.html`](docs/Deployment.html) | Deployment guide |

Rebuild the docs from source with:

```sh
cd docsource
make html
```

---

## Blazor / .NET Port (`munientry-net/`)

### Quick Start

Run all commands from the `munientry-net/` directory:

```sh
cd munientry-net

docker compose build api      # Build the API (includes Munientry.DocxTemplating library)
docker compose build client   # Build the Blazor WASM client
docker compose up --no-build  # Start all services
```

- **Client:** [http://localhost:3000](http://localhost:3000)
- **API:** [http://localhost:5000](http://localhost:5000)

### Running Locally Without Docker

```sh
cd munientry-net/api
dotnet run                    # API on http://localhost:5000 — uses Windows Auth by default
```

```sh
cd munientry-net/client
dotnet run                    # Blazor dev server
```

### Docker + Database Configuration

Windows Authentication is not supported in Linux containers. Create a `.env` file in `munientry-net/`
(never commit this file) and set:

```
AUTHORITY_COURT_CONNSTR=Server=host.docker.internal;Database=AuthorityCourt;User Id=<SQL_LOGIN>;Password=<PASSWORD>;TrustServerCertificate=True;
```

`docker-compose.yml` injects this at runtime. If the variable is absent the container falls back to
the Windows Auth connection string in `appsettings.json`, which will not work from a Linux container.

See [`munientry-net/docs/StoredProcedureIntegration.md`](munientry-net/docs/StoredProcedureIntegration.md)
for full connection string documentation.

### .NET Documentation

| File | Contents |
|---|---|
| [`munientry-net/docs/StoredProcedureIntegration.md`](munientry-net/docs/StoredProcedureIntegration.md) | SQL Server stored procedures, connection strings, case search & daily list endpoints |
| [`munientry-net/docs/DTO_Reference.md`](munientry-net/docs/DTO_Reference.md) | DTO inventory, namespace conventions, `FormPageBase<TDto>` pattern |
| [`munientry-net/docs/Blazor_DOCX_Migration.md`](munientry-net/docs/Blazor_DOCX_Migration.md) | Form-by-form migration status: Python → Blazor DOCX generation |
| [`munientry-net/docs/EntraID_Setup.md`](munientry-net/docs/EntraID_Setup.md) | Step-by-step guide to enable Microsoft Entra ID (Azure AD) authentication |

---

## Client Architecture

### Daily Case Lists — Home Page (`/`)

`client/Pages/Index.razor` is the Blazor equivalent of the Python app's **Daily Case Lists tab** on
the main window. It replaces the six `DailyCaseListComboBox` widgets driven by radio buttons.

| Python | Blazor |
|--------|--------|
| `case_list_tab` — 6 `DailyCaseListComboBox` widgets | 3 × 2 CSS grid of `<select size="9">` panels |
| Radio button activates one list at a time | Click panel header to highlight active list; all lists always visible |
| Loads all procs on startup via `CaseListHandler` | `OnInitializedAsync` fires all 6 API calls in parallel via `Task.WhenAll` |
| `reload_cases_Button` | "⟳ Reload" button — also auto-reloads on date change |
| `"LastName - CaseNumber"` combo text | `"LastName — CaseNumber  \|  HH:MM"` option text (time from richer API result) |
| Stored procedures used `GETDATE()` internally — no date input | Date picker (defaults to today) passed to `GET /api/dailylist/{listType}/{date}` |
| Entry dialog opened from toolbar buttons | Context-sensitive action bar appears when a case is selected, links to relevant forms with `/{CaseNumber}` pre-filled |

**Key files:**

| File | Role |
|---|---|
| `munientry-net/client/Pages/Index.razor` | Home page — daily case lists UI |
| `munientry-net/client/Shared/DailyListService.cs` | HTTP client for `GET /api/dailylist/{listType}/{date}` |
| `munientry-net/client/Shared/Models/DailyListResultDto.cs` | Client-side DTO for daily list rows |
| `munientry-net/api/Services/DailyListService.cs` | API-side service — executes stored procedures |
| `munientry-net/api/Data/DailyListResultDto.cs` | API-side DTO + `DailyListStoredProcs` map |

**Valid list-type keys** (`arraignments`, `slated`, `pleas`, `pcvh_fcvh`, `final_pretrial`, `trials_to_court`)
map to the same stored procedures as the Python `DAILY_CASE_LIST_STORED_PROCS` dictionary.

### Entry Forms — `FormPageBase<TDto>`

All criminal, probation, and driving entry forms share `client/Shared/FormPageBase.cs`:

| Concern | How it's handled |
|---|---|
| Case pre-population | `OnParametersSetAsync` → `CaseSearchService.SearchCaseAsync` → `PopulateFromCaseAsync` override |
| DOCX download | `HandleValidSubmit` POSTs via `ICriminalFormApiClient`, detects `wordprocessingml` content-type, triggers JS `downloadFile` interop |
| Loading / submitting state | `IsLoadingCase` / `IsSubmitting` booleans — all form buttons disabled while either is true |
| URL resolution | `ApiHelper.GetApiBaseUrl()` — resolves API base URL dynamically for Docker / local environments |

Each form only needs to implement:
1. `protected override string ApiEndpoint` — relative POST path (e.g. `"api/diversionplea"`)
2. `protected override Task PopulateFromCaseAsync(...)` — maps case search results onto the model

---

## Included Forms

| Blazor Form Page | Route | Legacy Python Dialog |
|---|---|---|
| Home — Daily Case Lists | `/` | `main_window_ui.py` — `case_list_tab` |
| Criminal > Arraignment Continuance | `/criminal/arraignmentcontinuance` | `arraignment_continuance_dialog_ui.py` |
| Criminal > Fine Only Plea | `/criminal/fineonlyplea/{CaseNumber?}` | `fine_only_plea_dialog_ui.py` |
| Criminal > Diversion Plea | `/criminal/diversion-plea/{CaseNumber?}` | `diversion_plea_dialog_ui.py` |
| Criminal > LEAP Admission / Plea | `/criminal/leap-admission-plea/{CaseNumber?}` | `leap_admission_plea_dialog_ui.py` |
| Criminal > LEAP Admission Already Valid | `/criminal/leap-admission-already-valid/{CaseNumber?}` | `leap_admission_already_valid_dialog_ui.py` |
| Criminal > LEAP Valid Sentencing | `/criminal/leap-valid-sentencing/{CaseNumber?}` | `leap_valid_sentencing_dialog_ui.py` |
| Criminal > LEAP Sentencing | `/criminal/leap-sentencing/{CaseNumber?}` | `leap_sentencing_dialog_ui.py` |
| Criminal > Jail / CC Plea | `/criminal/jail-cc-plea/{CaseNumber?}` | `jail_cc_plea_dialog_ui.py` |
| Criminal > Sentencing Only (Already Plead) | `/criminal/sentencing-only-already-plead/{CaseNumber?}` | `sentencing_only_already_plead_dialog_ui.py` |
| Criminal > Not Guilty Plea | `/criminal/not-guilty-plea/{CaseNumber?}` | `not_guilty_plea_dialog_ui.py` |
| Criminal > Not Guilty / Appear / Bond Special | `/criminal/not-guilty-appear-bond-special` | `not_guilty_appear_bond_special_dialog_ui.py` |
| Criminal > Appear on Warrant (No Plea) | `/criminal/appear-on-warrant-no-plea/{CaseNumber?}` | `appear_on_warrant_no_plea_dialog_ui.py` |
| Criminal > Bond Modification / Revocation | `/criminal/bond-modification-revocation/{CaseNumber?}` | `bond_modification_revocation_dialog_ui.py` |
| Criminal > Community Service Secondary | `/criminal/community-service-secondary` | `community_service_secondary_dialog_ui.py` |
| Criminal > Arraignment / FTA / Bond | `/criminal/arraignment-fta-bond/{CaseNumber?}` | `arraignment_fta_bond_dialog_ui.py` |
| Criminal > Deny Privileges / Permit Retest | `/criminal/deny-privileges-permit-retest/{CaseNumber?}` | `deny_privileges_permit_retest_dialog_ui.py` |
| Scheduling > Trial to Court Notice | `/scheduling/trial-to-court-notice-of-hearing` | `trial_to_court_hearing_dialog_ui.py` |
| Scheduling > General Notice of Hearing | `/scheduling/general-notice-of-hearing` | `general_notice_of_hearing_dialog_ui.py` |
| Scheduling > Scheduling Entry | `/scheduling/scheduling-entry` | `sched_entry_dialogs.py` |
| Probation > Violation Bond | `/probation/violation-bond/{CaseNumber?}` | `probation_violation_bond_dialog_ui.py` |
| Probation > Community Control Terms | `/probation/community-control-terms/{CaseNumber?}` | `community_control_terms_dialog_ui.py` |
| Probation > CC Terms & Violation Notice | `/probation/community-control-terms-notices/{CaseNumber?}` | `community_control_terms_notices_dialog_ui.py` |
| Admin > Fiscal Journal Entry | `/admin/workflows-fiscal` | `workflows_fiscal_dialog_ui.py` |
| Admin > Juror Payment | `/admin/juror-payment` | `juror_payment_dialog_ui.py` |
| Admin > Time to Pay Order | `/admin/time-to-pay-order` | `time_to_pay_order_dialog_ui.py` |
| Notices > Civil Freeform Notice | `/notices/notices-freeform-civil/{CaseNumber?}` | `notices_freeform_civil_dialog_ui.py` |
| Notices > Civil Freeform Entry | `/notices/civil-freeform-entry/{CaseNumber?}` | `civil_freeform_entry_dialog_ui.py` |
| Driving > Driving Privileges | `/driving/driving-privileges/{CaseNumber?}` | `driving_privileges_dialog_ui.py` |

---

## API Test Coverage

Integration tests live in `munientry-net/api.Tests/` using `WebApplicationFactory<Program>`.
Every test asserts HTTP 2xx and — for DOCX endpoints — a `wordprocessingml` content-type with
non-empty bytes. **87 test methods across 36 test files.**

GET endpoint tests (case search, daily list, driving case) inject a fake in-memory service via
`ConfigureTestServices` so no real SQL Server connection is required.

### Criminal

| Test File | API Endpoint |
|---|---|
| `AppearOnWarrantNoPleaApiTests.cs` | `POST /api/appearonwarrantnoplea` |
| `ArraignmentContinuanceApiTests.cs` | `POST /api/arraignmentcontinuance` |
| `BondModificationRevocationApiTests.cs` | `POST /api/bondmodificationrevocation` |
| `CommunityServiceSecondaryApiTests.cs` | `POST /api/communityservicesecondary` |
| `DenyPrivilegesPermitRetestApiTests.cs` | `POST /api/denyprivilegespermitretest` |
| `DiversionDialogApiTests.cs` | `POST /api/diversiondialog` |
| `DiversionPleaApiTests.cs` | `POST /api/diversionplea` |
| `FineOnlyApiTests.cs` | `GET /api/fineonly/{CaseNumber}` · `POST /api/fineonlyplea` |
| `JailCcPleaApiTests.cs` | `POST /api/jailccplea` |
| `LeapAdmissionAlreadyValidApiTests.cs` | `POST /api/leapadmissionalreadyvalid` |
| `LeapAdmissionPleaApiTests.cs` | `POST /api/leapadmissionplea` |
| `LeapSentencingApiTests.cs` | `POST /api/leapsentencing` |
| `LeapValidSentencingApiTests.cs` | `POST /api/leapvalidsentencing` |
| `NotGuiltyAppearBondSpecialApiTests.cs` | `POST /api/notguiltyappearbondspecial` |
| `NotGuiltyPleaApiTests.cs` | `POST /api/notguiltyplea` |
| `PleaOnlyFutureSentencingApiTests.cs` | `POST /api/pleaonlyfuturesentencing` |
| `SentencingOnlyAlreadyPleadApiTests.cs` | `POST /api/sentencingonlyalreadypead` |
| `TrialSentencingApiTests.cs` | `POST /api/trialsentencing` |

### Probation

| Test File | API Endpoint |
|---|---|
| `CommunityControlTermsApiTests.cs` | `POST /api/communitycontrolterms` |
| `CommunityControlTermsNoticesTests.cs` | `POST /api/communitycontroltermsnotices` |
| `ProbationViolationBondApiTests.cs` | `POST /api/probationviolationbond` |

### Driving

| Test File | API Endpoint |
|---|---|
| `DrivingPrivilegesTests.cs` | `POST /api/drivingprivileges` |

### Notices

| Test File | API Endpoint |
|---|---|
| `CivilFreeformEntryTests.cs` | `POST /api/civilfreeformentry` |
| `NoticesFreeformCivilTests.cs` | `POST /api/noticesfreeformcivil` |

### Scheduling

| Test File | API Endpoint |
|---|---|
| `BondHearingApiTests.cs` | `POST /api/bondhearing` |
| `FinalJuryNoticeApiTests.cs` | `POST /api/finaljury` |
| `GeneralNoticeOfHearingApiTests.cs` | `POST /api/generalnoticeofhearing` |
| `SchedulingEntryApiTests.cs` | `POST /api/schedulingentry` (3 variants: Rohrer · Fowler · Hemmeter) |
| `TrialToCourtNoticeApiTests.cs` | `POST /api/trialtocourt` |

### Admin

| Test File | API Endpoint |
|---|---|
| `FiscalJournalEntryTests.cs` | `POST /api/fiscaljournalentry` |
| `JurorPaymentApiTests.cs` | `POST /api/jurorpayment` |
| `TimeToPayOrderApiTests.cs` | `POST /api/timetopayorder` |

### Data Queries

GET endpoints that return JSON from SQL stored procedures. Each test uses a fake in-memory
service (no SQL Server needed); tests cover happy-path, not-found, and error paths.

| Test File | API Endpoint | Tests |
|---|---|---|
| `CaseSearchApiTests.cs` | `GET /api/case/search/{caseNumber}` | Found→200+charges, unknown→404, defendant fields, charge/statute fields |
| `DailyListApiTests.cs` | `GET /api/dailylist/{listType}/{date}` | All 6 list types→200, case-insensitive list type, empty list, unknown type→400, bad date→400, slash-date routing, DTO shape |
| `DrivingCaseApiTests.cs` | `GET /api/drivingcase/{caseNumber}` | Found→200+DTO, defendant info, license/address, unknown→404, all DTO fields populated |

> `FiscalJournalEntryTests`, `CaseSearchApiTests`, `DailyListApiTests`, and `DrivingCaseApiTests`
> all inject fake/stub services so no real SQL Server connection is required during testing.

### Pure Unit Tests

Logic-only tests — no HTTP server, no database, no fakes required.

| Test File | What it tests |
|---|---|
| `DailyListStoredProcsTests.cs` | `DailyListStoredProcs.GetProcName()` and `ValidTypes`: all 6 valid list types map to the correct stored procedure name; case-insensitivity; invalid/partial/empty inputs return `null`; `ValidTypes` has exactly 6 entries; all proc names are unique and follow `[reports].[DMCMuniEntry*]` format |

### Pages Without API Endpoints

| Blazor Page | Notes |
|---|---|
| `Criminal/ArraignmentFtaBond.razor` | Client-side only; no DOCX generation |
| `Criminal/SealingDenyPrivileges.razor` | Client-side only; no DOCX generation |
| `Workflows/PretrialHemmeterMattoxMagistrate.razor` | Client-side only; no DOCX generation |
| `AddSecondary/AddAmendDialogs.razor` | Navigation / launcher page |
