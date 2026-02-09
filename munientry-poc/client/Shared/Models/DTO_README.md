# DTOs in client/Shared/Models

## Purpose
This folder contains all Data Transfer Objects (DTOs) used by the Blazor client for form submissions, API communication, and data binding. Each DTO here should have a matching definition in the API (api/Data) to ensure consistent data contracts between client and server.

## Structure
- All DTOs use the namespace: `Munientry.Poc.Client.Shared.Models`
- DTOs are named according to their form or data purpose (e.g., `ArraignmentContinuanceDto`, `DrivingCaseInfoDto`).
- Each DTO should match the fields and types of its API counterpart.

## Maintenance Guidelines
- When adding a new form, create or update the corresponding DTO in this folder.
- Keep DTOs in sync with the API: update fields, types, and names as needed.
- Remove obsolete DTOs when forms are retired or replaced.
- Do not use or reference the old `client/Dtos` folder; it is now obsolete.


## DTO List (Location: client/Shared/Models)

```
ArraignmentContinuanceDto.cs
BondHearingDto.cs
CivilComplaintDto.cs
CivilFreeformEntryDto.cs
CivilProtectionOrderDto.cs
CivilSummonsDto.cs
CommunityControlTermsDto.cs
CommunityControlTermsNoticesDto.cs
CriminalComplaintDto.cs
CriminalSummonsDto.cs
DenyPrivilegesPermitRetestDto.cs
DiversionDto.cs
DrivingCaseInfoDto.cs
DrivingPrivilegesDto.cs
FinalJuryNoticeDto.cs
FineOnlyDto.cs
FineOnlyPleaDto.cs
FiscalJournalEntryDto.cs
GeneralNoticeOfHearingDto.cs
JurorPaymentDto.cs
JuryDemandDto.cs
LeapSentencingDto.cs
NoticesFreeformCivilDto.cs
ProbationExtensionDto.cs
ProbationNoticeDto.cs
ProbationOrderDto.cs
ProbationTerminationDto.cs
ProbationViolationBondDto.cs
ProbationViolationDto.cs
SchedulingEntryDto.cs
TimeToPayOrderDto.cs
TrialToCourtNoticeDto.cs
WarrantRequestDto.cs
```

All DTOs above are located in: `client/Shared/Models/`

## Next Steps
- Fill in any stubbed DTOs with the correct fields from the API.
- Delete the old `client/Dtos` folder if not needed.

---
_Last updated: February 7, 2026_