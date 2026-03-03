# DTOs — shared/Dtos

## Purpose
All Data Transfer Objects (DTOs) used by both the Blazor client and the API live in a single shared library: `shared/Munientry.Shared.csproj`. This eliminates the client/server drift that existed when each project maintained its own copy.

> **See also:** [Blazor_DOCX_Migration.md](Blazor_DOCX_Migration.md) — maps each form to its DTO, API endpoint, SP, and legacy Python equivalent.

## Structure
- All DTOs use the namespace: `Munientry.Shared.Dtos`
- Source location: `shared/Dtos/*.cs`
- Validation attributes (`PastDateAttribute`, `FutureDateAttribute`) use the namespace `Munientry.Shared.Validation` and live in `shared/Validation/`.
- DTOs are named according to their form or data purpose (e.g., `ArraignmentContinuanceDto`, `DrivingCaseInfoDto`).
- `EntryType.cs` in this folder is a shared enum, not a DTO — it is used by `DenyPrivilegesPermitRetestDto` to distinguish entry types.

## Maintenance Guidelines
- When adding a new form, create or update the corresponding DTO in `shared/Dtos/` — **one file, used by both the API and the client**.
- Remove obsolete DTOs when forms are retired or replaced.
- All three projects (`Munientry.Api`, `Munientry.Client`, `api.Tests`) reference `Munientry.Shared` via `<ProjectReference>`.

## Base Class and Typed Client Infrastructure

All 35 entry forms (`Criminal/`, `Probation/`, `Driving/`, `Notices/`, `Scheduling/`, `Admin/`) now inherit from `FormPageBase<TDto>` defined in
`client/Shared/FormPageBase.cs`. This base class:

- Holds the typed `TDto Model` instance used for form binding.
- Manages `IsSubmitting` / `IsLoadingCase` UI state.
- Calls `CaseSearch.SearchCaseAsync` (via the injected `CaseSearchApiClient`) to pre-populate the model on case number load.
- POSTs the model to the API via `IEntryFormApiClient` and handles DOCX streaming or text result display.

The typed HTTP client interface is `IEntryFormApiClient` (`client/Shared/IEntryFormApiClient.cs`),
implemented by `EntryFormApiClient` (`client/Shared/EntryFormApiClient.cs`) and registered in
`Program.cs` as a scoped service using `ApiHelper.GetApiBaseUrl()` for the base address.

## DTO List (Location: shared/Dtos/)

```
AppearOnWarrantNoPleaDto.cs
ArraignmentContinuanceDto.cs
BondHearingDto.cs
BondModificationRevocationDto.cs
CaseSearchResultDto.cs          ← API response DTO for /api/case/search; not a form submission DTO
ChargeItemDto.cs                ← Per-charge row for LEAP Admission forms (Offense, Statute, Degree, Plea)
CivilFreeformEntryDto.cs
CommunityControlTermsDto.cs
CommunityControlTermsNoticesDto.cs
CommunityServiceSecondaryDto.cs
DenyPrivilegesPermitRetestDto.cs
DiversionDialogDto.cs
DiversionPleaDto.cs
DrivingCaseInfoDto.cs
DrivingPrivilegesDto.cs
EntryType.cs                    ← Enum (not a DTO); used by DenyPrivilegesPermitRetestDto
FinalJuryNoticeDto.cs
FineOnlyDto.cs
FineOnlyPleaDto.cs
FiscalJournalEntryDto.cs
GeneralNoticeOfHearingDto.cs
JailCcPleaDto.cs
JurorPaymentDto.cs
LeapAdmissionAlreadyValidDto.cs
LeapAdmissionPleaDto.cs
LeapSentencingDto.cs
LeapValidSentencingDto.cs
NotGuiltyAppearBondSpecialDto.cs
NotGuiltyPleaDto.cs
NoticesFreeformCivilDto.cs
PleaOnlyFutureSentencingDto.cs
ProbationViolationBondDto.cs
SchedulingEntryDto.cs
SentencingChargeItemDto.cs      ← Per-charge row for sentencing/jail forms (adds Finding, FinesAmount, FinesSuspended, JailDays, JailDaysSuspended)
SentencingOnlyAlreadyPleadDto.cs
TimeToPayOrderDto.cs
TrialToCourtNoticeDto.cs
CompetencyEvaluationDto.cs
CriminalFreeformEntryDto.cs
CriminalSealingEntryDto.cs
FailureToAppearDto.cs
```

All files above are located in: `shared/Dtos/`

---

## Known Limitations / TODOs

### Judicial Officer Auto-Population

> **TODO**: The fields `judicial_officer.first_name`, `judicial_officer.last_name`, and `judicial_officer.officer_type` are currently hardcoded as empty strings in **all** API endpoints in `api/Program.cs`.
>
> These must be populated from the authenticated user's session or claims once Entra ID authentication is fully wired. When a judge logs in, their name and role should be resolved from the token claims and injected into the DOCX template automatically, removing the need for manual entry.
>
> See `docs/EntraID_Setup.md` for the authentication setup plan.

### Aggregate Reports

All aggregate report features are now fully migrated:

- Not Guilty Report — `GET /api/v1/reports/not-guilty/{date}`
- Event Type Reports — `GET /api/v1/reports/events/{code}/{date}`
- Batch FTA processing — `GET /api/v1/reports/fta/{date}` + `GET /api/v1/reports/fta/batch`

---
_Last updated: June 2026_