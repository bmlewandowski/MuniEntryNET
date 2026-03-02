# Stored Procedure Integration

## Overview

The API uses SQL Server stored procedures for all read operations against the `AuthorityCourt`
database. **There are no write operations — the system is read-only by design.** The legacy Python
application was read-only, and the client has confirmed this remains the design for the new
application. No SP, endpoint, or service for any write operation will be added unless the design
decision is explicitly changed.

The legacy Python source used a mix of:

1. **Stored procedures** — defined in `DAILY_CASE_LIST_STORED_PROCS` in
   `munientry/settings/app_settings.py`
2. **Hard-coded SQL queries** — defined in
   `munientry/sqlserver/crim_sql_server_queries.py`

The new application replaces both with stored procedures. The client confirmed that the underlying
SQL of each `[reports].[DMCMuniEntry*]` stored procedure aligns almost identically with the
corresponding legacy hard-coded query function.

---

## Connection String

### Running locally (`dotnet run`)

The default connection string in `appsettings.json` uses Windows Authentication. The API process
runs as the logged-in domain account and passes those credentials to SQL Server automatically — no
password configuration required:

```json
{
  "ConnectionStrings": {
    "AuthorityCourt": "Server=localhost;Database=AuthorityCourt;Integrated Security=True;TrustServerCertificate=True;"
  }
}
```

### Running in Docker

Windows Authentication is not supported in Linux containers. Set the `AUTHORITY_COURT_CONNSTR`
environment variable to a SQL login connection string. The easiest way is a `.env` file in the
`munientry-net/` directory:

```
AUTHORITY_COURT_CONNSTR=Server=host.docker.internal;Database=AuthorityCourt;User Id=<SQL_LOGIN>;Password=<PASSWORD>;TrustServerCertificate=True;
```

`host.docker.internal` resolves to the host machine from inside the Docker container, reaching
`localhost`'s SQL Server instance. The `docker-compose.yml` will inject this as
`ConnectionStrings__AuthorityCourt`, which ASP.NET Core maps directly to the `AuthorityCourt`
connection string, overriding `appsettings.json` at runtime.

> Do not commit `.env` files to source control — they are listed in `.gitignore`.

### NuGet package

The API uses `Microsoft.Data.SqlClient` (5.x) — the actively maintained Microsoft package.
`System.Data.SqlClient` is deprecated and has been removed.

---

## Test Database

The client has restored a copy of the production database on the SQL Server instance on the
development server (accessible via RDP). All 12 `[reports].[DMCMuniEntry*]` stored procedures
have been deployed and verified against this instance. Two issues were found and corrected by the
client before testing:

1. **Input parameters were not coded correctly** — updated and re-catalogued.
2. **Production SPs filter on `CAST(GETDATE() AS DATE)` internally** — altered for testing to
   return any data `> 01/01/2026` so there is data available during development.

The following test executions were used by the client to confirm correct behavior:

```sql
exec reports.DMCMuniEntryArraignment;
exec reports.DMCMuniEntryBenchTrials;
exec reports.DMCMuniEntryCaseDocket '25CRB01405';
exec reports.DMCMuniEntryCaseSearch '25CRB01405';
exec reports.DMCMuniEntryDrivingCaseSearch '25CRB01405';
exec reports.DMCMuniEntryEventReport 'FIN','01/02/2026';
exec reports.DMCMuniEntryFinalPreTrials;
exec reports.DMCMuniEntryFTAReport '01/02/2026';
exec reports.DMCMuniEntryNotGuiltyReport '01/02/2026';
exec reports.DMCMuniEntryPleas;
exec reports.DMCMuniEntryPrelimCommContViolHearings;
exec reports.DMCMuniEntrySlated;
```

---

## Stored Procedures — Production vs. Testing Behavior

Six SPs (**Arraignment, BenchTrials, FinalPreTrials, Pleas, PrelimCommContViolHearings, Slated**)
take **no parameters**. In production they filter on `CAST(GETDATE() AS DATE)` internally,
returning only today's scheduled cases. On the test database this filter has been relaxed to
`> 01/01/2026`. These SPs must **not** be passed extra parameters — SQL Server will raise
`"has too many arguments specified"`.

