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
The legacy `client/Dtos/` folder (namespace `Munientry.Poc.Client.Dtos`) still exists and contains 14 older DTOs.
Five forms still reference DTOs from that folder: `BondModificationRevocation`, `CommunityServiceSecondary`, `DiversionPlea`, `JailCcPlea`, and `LeapValidSentencing`.
All new form DTOs should be placed in `client/Shared/Models/` instead. The `client/Dtos/` folder can be
removed once those five forms' DTOs are migrated.

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
CaseSearchResultDto.cs          ← API response DTO for /api/case/search; not a form submission DTO
CivilComplaintDto.cs
CivilFreeformEntryDto.cs
CivilProtectionOrderDto.cs
CivilSummonsDto.cs
CommunityControlTermsDto.cs
CommunityControlTermsNoticesDto.cs
CommunityServiceSecondaryDto.cs
CriminalComplaintDto.cs
CriminalSummonsDto.cs
DenyPrivilegesPermitRetestDto.cs
DiversionDialogDto.cs
DiversionDto.cs
DrivingCaseInfoDto.cs
DrivingPrivilegesDto.cs
EntryType.cs                    ← Enum (not a DTO); used by DenyPrivilegesPermitRetestDto
FinalJuryNoticeDto.cs
FineOnlyDto.cs
FineOnlyPleaDto.cs
FiscalJournalEntryDto.cs
GeneralNoticeOfHearingDto.cs
JurorPaymentDto.cs
JuryDemandDto.cs
LeapAdmissionAlreadyValidDto.cs
LeapAdmissionPleaDto.cs
LeapSentencingDto.cs
NotGuiltyAppearBondSpecialDto.cs
NotGuiltyPleaDto.cs
NoticesFreeformCivilDto.cs
PleaOnlyFutureSentencingDto.cs
ProbationExtensionDto.cs
ProbationNoticeDto.cs
ProbationOrderDto.cs
ProbationTerminationDto.cs
ProbationViolationBondDto.cs
ProbationViolationDto.cs
SchedulingEntryDto.cs
SentencingOnlyAlreadyPleadDto.cs
TimeToPayOrderDto.cs
TrialToCourtNoticeDto.cs
WarrantRequestDto.cs
```

All files above are located in: `client/Shared/Models/`

---
_Last updated: February 28, 2026_