# MuniEntry POC Setup & Forms Guide

## Project Setup

To set up and run the MuniEntry Proof-of-Concept project, use the following commands from the `munientry-net` directory:

```sh
docker compose build api   # Build the API service (multi-project: includes Munientry.DocxTemplating library)
docker compose build client   # Build the Blazor client app
docker compose up --no-build # Start all services (API and client)
```

- The client will be available at [http://localhost:3000](http://localhost:3000) (or as configured in your docker/nginx setup).
- The API will be available at [http://localhost:5000](http://localhost:5000) (or as configured).

## Included Forms (Side Navigation)

Below are the main forms available from the side navigation, along with the legacy Python forms they replace:

| Blazor Form Page | Legacy Python Dialog/UI |
|------------------|------------------------|
| Criminal > Fine Only Plea | fine_only_plea_dialog_ui.py |
| Criminal > Diversion Plea | diversion_plea_dialog_ui.py |
| Criminal > Not Guilty Plea | not_guilty_plea_dialog_ui.py |
| Criminal > Leap Admission Already Valid | leap_admission_already_valid_dialog_ui.py |
| Criminal > Leap Sentencing | leap_sentencing_dialog_ui.py |
| Criminal > Leap Valid Sentencing | leap_valid_sentencing_dialog_ui.py |
| Criminal > Sentencing Only Already Plead | sentencing_only_already_plead_dialog_ui.py |
| Criminal > Trial Sentencing | trial_sentencing_dialog_ui.py |
| Criminal > Plea Only Future Sentencing | plea_only_future_sentencing_dialog_ui.py |
| Criminal > Deny Privileges Permit Retest | deny_privileges_permit_retest_dialog_ui.py |
| Criminal > Community Service Secondary | community_service_secondary_dialog_ui.py |
| Criminal > Arraignment FTA Bond | arraignment_fta_bond_dialog_ui.py |
| Criminal > Bond Modification Revocation | bond_modification_revocation_dialog_ui.py |
| Criminal > Arraignment Continuance | arraignment_continuance_dialog_ui.py |
| Criminal > Appear On Warrant No Plea | appear_on_warrant_no_plea_dialog_ui.py |
| Notices > Civil Freeform Entry | civil_freeform_entry_dialog_ui.py |
| Notices > Notices Freeform Civil | notices_freeform_civil_dialog_ui.py |
| Scheduling > Trial To Court Notice Of Hearing | trial_to_court_notice_of_hearing_dialog_ui.py |
| Scheduling > General Notice Of Hearing | general_notice_of_hearing_dialog_ui.py |
| Scheduling > Final Jury Notice Of Hearing | final_jury_notice_of_hearing_dialog_ui.py |
| Scheduling > Bond Hearing | bond_hearing_dialog_ui.py |
| Scheduling > Scheduling Entry | sched_entry_dialogs.py |
| Probation > Probation Violation Bond | probation_violation_bond_dialog_ui.py |
| Probation > Community Control Terms Notices | community_control_terms_notices_dialog_ui.py |
| Probation > Community Control Terms | community_control_terms_dialog_ui.py |
| Driving > Driving Privileges | driving_privileges_dialog_ui.py |
| Add Secondary > Add/Amend Dialogs | add_amend_dialogs_ui.py |
| Admin > Workflows Fiscal | workflows_fiscal_dialog_ui.py |
| Admin > Time To Pay Order | time_to_pay_order_dialog_ui.py |
| Admin > Juror Payment | juror_payment_dialog_ui.py |
| Workflows > Pretrial Hemmeter Mattox Magistrate | pretrial_hemmeter_mattox_magistrate_dialog_ui.py |

> **Note:** This list is based on the current navigation and may change as new forms are added or legacy dialogs are replaced.

## Running Locally (Without Docker)

To run the API directly on Windows with Windows Authentication against a local SQL Server instance:

```sh
cd api
dotnet run
```

The API will start on `http://localhost:5000`. The default `appsettings.json` connection string uses
`Integrated Security=True`, so no credentials need to be configured — the process runs as the
currently logged-in domain account.

The Blazor client can still be served via Docker while the API runs locally, or run separately with:

```sh
cd client
dotnet run
```

## Docker + Database Configuration

Windows Authentication is not supported in Linux containers. To use Docker, create a `.env` file
in the `munientry-net/` directory (never commit this file) and set:

```
AUTHORITY_COURT_CONNSTR=Server=host.docker.internal;Database=AuthorityCourt;User Id=<SQL_LOGIN>;Password=<PASSWORD>;TrustServerCertificate=True;
```

`docker-compose.yml` injects this as the connection string override at runtime. If this variable
is not set, the container falls back to the `appsettings.json` default (Windows Auth), which will
not connect from inside a Linux container.

See [`docs/StoredProcedureIntegration.md`](docs/StoredProcedureIntegration.md) for full connection string documentation.

## Additional Notes
- Make sure Docker Desktop is running before executing the above commands.
- For development, you may want to stop and rebuild containers after making changes to the client or API code.

## Documentation

| File | Contents |
|---|---|
| [`docs/StoredProcedureIntegration.md`](docs/StoredProcedureIntegration.md) | SQL Server stored procedures, connection strings, case search & daily list endpoints |
| [`docs/DTO_Reference.md`](docs/DTO_Reference.md) | DTO inventory, namespace conventions, `FormPageBase<TDto>` pattern |
| [`docs/Blazor_DOCX_Migration.md`](docs/Blazor_DOCX_Migration.md) | Form-by-form migration status: Python → Blazor DOCX generation |
| [`docs/EntraID_Setup.md`](docs/EntraID_Setup.md) | Step-by-step guide to enable Microsoft Entra ID authentication |
| `Munientry.DocxTemplating/DocxTemplateProcessor.cs` | Portable class library: DOCX template filling engine (run-consolidation, Jinja2 token replacement, header/footer support) |

## Client Architecture

All criminal, probation, and driving entry forms (14 total) share a common base class: `FormPageBase<TDto>`
in `client/Shared/FormPageBase.cs`. This eliminates per-form boilerplate and centralises:

| Concern | How it's handled |
|---|---|
| Case pre-population | `OnParametersSetAsync` → `CaseSearchService.SearchCaseAsync` → `PopulateFromCaseAsync` override |
| DOCX download | `HandleValidSubmit` POSTs via `ICriminalFormApiClient`, detects `wordprocessingml` content-type, triggers JS `downloadFile` interop |
| Loading / submitting state | `IsLoadingCase` / `IsSubmitting` booleans — all form buttons are disabled while either is true |
| URL resolution | `ApiHelper.GetApiBaseUrl()` — resolves API base URL dynamically at runtime |

Each form only needs to implement:
1. `protected override string ApiEndpoint` — the relative POST path (e.g. `"api/diversionplea"`)
2. `protected override Task PopulateFromCaseAsync(...)` — map case search results onto the model

See [`docs/DTO_Reference.md`](docs/DTO_Reference.md) and [`docs/StoredProcedureIntegration.md`](docs/StoredProcedureIntegration.md) for more detail.

## API Test Coverage

All integration tests live in `api.Tests/` and use `WebApplicationFactory<Program>` against the real
`Program.cs` endpoint registrations. Every test asserts an HTTP 2xx response and — for DOCX-generating
endpoints — a `application/vnd.openxmlformats-officedocument.wordprocessingml.document` content-type
with non-empty bytes. **35 test methods across 32 test files.**

### Criminal

| Test File | Blazor Page | API Endpoint | Test Methods |
|---|---|---|---|
| `AppearOnWarrantNoPleaApiTests.cs` | `Criminal/AppearOnWarrantNoPlea.razor` | `POST /api/appearonwarrantnoplea` | 1 |
| `ArraignmentContinuanceApiTests.cs` | `Criminal/ArraignmentContinuance.razor` | `POST /api/arraignmentcontinuance` | 1 |
| `BondModificationRevocationApiTests.cs` | `Criminal/BondModificationRevocation.razor` | `POST /api/bondmodificationrevocation` | 1 |
| `CommunityServiceSecondaryApiTests.cs` | `Criminal/CommunityServiceSecondary.razor` | `POST /api/communityservicesecondary` | 1 |
| `DenyPrivilegesPermitRetestApiTests.cs` | `Criminal/DenyPrivilegesPermitRetest.razor` | `POST /api/denyprivilegespermitretest` | 1 |
| `DiversionDialogApiTests.cs` | `Criminal/DiversionDialog.razor` | `POST /api/diversiondialog` | 1 |
| `DiversionPleaApiTests.cs` | `Criminal/DiversionPlea.razor` | `POST /api/diversionplea` | 1 |
| `FineOnlyApiTests.cs` | `Criminal/FineOnlyPlea.razor` | `GET /api/fineonly/{CaseNumber}` · `POST /api/fineonlyplea` | 2 |
| `JailCcPleaApiTests.cs` | `Criminal/JailCcPlea.razor` | `POST /api/jailccplea` | 1 |
| `LeapAdmissionAlreadyValidApiTests.cs` | `Criminal/LeapAdmissionAlreadyValid.razor` | `POST /api/leapadmissionalreadyvalid` | 1 |
| `LeapAdmissionPleaApiTests.cs` | `Criminal/LeapAdmissionPlea.razor` | `POST /api/leapadmissionplea` | 1 |
| `LeapSentencingApiTests.cs` | `Criminal/LeapSentencing.razor` | `POST /api/leapsentencing` | 1 |
| `LeapValidSentencingApiTests.cs` | `Criminal/LeapValidSentencing.razor` | `POST /api/leapvalidsentencing` | 1 |
| `NotGuiltyAppearBondSpecialApiTests.cs` | `Criminal/NotGuiltyAppearBondSpecial.razor` | `POST /api/notguiltyappearbondspecial` | 1 |
| `NotGuiltyPleaApiTests.cs` | `Criminal/NotGuiltyPlea.razor` | `POST /api/notguiltyplea` | 1 |
| `PleaOnlyFutureSentencingApiTests.cs` | `Criminal/PleaOnlyFutureSentencing.razor` | `POST /api/pleaonlyfuturesentencing` | 1 |
| `SentencingOnlyAlreadyPleadApiTests.cs` | `Criminal/SentencingOnlyAlreadyPlead.razor` | `POST /api/sentencingonlyalreadypead` | 1 |
| `TrialSentencingApiTests.cs` | `Criminal/TrialSentencing.razor` | `POST /api/trialsentencing` | 1 |

### Probation

| Test File | Blazor Page | API Endpoint | Test Methods |
|---|---|---|---|
| `CommunityControlTermsApiTests.cs` | `Probation/CommunityControlTerms.razor` | `POST /api/communitycontrolterms` | 1 |
| `CommunityControlTermsNoticesTests.cs` | `Probation/CommunityControlTermsNotices.razor` | `POST /api/communitycontroltermsnotices` | 1 |
| `ProbationViolationBondApiTests.cs` | `Probation/ProbationViolationBond.razor` | `POST /api/probationviolationbond` | 1 |

### Driving

| Test File | Blazor Page | API Endpoint | Test Methods |
|---|---|---|---|
| `DrivingPrivilegesTests.cs` | `Driving/DrivingPrivileges.razor` | `POST /api/drivingprivileges` | 1 |

### Notices

| Test File | Blazor Page | API Endpoint | Test Methods |
|---|---|---|---|
| `CivilFreeformEntryTests.cs` | `Notices/CivilFreeformEntry.razor` | `POST /api/civilfreeformentry` | 1 |
| `NoticesFreeformCivilTests.cs` | `Notices/NoticesFreeformCivil.razor` | `POST /api/noticesfreeformcivil` | 1 |

### Scheduling

| Test File | Blazor Page | API Endpoint | Test Methods |
|---|---|---|---|
| `BondHearingApiTests.cs` | `Scheduling/BondHearing.razor` ⚠️ | `POST /api/bondhearing` | 1 |
| `FinalJuryNoticeApiTests.cs` | `Scheduling/FinalJuryNoticeOfHearing.razor` ⚠️ | `POST /api/finaljury` | 1 |
| `GeneralNoticeOfHearingApiTests.cs` | `Scheduling/GeneralNoticeOfHearing.razor` | `POST /api/generalnoticeofhearing` | 1 |
| `SchedulingEntryApiTests.cs` | `Scheduling/SchedulingEntry.razor` | `POST /api/schedulingentry` | 3 (Rohrer · Fowler · Hemmeter) |
| `TrialToCourtNoticeApiTests.cs` | `Scheduling/TrialToCourtNoticeOfHearing.razor` | `POST /api/trialtocourt` | 1 |

> ⚠️ `BondHearing.razor` and `FinalJuryNoticeOfHearing.razor` have their `@page` directives removed
> (content migrated to other forms), but their API endpoints remain registered and tested.

### Admin

| Test File | Blazor Page | API Endpoint | Test Methods |
|---|---|---|---|
| `FiscalJournalEntryTests.cs` | `Admin/WorkflowsFiscal.razor` | `POST /api/fiscaljournalentry` | 1 |
| `JurorPaymentApiTests.cs` | `Admin/JurorPayment.razor` | `POST /api/jurorpayment` | 1 |
| `TimeToPayOrderApiTests.cs` | `Admin/TimeToPayOrder.razor` | `POST /api/timetopayorder` | 1 |

> **Note:** `FiscalJournalEntryTests` injects a `FiscalJournalEntryServiceStub` (no-op) via
> `WithWebHostBuilder` so that no real SQL Server connection is required during testing.

### Pages Without API Endpoints (No Tests)

These Blazor pages render UI only — they have no corresponding API endpoint and no test:

| Blazor Page | Notes |
|---|---|
| `Criminal/ArraignmentFtaBond.razor` | Client-side only; no DOCX generation |
| `Criminal/SealingDenyPrivileges.razor` | Client-side only; no DOCX generation |
| `Workflows/PretrialHemmeterMattoxMagistrate.razor` | Client-side only; no DOCX generation |
| `AddSecondary/AddAmendDialogs.razor` | Navigation/launcher page |