The remaining SPs (**CaseDocket, CaseSearch, DrivingCaseSearch, EventReport, FTAReport,
NotGuiltyReport**) take explicit input parameters — see the individual entries below.

---

## Complete Stored Procedure Catalogue

### Implementation Status Summary

| Stored Procedure | Parameters | Legacy Source | Status | API Endpoint |
|---|---|---|---|---|
| `DMCMuniEntryArraignment` | None | `app_settings.py` (SP) | ✅ Implemented | `GET /dailylist/arraignments` |
| `DMCMuniEntryBenchTrials` | None | `app_settings.py` (SP) | ✅ Implemented | `GET /dailylist/trials_to_court` |
| `DMCMuniEntryCaseDocket` | `@CaseNumber` | `get_case_docket_query` | ✅ Implemented | `GET /case/docket/{caseNumber}` |
| `DMCMuniEntryCaseSearch` | `@CaseNumber` | `general_case_search_query` | ✅ Implemented | `GET /case/search/{caseNumber}` |
| `DMCMuniEntryDrivingCaseSearch` | `@CaseNumber` | `driving_case_search_query` | ✅ Implemented | `GET /drivingcase/{caseNumber}` |
| `DMCMuniEntryEventReport` | `@EventCode`, `@EventDate` | `event_type_report_query` | ✅ Implemented — ⚠️ `@EventCode` format unconfirmed | `GET /reports/events/{code}/{date}` |
| `DMCMuniEntryFinalPreTrials` | None | `app_settings.py` (SP) | ✅ Implemented | `GET /dailylist/final_pretrial` |
| `DMCMuniEntryFTAReport` | `@EventDate` | `batch_fta_query` | ✅ Implemented | `GET /reports/fta/{date}` |
| `DMCMuniEntryNotGuiltyReport` | `@EventDate` | `not_guilty_report_query` | ✅ Implemented | `GET /reports/not-guilty/{date}` |
| `DMCMuniEntryPleas` | None | `app_settings.py` (SP) | ✅ Implemented | `GET /dailylist/pleas` |
| `DMCMuniEntryPrelimCommContViolHearings` | None | `app_settings.py` (SP) | ✅ Implemented | `GET /dailylist/pcvh_fcvh` |
| `DMCMuniEntrySlated` | None | `app_settings.py` (SP) | ✅ Implemented | `GET /dailylist/slated` |

All routes above are under the `/api/v1/` group prefix.

---

## Implemented Stored Procedures

### 1. Case Search — `[reports].[DMCMuniEntryCaseSearch]`

**Legacy equivalent:** `general_case_search_query(case_number)` in `crim_sql_server_queries.py`

**Purpose:** Returns all charges (SubCases) and defendant information for a given case number.
Used to pre-populate criminal entry dialogs (defendant name, charges, statute, degree, FRA status, defense counsel).

**Parameter:** `@CaseNumber` (string)

**Returns:** One row per charge on the case. Multiple charges on the same case = multiple rows.

**API Endpoint:**
```
GET /api/case/search/{caseNumber}
```

**Example:**
```
GET /api/case/search/23CRB00123
```

**Response shape** (see `shared/Dtos/CaseSearchResultDto.cs`):
```json
[
  {
    "subCaseNumber": "23CRB00123-A",
    "caseNumber": "23CRB00123",
    "subCaseId": 4512,
    "charge": "Speeding",
    "violationId": 891,
    "violationDetailId": 1042,
    "statute": "333.01",
    "degreeCode": "MM",
    "defFirstName": "John",
    "defLastName": "Doe",
    "movingBool": true,
    "violationDate": "2023-04-15T00:00:00",
    "endDate": null,
    "fraInFile": "N",
    "defenseCounsel": "Jane Smith",
    "pubDef": false
  }
]
```

**Service:** `Services/CaseData/CaseSearchService.cs`

---

### 2. Daily Case Lists — 6 Stored Procedures

**Legacy equivalent:** `DAILY_CASE_LIST_STORED_PROCS` map in `app_settings.py`

**Purpose:** Returns the scheduled cases for a given date for each hearing type.
Used to populate the daily case list boxes in the main window so staff can select a case to open a dialog for.

