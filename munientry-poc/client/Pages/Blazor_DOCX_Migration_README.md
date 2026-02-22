# MuniEntry Blazor Migration: DOCX Generation Coverage

This document summarizes the migration status of legacy Python forms to Blazor, specifically focusing on DOCX generation capability.

## Form-by-Form Comparison: Python vs Blazor

| Form Name / Entry Type                | Legacy Python (DOCX) | Blazor (DOCX) | Blazor Route                          | Status         |
|---------------------------------------|----------------------|--------------|---------------------------------------|----------------|
| Arraignment Continuance Entry         | Yes                  | Yes          | /criminal/arraignmentcontinuance      | Migrated       |
| Fine Only Judgment Entry              | Yes                  | Yes          | /criminal/fineonlyplea                | Migrated       |
| Jail CC Judgment Entry                | Yes                  | Yes          | /criminal/jail-cc-plea                | Migrated       |
| Diversion Judgment Entry              | Yes                  | Yes          | /criminal/diversion-plea              | Migrated       |
| Not Guilty Bond Entry                 | Yes                  | Yes          | /criminal/not-guilty-plea             | Migrated       |
| No Plea Bond Entry                    | Yes                  | Yes          | /criminal/appear-on-warrant-no-plea   | Migrated       |
| Probation Violation Bond Entry        | Yes                  | Yes          | /probation/violation-bond             | Migrated       |
| Failure To Appear Entry               | Yes                  | Yes          | /criminal/arraignment-fta-bond        | Migrated       |
| Freeform Entry                        | Yes                  | Yes          | /notices/notices-freeform-civil       | Migrated       |
| Bond Hearing Entry                    | Yes                  | Yes          | /scheduling/bond-hearing              | Migrated       |
| Plea Only Entry                       | Yes                  | Yes          | /criminal/plea-only-future-sentencing | Migrated       |
| Rohrer Scheduling Entry               | Yes                  | Yes          | /scheduling/rohrer-scheduling         | Migrated       |
| Fowler Scheduling Entry               | Yes                  | Yes          | /scheduling/fowler-scheduling         | Migrated       |
| Final And Jury Notice Hearing Entry   | Yes                  | Yes          | /scheduling/trial-to-court-notice-of-hearing | Migrated |
| General Notice Of Hearing Entry       | Yes                  | Yes          | /scheduling/general-notice-of-hearing | Migrated       |
| Trial To Court Notice Hearing Entry   | Yes                  | Yes          | /scheduling/trial-to-court-notice-of-hearing | Migrated |
| Civil Freeform Entry                  | Yes                  | Yes          | /notices/civil-freeform-entry         | Migrated       |
| Criminal Sealing Entry                | Yes                  | Yes          | /criminal/sealing-deny-privileges     | Migrated       |
| Terms Of Community Control Entry      | Yes                  | Yes          | /probation/community-control-terms    | Migrated       |
| Notice Of Community Control Violation Entry | Yes           | Yes          | /probation/community-control-terms-notices | Migrated |
| Competency Evaluation Entry           | Yes                  | Yes          | /admin/competency-evaluation          | Migrated       |
| Driving Privileges Entry              | Yes                  | Yes          | /driving/driving-privileges           | Migrated       |
| Deny Privileges Entry                 | Yes                  | Yes          | /criminal/deny-privileges-permit-retest | Migrated   |
| Leap Admission Plea Entry             | Yes                  | Yes          | /criminal/leap-admission-plea         | Migrated       |
| Leap Admission Plea Valid Entry       | Yes                  | Yes          | /criminal/leap-admission-already-valid | Migrated   |
| Leap Sentencing Judgment Entry        | Yes                  | Yes          | /criminal/leap-sentencing             | Migrated       |
| Sentencing Judgment Entry             | Yes                  | Yes          | /criminal/trial-sentencing            | Migrated       |

---

**Summary:**
- All legacy Python forms with DOCX generation have been migrated to Blazor with DOCX generation enabled.
- No forms with legacy DOCX templates are left behind; all are accessible and functional in Blazor.

_Last updated: February 22, 2026_
