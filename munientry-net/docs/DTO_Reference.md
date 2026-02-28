# DTOs in client/Shared/Models

## Purpose
This folder contains all Data Transfer Objects (DTOs) used by the Blazor client for form submissions, API communication, and data binding. Each DTO here should have a matching definition in the API (`api/Data`) to ensure consistent data contracts between client and server.

## Structure
- All DTOs use the namespace: `Munientry.Poc.Client.Shared.Models`
- DTOs are named according to their form or data purpose (e.g., `ArraignmentContinuanceDto`, `DrivingCaseInfoDto`).
- Each DTO should match the fields and types of its API counterpart.
- `EntryType.cs` in this folder is a shared enum, not a DTO — it is used by `DenyPrivilegesPermitRetestDto` to distinguish entry types.

## Maintenance Guidelines
- When adding a new form, create or update the corresponding DTO in this folder.
- Keep DTOs in sync with the API: update fields, types, and names as needed.
- Remove obsolete DTOs when forms are retired or replaced.

## `client/Dtos` folder
The `client/Dtos/` folder has been removed. All DTOs now live in `client/Shared/Models/`.

## Base Class and Typed Client Infrastructure

All 14 entry forms (`Criminal/`, `Probation/`, `Driving/`) now inherit from `FormPageBase<TDto>` defined in
`client/Shared/FormPageBase.cs`. This base class:

- Holds the typed `TDto Model` instance used for form binding.
- Manages `IsSubmitting` / `IsLoadingCase` UI state.
- Calls `CaseSearchService.SearchCaseAsync` to pre-populate the model on case number load.
- POSTs the model to the API via `ICriminalFormApiClient` and handles DOCX streaming or text result display.

The typed HTTP client interface is `ICriminalFormApiClient` (`client/Shared/ICriminalFormApiClient.cs`),
implemented by `CriminalFormApiClient` (`client/Shared/CriminalFormApiClient.cs`) and registered in
`Program.cs` as a scoped service using `ApiHelper.GetApiBaseUrl()` for the base address.

## DTO List (Location: client/Shared/Models)

```
AppearOnWarrantNoPleaDto.cs
ArraignmentContinuanceDto.cs
BondHearingDto.cs
BondModificationRevocationDto.cs
CaseSearchResultDto.cs          ← API response DTO for /api/case/search; not a form submission DTO
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
SentencingOnlyAlreadyPleadDto.cs
TimeToPayOrderDto.cs
TrialToCourtNoticeDto.cs
CompetencyEvaluationDto.cs
CriminalFreeformEntryDto.cs
CriminalSealingEntryDto.cs
FailureToAppearDto.cs
```

All files above are located in: `client/Shared/Models/`

---

## Known Limitations / TODOs

### Judicial Officer Auto-Population

> **TODO**: The fields `judicial_officer.first_name`, `judicial_officer.last_name`, and `judicial_officer.officer_type` are currently hardcoded as empty strings in **all** API endpoints in `api/Program.cs`.
>
> These must be populated from the authenticated user's session or claims once Entra ID authentication is fully wired. When a judge logs in, their name and role should be resolved from the token claims and injected into the DOCX template automatically, removing the need for manual entry.
>
> See `docs/EntraID_Setup.md` for the authentication setup plan.

### Aggregate Reports

The following Python aggregate report features are out of scope for the initial Blazor migration and will be addressed separately:

- Not Guilty Report
- Courtroom A / B / C Event Reports
- Event Type Reports
- Batch FTA processing

---
_Last updated: June 2026_