| `listType` param  | Stored Procedure                                    | Legacy key                  |
|-------------------|-----------------------------------------------------|-----------------------------|
| `arraignments`    | `[reports].[DMCMuniEntryArraignment]`               | `arraignments_cases_box`    |
| `slated`          | `[reports].[DMCMuniEntrySlated]`                    | `slated_cases_box`          |
| `pleas`           | `[reports].[DMCMuniEntryPleas]`                     | `pleas_cases_box`           |
| `pcvh_fcvh`       | `[reports].[DMCMuniEntryPrelimCommContViolHearings]`| `pcvh_fcvh_cases_box`       |
| `final_pretrial`  | `[reports].[DMCMuniEntryFinalPreTrials]`            | `final_pretrial_cases_box`  |
| `trials_to_court` | `[reports].[DMCMuniEntryBenchTrials]`               | `trials_to_court_cases_box` |

**Parameter:** `@ReportDate` (date)

**API Endpoint:**
```
GET /api/dailylist/{listType}/{date}
```

**Example:**
```
GET /api/dailylist/arraignments/2026-02-22
GET /api/dailylist/pleas/2026-02-22
```

**Response shape** (see `Data/DailyListResultDto.cs`):
```json
[
  {
    "time": "09:00",
    "caseNumber": "23CRB00123",
    "defFullName": "John Doe",
    "subCaseNumber": "23CRB00123-A",
    "charge": "Speeding",
    "eventId": "27",
    "judgeId": "3",
    "defenseCounsel": "Jane Smith"
  }
]
```

**Service:** `Services/CaseData/DailyListService.cs`
**SP map:** `shared/Dtos/DailyListResultDto.cs` → `DailyListStoredProcs` static class

---

## Files

| File | Purpose |
|------|---------|
| `shared/Dtos/CaseDocketEntryDto.cs` | DTO for `DMCMuniEntryCaseDocket` result rows |
| `shared/Dtos/EventReportResultDto.cs` | DTO for `DMCMuniEntryEventReport` result rows |
| `shared/Dtos/FtaReportResultDto.cs` | DTO for `DMCMuniEntryFTAReport` result rows |
| `shared/Dtos/NotGuiltyReportResultDto.cs` | DTO for `DMCMuniEntryNotGuiltyReport` result rows |
| `shared/Dtos/CaseSearchResultDto.cs` | DTO for `DMCMuniEntryCaseSearch` result rows |
| `shared/Dtos/DailyListResultDto.cs` | DTO for all 6 daily-list SP result rows + SP name map |
| `api/Services/CaseData/CaseSearchService.cs` | Executes `DMCMuniEntryCaseSearch` |
| `api/Services/CaseData/DrivingCaseService.cs` | Executes `DMCMuniEntryDrivingCaseSearch` |
| `api/Services/CaseData/CaseDocketService.cs` | Executes `DMCMuniEntryCaseDocket` |
| `api/Services/Reports/EventReportService.cs` | Executes `DMCMuniEntryEventReport` |
| `api/Services/Reports/FtaReportService.cs` | Executes `DMCMuniEntryFTAReport` |
| `api/Services/Reports/NotGuiltyReportService.cs` | Executes `DMCMuniEntryNotGuiltyReport` |
| `api/Services/CaseData/DailyListService.cs` | Executes one of the 6 daily-list SPs; forwards `@ReportDate` when `DailyList:PassDateParameter=true` |
| `api/Endpoints/CaseDataEndpoints.cs` | Maps `/case/search`, `/case/docket`, `/dailylist`, `/drivingcase` routes |
| `api/Endpoints/ReportsEndpoints.cs` | Maps `/reports/not-guilty`, `/reports/events`, `/reports/fta`, `/reports/fta/entry`, `/reports/fta/batch` routes |
| `api/ServiceRegistration.cs` | DI registrations for all services |
| `api/appsettings.json` | Connection string config + `DailyList:PassDateParameter` flag (default `false`) |
| `api/appsettings.Development.json` | Dev override — sets `DailyList:PassDateParameter: true` |
| `client/Shared/ReportsApiClient.cs` | Client HTTP wrapper for all report/docket endpoints |
| `shared/Dtos/CaseDocketEntryDto.cs` | Shared DTO used by both API and client for `CaseDocketEntryDto` |
| `shared/Dtos/NotGuiltyReportResultDto.cs` | Shared DTO used by both API and client for `NotGuiltyReportResultDto` |
| `shared/Dtos/EventReportResultDto.cs` | Shared DTO used by both API and client for `EventReportResultDto` |
| `shared/Dtos/FtaReportResultDto.cs` | Shared DTO used by both API and client for `FtaReportResultDto` |
| `client/Pages/Misc/CaseDocket.razor` | Case docket history page — accepts case number, displays Date/Remark table |
| `client/Pages/Reports/NotGuiltyReport.razor` | Not Guilty Report page — date picker, sortable table (Case Number/Defendant/Entry) |
| `client/Pages/Reports/EventReport.razor` | Event Report page — event type dropdown + date picker, sortable table |
| `client/Pages/Reports/FtaBatchReport.razor` | FTA Batch page — date picker, case preview list, ZIP download of all DOCXs |

