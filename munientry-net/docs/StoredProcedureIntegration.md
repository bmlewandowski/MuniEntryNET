# Stored Procedure Integration

## Overview

The API uses SQL Server stored procedures for all read operations against the `AuthorityCourt` database.
There are no write operations — the legacy application was read-only and the new application follows the same pattern.

The legacy Python source used a mix of:
1. **Stored procedures** — defined in `DAILY_CASE_LIST_STORED_PROCS` in `munientry/settings/app_settings.py`
2. **Hard-coded SQL queries** — defined in `munientry/sqlserver/crim_sql_server_queries.py`

The new application replaces both with stored procedures. The hard-coded queries have equivalent stored procedures on the server whose underlying SQL matches the legacy queries.

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

**Response shape** (see `Data/CaseSearchResultDto.cs`):
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

**Service:** `Services/CaseSearchService.cs`

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

**Service:** `Services/DailyListService.cs`
**SP map:** `Data/DailyListResultDto.cs` → `DailyListStoredProcs` static class

---

## Files

| File | Purpose |
|------|---------|
| `Data/CaseSearchResultDto.cs` | DTO for case search SP result rows |
| `Data/DailyListResultDto.cs` | DTO for daily list SP result rows + SP name map |
| `Services/CaseSearchService.cs` | Executes `DMCMuniEntryCaseSearch` SP |
| `Services/DailyListService.cs` | Executes one of the 6 daily list SPs |
| `Program.cs` | Registers services and maps endpoints |
| `appsettings.json` | Connection string config |

---

## Client-Side Integration

All criminal/probation/driving entry forms inherit from `FormPageBase<TDto>` in
`client/Shared/FormPageBase.cs`. When a case number is provided (via the `[Parameter] CaseNumber`
property), the base class automatically calls `CaseSearchService.SearchCaseAsync(caseNumber)`,
which hits `GET /api/case/search/{caseNumber}` and returns the results. Each form then overrides
`PopulateFromCaseAsync(results)` to map the SP result rows onto its `Model` properties.

`DrivingPrivileges` overrides `LoadCaseDataAsync` entirely to call `GET /api/drivingcase/{caseNumber}`
via a direct `HttpClient` call using `ApiHelper.GetApiBaseUrl()`, since it uses a different SP and
returns a different result shape (`DrivingCaseInfoDto`).

---

## Adding a New Stored Procedure

1. Create a DTO in `Data/` mapping the SP result columns.
2. Create a service in `Services/` that opens a `SqlConnection`, calls the SP with `CommandType.StoredProcedure`, and maps the reader to the DTO.
3. Register the service in `Program.cs` with `builder.Services.AddScoped<YourService>()`.
4. Add a `MapGet` or `MapPost` endpoint in `Program.cs`.

Follow the pattern in `CaseSearchService.cs` as the reference implementation.

---

## Implemented — Previously Listed as Outstanding

All legacy hard-coded queries from `crim_sql_server_queries.py` now have matching stored procedures
wired into the API:

| Legacy function | Stored procedure | Service |
|---|---|---|
| `event_type_report_query` | `[reports].[DMCMuniEntryEventReport]` | _(endpoint TBD)_ |
| `get_case_docket_query` | `[reports].[DMCMuniEntryCaseDocket]` | _(endpoint TBD)_ |
| `batch_fta_query` | `[reports].[DMCMuniEntryFTAReport]` | _(endpoint TBD)_ |
| `not_guilty_report_query` | `[reports].[DMCMuniEntryNotGuiltyReport]` | _(endpoint TBD)_ |
| `driving_case_search_query` | `[reports].[DMCMuniEntryDrivingCaseSearch]` | `DrivingCaseService.cs` |

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

### Forms that use `/api/dailylist/{listType}/{date}`

These forms load a list of scheduled cases for a given hearing type and date, used to select a case before opening an entry dialog.

| Form | Razor File |
|------|------------|
| Main Window | `client/Pages/Misc/MainWindow.razor` |
| Trial to Court Notice of Hearing | `client/Pages/Scheduling/TrialToCourtNoticeOfHearing.razor` |
| General Notice of Hearing | `client/Pages/Scheduling/GeneralNoticeOfHearing.razor` |
| Final Jury Notice of Hearing | `client/Pages/Scheduling/FinalJuryNoticeOfHearing.razor` |
| Bond Hearing | `client/Pages/Scheduling/BondHearing.razor` |

### Forms with a dedicated SP (already implemented)

| Form | Service | Stored Procedure |
|------|---------|-----------------|
| Driving Privileges | `Services/DrivingCaseService.cs` | `[reports].[DMCMuniEntryDrivingCaseSearch]` |

### Not impacted (no case lookup needed)

| Form | Reason |
|------|--------|
| `Admin/JurorPayment.razor` | Admin form, no case search |
| `Admin/TimeToPayOrder.razor` | Admin form, no case search |
| `Admin/WorkflowsFiscal.razor` | Fiscal entry, no case search |
| `Notices/NoticesFreeformCivil.razor` | Civil freeform, no criminal SP |
| `Notices/CivilFreeformEntry.razor` | Civil freeform, no criminal SP |

---

_Last updated: February 28, 2026_
