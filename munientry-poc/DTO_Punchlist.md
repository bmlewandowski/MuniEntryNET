# DTO Punchlist (as of 2026-02-07)

This document tracks DTOs that are referenced, have TODOs, or are otherwise in question regarding their necessity, service association, and form linkage. Each entry includes a recommendation for next steps.

## DTO Linkage to Blazor Navigation Forms

### Linked to Navigation Forms
| DTO Name                     | Linked Form(s)                          |
|-----------------------------|------------------------------------------|
| DiversionDto                 | Diversion.razor, ApiTestDiversion.razor  |
| DiversionDialogDto           | Diversion.razor, ApiTestDiversion.razor  |
| DrivingPrivilegesDto         | Driving/DrivingPrivileges.razor          |
| AppearOnWarrantNoPleaDto     | Criminal/AppearOnWarrantNoPlea.razor     |
| ArraignmentContinuanceDto    | Criminal/ArraignmentContinuance.razor    |
| LeapSentencingDto            | Criminal/LeapSentencing.razor            |
| LeapAdmissionPleaDto         | Criminal/LeapAdmissionPlea.razor         |
| LeapAdmissionAlreadyValidDto | Criminal/LeapAdmissionAlreadyValid.razor |
| PleaOnlyFutureSentencingDto  | Criminal/PleaOnlyFutureSentencing.razor  |
| NotGuiltyPleaDialogDto       | Criminal/NotGuiltyPleaDialog.razor       |
| NotGuiltyPleaDto             | Criminal/NotGuiltyPlea.razor             |
| NotGuiltyAppearBondSpecialDto| Criminal/NotGuiltyAppearBondSpecial.razor|
| DenyPrivilegesPermitRetestDto| Criminal/SealingDenyPrivileges.razor     |
| TrialToCourtNoticeDto        | Criminal/TrialSentencing.razor           |

### Not Linked to Navigation Forms
| DTO Name                     | Notes                                    |
|-----------------------------|------------------------------------------|
| WarrantRequestDto            | No matching form in navigation            |
| ProbationTerminationDto      | No matching form in navigation            |
| ProbationOrderDto            | No matching form in navigation            |
| ProbationNoticeDto           | No matching form in navigation            |
| ProbationExtensionDto        | No matching form in navigation            |
| JuryDemandDto                | No matching form in navigation            |
| CriminalSummonsDto           | No matching form in navigation            |
| CriminalComplaintDto         | No matching form in navigation            |

## DTOs Missing in api/Data and api/Services

| DTO Name                | Referenced/TODO | Service Exists | Form Exists | Recommendation/Notes |
|-------------------------|-----------------|---------------|-------------|---------------------|
| WarrantRequestDto       | TODO/Referenced | No            | ?           | Review if still needed. If yes, create DTO/service and associate with form. |
| ProbationTerminationDto | TODO/Referenced | No            | ?           | Review if still needed. If yes, create DTO/service and associate with form. |
| ProbationOrderDto       | TODO/Referenced | No            | ?           | Review if still needed. If yes, create DTO/service and associate with form. |
| ProbationNoticeDto      | TODO/Referenced | No            | ?           | Review if still needed. If yes, create DTO/service and associate with form. |
| ProbationExtensionDto   | TODO/Referenced | No            | ?           | Review if still needed. If yes, create DTO/service and associate with form. |
| JuryDemandDto           | TODO/Referenced | No            | ?           | Review if still needed. If yes, create DTO/service and associate with form. |
| CriminalSummonsDto      | TODO/Referenced | No            | ?           | Review if still needed. If yes, create DTO/service and associate with form. |
| CriminalComplaintDto    | TODO/Referenced | No            | ?           | Review if still needed. If yes, create DTO/service and associate with form. |


## DTOs Present and Linked

| DTO Name              | Service Exists | Form Exists | Notes |
|-----------------------|---------------|-------------|-------|
| TrialToCourtNoticeDto | Yes           | ?           | Present and linked. |
| LeapSentencingDto     | Yes           | ?           | Present and linked. |


## Action Items