---

## Client-Side Integration

All criminal/probation/driving entry forms inherit from `FormPageBase<TDto>` in
`client/Shared/FormPageBase.cs`. When a case number is provided (via `[Parameter] CaseNumber`),
the base class calls `CaseSearch.SearchCaseAsync(caseNumber)` through the injected
`CaseSearchApiClient`, hitting `GET /api/v1/case/search/{caseNumber}`. Each form overrides
`PopulateFromCaseAsync(results)` to map SP result rows onto its own `Model` properties.

`DrivingPrivileges` overrides `LoadCaseDataAsync` entirely to call
`GET /api/v1/drivingcase/{caseNumber}` via `DrivingCaseService`, since it uses a different SP
with a different result shape (`DrivingCaseInfoDto`).

The main window (`client/Pages/Index.razor`) calls
`DailyListApiClient.GetDailyListAsync(listType, date)` for each of the 6 list types, hitting
`GET /api/v1/dailylist/{listType}/{date}`. The date is accepted and forwarded to the SP **when
`DailyList:PassDateParameter=true`** (see the Date Parameter Handling section below).

### Report Pages

Four Blazor pages under `client/Pages/Reports/` and `client/Pages/Misc/` expose the four
report-oriented SPs directly to court staff:

| Page | Route | Python equivalent | Output |
|---|---|---|---|
| `CaseDocket.razor` | `/case/docket` or `/case/docket/{caseNumber}` | `CrimCaseDocket.get_docket()` in `crim_getters.py` | Sortable Date/Remark table; also linked from homepage "View Docket" action |
| `NotGuiltyReport.razor` | `/reports/not-guilty` | `run_not_guilty_report()` in `daily_reports.py` | Date picker → sortable table (Case Number, Defendant, Docket Entry) |
| `EventReport.razor` | `/reports/event` | `run_event_type_report()` in `authoritycourt_reports.py` | Event type dropdown + date → sortable table, default sort ascending by Time |
| `FtaBatchReport.razor` | `/reports/fta` | `run_batch_fta_process()` in `batch_menu.py` | Date → case preview list → "Generate All Entries" downloads ZIP of DOCXs |

All four report pages are linked from the **Reports** section in the sidebar navigation
(`client/Shared/MainLayout.razor`). `CaseDocket` is also reachable from the homepage action bar
via the "View Docket" button, which appears for every selected case regardless of list type.

### FTA Batch — DOCX Generation

The FTA Batch page calls two API endpoints:

1. `GET /api/v1/reports/fta/{date}` — fetches the list of FTA-eligible case numbers
   (`[reports].[DMCMuniEntryFTAReport]`).
2. `GET /api/v1/reports/fta/batch/{date}` — the API calls `FtaReportService` to get the case list,
   then for each case number calls `CaseSearchService.SearchCaseAsync` to get defendant name, fills
   `Batch_Failure_To_Appear_Arraignment_Template.docx`, and returns all DOCXs in a ZIP.

Individual DOCXs can also be downloaded case-by-case via
`GET /api/v1/reports/fta/entry/{caseNumber}/{date}` (per-row "Download" button on the page).

