# MuniEntry Blazor Migration: DOCX Generation Coverage

This document summarizes the migration status of legacy Python forms to Blazor, specifically focusing on DOCX generation capability.

## Form-by-Form Comparison: Python vs Blazor

| Form Name / Entry Type                          | Legacy Python (DOCX) | Blazor (DOCX) | Blazor Route                                    | API Endpoint                    | Status    |
|-------------------------------------------------|----------------------|---------------|-------------------------------------------------|---------------------------------|-----------|
| Appear on Warrant / No Plea Bond                | Yes                  | Yes           | /criminal/appear-on-warrant-no-plea             | appearonwarrantnoplea           | Migrated  |
| Arraignment Continuance Entry                   | Yes                  | Yes           | /criminal/ArraignmentContinuance                | arraignmentcontinuance          | Migrated  |
| Arraignment FTA Bond Entry                      | Yes                  | Yes           | /criminal/arraignment-fta-bond                  | bondhearing                     | Migrated  |
| Bond Modification / Revocation Entry            | Yes                  | Yes           | /criminal/bond-modification-revocation          | bondmodificationrevocation      | Migrated  |
| Civil Freeform Entry                            | Yes                  | Yes           | /notices/civil-freeform-entry                   | civilfreeformentry              | Migrated  |
| Community Control Terms Entry                   | Yes                  | Yes           | /probation/community-control-terms              | communitycontrolterms           | Migrated  |
| Community Control Terms Notices Entry           | Yes                  | Yes           | /probation/community-control-terms-notices      | communitycontroltermsnotices    | Migrated  |
| Community Service Secondary Dialog              | Yes                  | Yes           | /criminal/community-service-secondary           | communityservicesecondary       | Migrated  |
| Deny Privileges / Permit Retest Entry           | Yes                  | Yes           | /criminal/deny-privileges-permit-retest         | denyprivilegespermitretest      | Migrated  |
| Diversion Dialog                                | Yes                  | Yes           | /criminal/diversion-dialog                      | diversiondialog                 | Migrated  |
| Diversion Plea Entry                            | Yes                  | Yes           | /criminal/diversion-plea                        | diversionplea                   | Migrated  |
| Driving Privileges Entry                        | Yes                  | Yes           | /driving/driving-privileges                     | drivingprivileges               | Migrated  |
| Fine Only Judgment Entry                        | Yes                  | Yes           | /criminal/fineonlyplea                          | fineonlyplea                    | Migrated  |
| Fine Only Plea Only Entry                       | Yes                  | Yes           | /criminal/fine-only-plea-only                   | fineonly                        | Migrated  |
| General Notice Of Hearing Entry                 | Yes                  | Yes           | /scheduling/general-notice-of-hearing           | generalnoticeofhearing          | Migrated  |
| Jail CC Judgment Entry                          | Yes                  | Yes           | /criminal/jail-cc-plea                          | jailccplea                      | Migrated  |
| Juror Payment Entry                             | Yes                  | Yes           | /admin/juror-payment                            | jurorpayment                    | Migrated  |
| LEAP Admission Already Valid Entry              | Yes                  | Yes           | /criminal/leap-admission-already-valid          | leapadmissionalreadyvalid       | Migrated  |
| LEAP Admission Plea Entry                       | Yes                  | Yes           | /criminal/leap-admission-plea                   | leapadmissionplea               | Migrated  |
| LEAP Sentencing Judgment Entry                  | Yes                  | Yes           | /criminal/leap-sentencing                       | leapsentencing                  | Migrated  |
| LEAP Valid Sentencing Entry                     | Yes                  | Yes           | /criminal/leap-valid-sentencing                 | leapvalidsentencing             | Migrated  |
| Not Guilty Appear Bond Special Dialog           | Yes                  | Yes           | /criminal/not-guilty-appear-bond-special        | notguiltyappearbondspecial      | Migrated  |
| Not Guilty Plea Entry                           | Yes                  | Yes           | /criminal/not-guilty-plea                       | notguiltyplea                   | Migrated  |
| Notices Freeform Civil Entry                    | Yes                  | Yes           | /notices/notices-freeform-civil                 | noticesfreeformcivil            | Migrated  |
| Plea Only / Future Sentencing Entry             | Yes                  | Yes           | /criminal/plea-only-future-sentencing           | pleaonlyfuturesentencing        | Migrated  |
| Probation Violation Bond Entry                  | Yes                  | Yes           | /probation/violation-bond                       | probationviolationbond          | Migrated  |
| Sentencing Only Already Plead Entry             | Yes                  | Yes           | /criminal/sentencing-only-already-plead         | sentencingonlyalreadypead       | Migrated  |
| Time To Pay Order                               | Yes                  | Yes           | /admin/time-to-pay-order                        | timetopayorder                  | Migrated  |
| Trial Sentencing Judgment Entry                 | Yes                  | Yes           | /criminal/trial-sentencing                      | trialsentencing                 | Migrated  |
| Trial To Court / Final Jury Notice Of Hearing   | Yes                  | Yes           | /scheduling/trial-to-court-notice-of-hearing    | trialtocourt                    | Migrated  |
| Criminal Sealing / Deny Privileges Entry        | Yes                  | No            | /criminal/sealing-deny-privileges               | —                               | Stub      |
| Scheduling Entry (Rohrer / Fowler / Hemmeter)   | Yes                  | No            | —                                               | schedulingentry                 | No page   |

---

## Utility / Non-DOCX Pages

These pages exist in Blazor but do not generate DOCX documents:

| Page                              | Route                                          | Notes                          |
|-----------------------------------|------------------------------------------------|--------------------------------|
| Main Window                       | /misc/main-window                              | UI shell                       |
| Workflows Fiscal                  | /admin/workflows-fiscal                        | Workflow launcher              |
| Add / Amend Charge Dialogs        | /addsecondary/add-amend-dialogs                | Secondary charge editing UI    |
| Pretrial Hemmeter / Magistrate    | /workflows/pretrial-hemmeter-mattox-magistrate | Workflow form                  |

---

**Summary:**
- 30 of 32 DOCX-generating forms are fully migrated end-to-end.
- 1 form is a page stub with no form or submit logic: **Criminal Sealing / Deny Privileges** (`/criminal/sealing-deny-privileges`)
- 1 form has an API endpoint but no Blazor page built yet: **Scheduling Entry** (Rohrer / Fowler / Hemmeter)
- All templates generate DOCX via `GenerateDocxBytes()` — no database writes from Blazor forms.

_Last updated: February 27, 2026_