Template fields filled (matching `create_entry()` in `batch_menu.py`):

| Template token | Source | Example |
|---|---|---|
| `case_number` | SP result — `CaseNumber` | `22CRB01234` |
| `def_first_name` | `CaseSearchResultDto.DefFirstName` | `John` |
| `def_last_name` | `CaseSearchResultDto.DefLastName` | `Doe` |
| `case_event_date` | Event date formatted as `MMMM dd, yyyy` | `January 15, 2026` |
| `warrant_rule` | Derived from case number type code (chars 2–4) | `Criminal Rule 4` (CRB) or `Traffic Rule 7` (all others) |

### DOCX Output — UNC Share

The legacy Python application wrote generated `.docx` files to a UNC share accessible from court
workstations; staff printed directly from that share. The Blazor application returns generated
`.docx` files as a browser file download from the API
(`Content-Type: application/vnd.openxmlformats-officedocument.wordprocessingml.document`).
Court workstations save the download; an admin-managed default save path pointing to the same UNC
share can be configured at the OS or browser level. See `Blazor_DOCX_Migration.md` for form-by-form
DOCX generation coverage.

---

## Event Report — `@EventCode` Parameter (Pending DBA Confirmation)

> **⚠️ DBA Action Required before production use of the Event Report page.**

The Blazor `EventReport.razor` page passes the event type as a plain-English string matching the
keys of the legacy Python `EVENT_IDS` dictionary in `report_constants.py`:

| Dropdown label | Value sent to `@EventCode` |
|---|---|
| Arraignments | `Arraignments` |
| Final Pretrials | `Final Pretrials` |
| Trials To Court | `Trials To Court` |
| Pleas | `Pleas` |
| Jury Trials | `Jury Trials` |

The test database EXEC used `'FIN'` as the event code:

```sql
exec reports.DMCMuniEntryEventReport 'FIN','01/02/2026';
```

This suggests the SP may expect short database event-type codes rather than the longer plain-English
names. **The DBA must confirm** what value(s) `[reports].[DMCMuniEntryEventReport]` accepts for
`@EventCode`. Once confirmed:

1. Update the `EventOptions` list in `client/Pages/Reports/EventReport.razor` so each option's
   `Value` property carries the correct code (the `Text` label shown to the user can remain
   descriptive).
2. If the SP supports a broader set of event types than the 5 currently listed, add the additional
   options to the dropdown.

---

## Daily Case List — Date Parameter Handling

### How the date flows

```
Blazor client
  → GET /api/v1/dailylist/{listType}/{date:yyyy-MM-dd}
      ↓
DailyListService.GetDailyListAsync(listType, reportDate)
  → reads DailyList:PassDateParameter from configuration
        if true  → cmd.Parameters.AddWithValue("@ReportDate", reportDate.Date)
        if false → no parameter passed (SP uses GETDATE() internally)
      ↓
[reports].[DMCMuniEntry*]  (one of the 6 SPs)
```

The Blazor client (`DailyListApiClient`) always sends today's date in normal operation. A dev
tool or the Swagger UI can pass any date when `PassDateParameter=true`.

### Config switch

| File | Setting | Behavior |
|---|---|---|
| `api/appsettings.json` | `"PassDateParameter": false` | **Production default** — SP uses `GETDATE()` internally; no parameter forwarded |
| `api/appsettings.Development.json` | `"PassDateParameter": true` | **Dev/test** — `@ReportDate` forwarded; any date in the URL is used |

ASP.NET Core automatically layers `appsettings.Development.json` on top of `appsettings.json`
when `ASPNETCORE_ENVIRONMENT=Development` (the `docker-compose.yml` default for local work). No
code change, no rebuild — just the environment variable.

### Required DBA SP update (one-time)

Before enabling `PassDateParameter=true`, the DBA must apply this change to each of the 6 SPs
on the target database:

```sql
-- Example for DMCMuniEntryArraignment; repeat pattern for all 6.
ALTER PROCEDURE [reports].[DMCMuniEntryArraignment]
    @ReportDate DATE = NULL   -- NULL means "use today"
AS
BEGIN
    -- Replace the existing GETDATE() / today filter line with:
    WHERE CAST(EventDate AS DATE) = ISNULL(@ReportDate, CAST(GETDATE() AS DATE))
    -- ...rest of SP body unchanged...
END
```

**Production behavior is unchanged** — calling with no argument leaves `@ReportDate = NULL`,
so `ISNULL(NULL, CAST(GETDATE() AS DATE))` still evaluates to today.

Test executions to verify after the SP update:

```sql
-- No argument → today's data (production equivalent)
exec reports.DMCMuniEntryArraignment;

-- With a specific date → data for that day
exec reports.DMCMuniEntryArraignment '2026-01-15';
exec reports.DMCMuniEntryBenchTrials '2026-01-15';
exec reports.DMCMuniEntryFinalPreTrials '2026-01-15';
exec reports.DMCMuniEntryPleas '2026-01-15';
exec reports.DMCMuniEntryPrelimCommContViolHearings '2026-01-15';
exec reports.DMCMuniEntrySlated '2026-01-15';
```

---

## Adding a New SP / Endpoint

1. Create a DTO in `shared/Dtos/` mapping the SP result columns (automatically available to both API and client).
2. Create a service in the appropriate domain subfolder under `api/Services/<Domain>/` that opens a `SqlConnection`, calls the SP with `CommandType.StoredProcedure`, and maps the reader to the DTO. Implement the matching `IXxxService` interface in the same subfolder.
3. Register the service in `api/ServiceRegistration.cs` via `services.AddScoped<IXxxService, XxxService>()`.
4. Add a `MapGet` endpoint in the appropriate file under `api/Endpoints/`.

Use `api/Services/CaseData/CaseSearchService.cs` as the reference implementation.

### Pending SPs — Implementation Notes

The 4 previously pending SPs (`CaseDocket`, `EventReport`, `FTAReport`, `NotGuiltyReport`) are
now fully implemented. These were batch/reporting features in the legacy application that had
not yet been migrated:

- **CaseDocket** — was used per-dialog to show docket history; now `GET /case/docket/{caseNumber}`
- **EventReport** — was a menu-driven on-screen table report; now `GET /reports/events/{code}/{date}`
- **FTAReport** — drove the batch DOCX generation loop in `batch_menu.py`; now `GET /reports/fta/{date}` returns the FTA-eligible case numbers so the Blazor batch page can request a DOCX per case
- **NotGuiltyReport** — was a menu-driven on-screen table report; now `GET /reports/not-guilty/{date}`

All 12 `[reports].[DMCMuniEntry*]` stored procedures now have a corresponding API endpoint and
client-facing Blazor page.

---

## Legacy Query → Stored Procedure Mapping

All legacy hard-coded queries from `crim_sql_server_queries.py` map to `[reports].[DMCMuniEntry*]`
stored procedures. The SP underlying SQL was confirmed by the client to align almost identically
with the legacy query function.

| Legacy function | Stored procedure | API Status |
|---|---|---|
| `general_case_search_query` | `[reports].[DMCMuniEntryCaseSearch]` | ✅ `CaseSearchService.cs` |
| `driving_case_search_query` | `[reports].[DMCMuniEntryDrivingCaseSearch]` | ✅ `DrivingCaseService.cs` |
| `event_type_report_query` | `[reports].[DMCMuniEntryEventReport]` | ✅ `EventReportService.cs` |
| `get_case_docket_query` | `[reports].[DMCMuniEntryCaseDocket]` | ✅ `CaseDocketService.cs` |
| `batch_fta_query` | `[reports].[DMCMuniEntryFTAReport]` | ✅ `FtaReportService.cs` |
| `not_guilty_report_query` | `[reports].[DMCMuniEntryNotGuiltyReport]` | ✅ `NotGuiltyReportService.cs` |

The 6 daily-list SPs from `app_settings.py` (`DMCMuniEntryArraignment`, `DMCMuniEntrySlated`,
`DMCMuniEntryPleas`, `DMCMuniEntryPrelimCommContViolHearings`, `DMCMuniEntryFinalPreTrials`,
`DMCMuniEntryBenchTrials`) were already stored procedures in the legacy system and remain SPs in
the new application — see `DailyListService.cs`.

---

## Impacted Forms

### Forms that use `/api/case/search/{caseNumber}`

Entering a case number in these forms should trigger the case search SP to pre-populate defendant name, charges, statute, degree, defense counsel, and FRA status.

| Form | Razor File |
|------|------------|
| LEAP Admission - Already Valid | `client/Pages/Criminal/LeapAdmissionAlreadyValid.razor` |
| LEAP Admission Plea | `client/Pages/Criminal/LeapAdmissionPlea.razor` |
| LEAP Sentencing | `client/Pages/Criminal/LeapSentencing.razor` |
| Jail/CC Plea | `client/Pages/Criminal/JailCcPlea.razor` |
| Fine Only Plea | `client/Pages/Criminal/FineOnlyPlea.razor` |
| Fine Only Plea Only | `client/Pages/Criminal/FineOnlyPleaOnly.razor` |
| Diversion Plea | `client/Pages/Criminal/DiversionPlea.razor` |
| Appear on Warrant No Plea | `client/Pages/Criminal/AppearOnWarrantNoPlea.razor` |
| Bond Modification Revocation | `client/Pages/Criminal/BondModificationRevocation.razor` |
| Deny Privileges / Permit Retest | `client/Pages/Criminal/DenyPrivilegesPermitRetest.razor` |
| Probation Violation Bond | `client/Pages/Probation/ProbationViolationBond.razor` |
| Community Control Terms | `client/Pages/Probation/CommunityControlTerms.razor` |
| Community Control Terms Notices | `client/Pages/Probation/CommunityControlTermsNotices.razor` |
| Failure To Appear | `client/Pages/Criminal/FailureToAppear.razor` |
| Criminal Sealing Entry | `client/Pages/Criminal/SealingDenyPrivileges.razor` |
| Competency Evaluation | `client/Pages/Criminal/CompetencyEvaluation.razor` |
| Criminal Freeform Entry | `client/Pages/Criminal/FreeformEntry.razor` |

### Forms that use `/api/dailylist/{listType}/{date}`

These forms load a list of scheduled cases for a given hearing type and date, used to select a case before opening an entry dialog.

| Form | Razor File |
|------|------------|
| Main Window | `client/Pages/Misc/MainWindow.razor` |
| Trial to Court Notice of Hearing | `client/Pages/Scheduling/TrialToCourtNoticeOfHearing.razor` |
| General Notice of Hearing | `client/Pages/Scheduling/GeneralNoticeOfHearing.razor` |
| Final Jury Notice of Hearing | `client/Pages/Scheduling/FinalJuryNoticeOfHearing.razor` |
| Bond Hearing | `client/Pages/Scheduling/BondHearing.razor` |
### Forms that use the report/docket endpoints

| Page | Route | SP called |
|------|-------|-----------|
| Case Docket | `/case/docket` | `DMCMuniEntryCaseDocket` |
| Not Guilty Report | `/reports/not-guilty` | `DMCMuniEntryNotGuiltyReport` |
| Event Report | `/reports/event` | `DMCMuniEntryEventReport` |
| FTA Batch | `/reports/fta` | `DMCMuniEntryFTAReport` + `DMCMuniEntryCaseSearch` |
### Forms with a dedicated SP (already implemented)

| Form | Service | Stored Procedure |
|------|---------|-----------------|
| Driving Privileges | `Services/CaseData/DrivingCaseService.cs` | `[reports].[DMCMuniEntryDrivingCaseSearch]` |

### Not impacted (no case lookup needed)

| Form | Reason |
|------|--------|
| `Admin/JurorPayment.razor` | Admin form, no case search |
| `Admin/TimeToPayOrder.razor` | Admin form, no case search |
| `Admin/WorkflowsFiscal.razor` | Fiscal entry, no case search |
| `Notices/NoticesFreeformCivil.razor` | Civil freeform, no criminal SP |
| `Notices/CivilFreeformEntry.razor` | Civil freeform, no criminal SP |

---

_Last updated: March 2026 — client pages for all 4 report SPs added; FTA DOCX batch endpoint added; `@EventCode` pending DBA confirmation documented._
