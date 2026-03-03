# MuniEntry Blazor Migration: Form → Stored Procedure → DOCX → Legacy Python Mapping

This document maps every Blazor form to its data-load stored procedure, whether it generates a DOCX, its DOCX template, and its equivalent legacy Python form.

All criminal/traffic case-based forms share a common data-load SP: `[reports].[DMCMuniEntryCaseSearch]` (replaces the legacy ad-hoc `general_case_search_query` inline SQL in `crim_sql_server_queries.py`). Exceptions are called out per section.

The Blazor app never writes back to the database. All DOCX generation is handled server-side by `DocxTemplateProcessor.FillTemplate()` in `api/DocxTemplateProcessor.cs`.

> **Related docs:** [DTO_Reference.md](DTO_Reference.md) — shared DTO list and `FormPageBase<TDto>` pattern | [StoredProcedureIntegration.md](StoredProcedureIntegration.md) — SP details, connection strings, test database | [Legacy_Save_Paths_And_Batch_FTA.md](Legacy_Save_Paths_And_Batch_FTA.md) — how the legacy app saved files

---

## Criminal / Traffic

| Blazor Form | API Endpoint | Data SP (load) | Generates DOCX | Template | Legacy Python Form |
|---|---|---|---|---|---|
| `NotGuiltyPlea.razor` | `POST /notguiltyplea` | `[reports].[DMCMuniEntryCaseSearch]` | Yes | `Not_Guilty_Bond_Template.docx` | `not_guilty_bond_dialog.NotGuiltyBondDialog` |
| `NotGuiltyAppearBondSpecial.razor` | `POST /notguiltyappearbondspecial` | `[reports].[DMCMuniEntryCaseSearch]` | Yes | `Not_Guilty_Bond_Template.docx` | `not_guilty_bond_dialog.NotGuiltyBondDialog` (special bond variant) |
| `FineOnlyPlea.razor` | `POST /fineonlyplea` | `[reports].[DMCMuniEntryCaseSearch]` | Yes | `Fine_Only_Plea_Final_Judgment_Template.docx` | `fine_only_plea_dialog.FineOnlyPleaDialog` |
| `FineOnlyPleaOnly.razor` | *(data-entry only, uses `FineOnlyDto`)* | `[reports].[DMCMuniEntryCaseSearch]` | No (data scaffold) | — | `fine_only_plea_dialog.FineOnlyPleaDialog` / `plea_only_dialog` (plea-only sub-view) |
| `JailCcPlea.razor` | `POST /jailccplea` | `[reports].[DMCMuniEntryCaseSearch]` | Yes | `Jail_Plea_Final_Judgment_Template.docx` | `jail_cc_plea_dialog.JailCCPleaDialog` |
| `ArraignmentContinuance.razor` | `POST /arraignmentcontinuance` | `[reports].[DMCMuniEntryCaseSearch]` | Yes | `Arraignment_Continue_Template.docx` | `arraignment_continue_dialog.ArraignmentContinueDialog` |
| `ArraignmentFtaBond.razor` | *(stub — merged into above routes)* | `[reports].[DMCMuniEntryCaseSearch]` | Yes | `Batch_Failure_To_Appear_Arraignment_Template.docx` | `arraignment_continue_dialog`, `failure_to_appear_dialog`, `bond_hearing_dialog`, `no_plea_bond_dialog`, `not_guilty_bond_dialog` (merged stub) |
| `FailureToAppear.razor` | `POST /failuretoappear` | `[reports].[DMCMuniEntryCaseSearch]` | Yes | `Failure_To_Appear_Template.docx` | `failure_to_appear_dialog.FailureToAppearDialog` |
| `AppearOnWarrantNoPlea.razor` | `POST /appearonwarrantnoplea` | `[reports].[DMCMuniEntryCaseSearch]` | Yes | `No_Plea_Bond_Template.docx` | `no_plea_bond_dialog.NoPleaBondDialog` |
| `DiversionDialog.razor` | `POST /diversiondialog` | `[reports].[DMCMuniEntryCaseSearch]` | Yes | `Diversion_Template.docx` | `diversion_dialog.DiversionPleaDialog` |
| `DiversionPlea.razor` | `POST /diversionplea` | `[reports].[DMCMuniEntryCaseSearch]` | Yes | `Diversion_Template.docx` | `diversion_dialog.DiversionPleaDialog` (plea variant) |
| `PleaOnlyFutureSentencing.razor` | `POST /pleaonlyfuturesentencing` | `[reports].[DMCMuniEntryCaseSearch]` | Yes | `Plea_Only_Template.docx` | `plea_only_future_sentence_dialog.PleaOnlyDialog` |
| `SentencingOnlyAlreadyPlead.razor` | `POST /sentencingonlyalreadyplead` | `[reports].[DMCMuniEntryCaseSearch]` | Yes | `Sentencing_Only_Template.docx` | `sentencing_only_dialog.SentencingOnlyDialog` |
| `TrialSentencing.razor` | `POST /trialsentencing` | `[reports].[DMCMuniEntryCaseSearch]` | Yes | `Trial_Sentencing_Template.docx` | `trial_sentencing_dialog.TrialSentencingDialog` |
| `LeapAdmissionPlea.razor` | `POST /leapadmissionplea` | `[reports].[DMCMuniEntryCaseSearch]` | Yes | `Leap_Admission_Plea_Template.docx` | `leap_plea_dialog.LeapAdmissionPleaDialog` |
| `LeapAdmissionAlreadyValid.razor` | `POST /leapadmissionalreadyvalid` | `[reports].[DMCMuniEntryCaseSearch]` | Yes | `Leap_Admission_Plea_Valid_Template.docx` | `leap_plea_valid_dialog.LeapPleaValidDialog` |
| `LeapSentencing.razor` | `POST /leapsentencing` | `[reports].[DMCMuniEntryCaseSearch]` | Yes | `Leap_Sentencing_Template.docx` | `leap_sentencing_dialog.LeapSentencingDialog` |
| `LeapValidSentencing.razor` | `POST /leapvalidsentencing` | `[reports].[DMCMuniEntryCaseSearch]` | Yes | `Leap_Sentencing_Template.docx` | `leap_sentencing_dialog.LeapSentencingDialog` (valid/already-admitted variant) |
| `FreeformEntry.razor` | `POST /freeformentry` | `[reports].[DMCMuniEntryCaseSearch]` | Yes | `Freeform_Entry_Template.docx` | `freeform_dialog.FreeformDialog` |
| `SealingDenyPrivileges.razor` | `POST /criminalsealing` | `[reports].[DMCMuniEntryCaseSearch]` | Yes | `Criminal_Sealing_Entry_Template.docx` | `criminal_sealing_dialog.CriminalSealingDialog` |
| `CompetencyEvaluation.razor` | `POST /competencyevaluation` | `[reports].[DMCMuniEntryCaseSearch]` | Yes | `Competency_Evaluation_Template.docx` | `competency_dialog.CompetencyDialog` |
| `CommunityServiceSecondary.razor` | `POST /communityservicesecondary` | `[reports].[DMCMuniEntryCaseSearch]` | Yes | *(secondary conditions appended)* | `secondary/add_community_control_dialog.py` |
| `DenyPrivilegesPermitRetest.razor` | `POST /denyprivilegespermitretest` | `[reports].[DMCMuniEntryCaseSearch]` | Yes | `Deny_Privileges_Template.docx` | `deny_privileges_dialog.DenyPrivilegesDialog` |

---

## Bond

| Blazor Form | API Endpoint | Data SP (load) | Generates DOCX | Template | Legacy Python Form |
|---|---|---|---|---|---|
| `BondModificationRevocation.razor` | `POST /bondmodificationrevocation` | `[reports].[DMCMuniEntryCaseSearch]` | Yes | `Bond_Hearing_Template.docx` | `bond_hearing_dialog.BondHearingDialog` (modification/revocation variant) |
| `ProbationViolationBond.razor` | `POST /probationviolationbond` | `[reports].[DMCMuniEntryCaseSearch]` | Yes | `Probation_Violation_Bond_Template.docx` | `probation_violation_bond_dialog.ProbationViolationBondDialog` |

> Note: `Scheduling/BondHearing.razor` is an empty stub — migrated into `ArraignmentFtaBond.razor`.

---

## Driving

| Blazor Form | API Endpoint | Data SP (load) | Generates DOCX | Template | Legacy Python Form |
|---|---|---|---|---|---|
| `DrivingPrivileges.razor` | `POST /drivingprivileges` | `[reports].[DMCMuniEntryDrivingCaseSearch]` | Yes | `Driving_Privileges_Template.docx` | `driving_privileges_dialog.DrivingPrivilegesDialog` |

> Driving uses its own dedicated SP (`DMCMuniEntryDrivingCaseSearch`) which replaces the legacy `driving_case_search_query` inline SQL in `crim_sql_server_queries.py`. It pulls from `[AuthorityCourt].[dbo].[TicketImport]` in addition to `CaseMaster`/`CasePerson`.

---

## Probation

| Blazor Form | API Endpoint | Data SP (load) | Generates DOCX | Template | Legacy Python Form |
|---|---|---|---|---|---|
| `CommunityControlTerms.razor` | `POST /communitycontrolterms` | `[reports].[DMCMuniEntryCaseSearch]` | Yes | `Terms_Of_Community_Control_Template.docx` | `terms_comm_control_dialog.TermsCommControlDialog` |
| `CommunityControlTermsNotices.razor` | `POST /communitycontroltermsnotices` | `[reports].[DMCMuniEntryCaseSearch]` | Yes | `Notice_CC_Violation_Template.docx` | `notice_cc_violation_dialog.NoticeCCViolationDialog` |

---

## Scheduling / Notices

| Blazor Form | API Endpoint | Data SP (load) | Generates DOCX | Template | Legacy Python Form |
|---|---|---|---|---|---|
| `SchedulingEntry.razor` | `POST /schedulingentry` | `[reports].[DMCMuniEntrySlated]` / `[reports].[DMCMuniEntryArraignment]` (via daily list) | Yes | `Scheduling_Entry_Template_{Fowler/Hemmeter/Rohrer}.docx` | `sched_entry_dialogs.SchedulingEntryDialog` |
| `FinalJuryNoticeOfHearing.razor` | `POST /finaljury` | `[reports].[DMCMuniEntryCaseSearch]` | Yes | `Final_Jury_Notice_Of_Hearing_Template.docx` | `final_jury_hearing_notice_dialog.FinalJuryNoticeHearingDialog` |
| `GeneralNoticeOfHearing.razor` | `POST /generalnoticeofhearing` | `[reports].[DMCMuniEntryCaseSearch]` | Yes | `General_Notice_Of_Hearing_Template.docx` | `general_hearing_notice_dialog.GeneralNoticeOfHearingDialog` |
| `TrialToCourtNoticeOfHearing.razor` | `POST /trialtocourt` | `[reports].[DMCMuniEntryCaseSearch]` | Yes | `Trial_To_Court_Hearing_Notice_Template.docx` | `trial_to_court_hearing_notice_dialog.TrialToCourtHearingDialog` |

---

## Admin / Fiscal

| Blazor Form | API Endpoint | Data SP (load) | Generates DOCX | Template | Legacy Python Form |
|---|---|---|---|---|---|
| `WorkflowsFiscal.razor` | `POST /fiscaljournalentry` | None (manual account entry — no DB lookup) | Yes | `Admin_Fiscal_Template.docx` | `admin_fiscal_dialog.AdminFiscalDialog` |
| `TimeToPayOrder.razor` | `POST /timetopayorder` | `[reports].[DMCMuniEntryCaseSearch]` | Yes | `Time_To_Pay_Template.docx` | `time_to_pay_dialog.TimeToPayDialog` |
| `JurorPayment.razor` | `POST /jurorpayment` | None (manual juror data entry — no DB lookup) | Yes | `Jury_Payment_Template.docx` | `jury_payment_dialog.JuryPaymentDialog` |

---

## Civil / Notices-Civil

| Blazor Form | API Endpoint | Data SP (load) | Generates DOCX | Template | Legacy Python Form |
|---|---|---|---|---|---|
| `CivilFreeformEntry.razor` | `POST /civilfreeformentry` | `[AuthorityCivil].[dbo]` inline query (`general_civil_case_query` in `civil_sql_server_queries.py`) — no SP yet | Yes | `Civil_Freeform_Entry_Template.docx` | `civ_freeform_dialog.CivFreeformDialog` |
| `NoticesFreeformCivil.razor` | `POST /noticesfreeformcivil` | `[AuthorityCivil].[dbo]` inline query (same as above) — no SP yet | Yes | *(civil notices freeform variant)* | `civ_freeform_dialog.CivFreeformDialog` (notices variant) |

> Civil forms still use legacy inline SQL against `[AuthorityCivil].[dbo]`. No stored procedure has been created for that database yet.

---

## Reports (read-only — no DOCX except FTA batch)

| Blazor Form | API Endpoint | Stored Procedure | Generates DOCX | Legacy Python Equivalent |
|---|---|---|---|---|
| `NotGuiltyReport.razor` | `GET /reports/not-guilty/{date}` | `[reports].[DMCMuniEntryNotGuiltyReport]` | No | `daily_reports.run_not_guilty_report()` / `run_not_guilty_report_today()` |
| `EventReport.razor` | `GET /reports/events/{eventCode}/{date}` | `[reports].[DMCMuniEntryEventReport]` | No | `authoritycourt_reports.run_event_type_report()` via `event_type_report_query` |
| `FtaBatchReport.razor` | `GET /reports/fta/{date}` → `GET /reports/fta/batch/{date}` | `[reports].[DMCMuniEntryFTAReport]` + `[reports].[DMCMuniEntryCaseSearch]` | Yes (ZIP of DOCXs per case) | `batch_menu.run_batch_fta_process()` / `create_fta_entries()` |

---

## Miscellaneous / Utility (no DOCX)

| Blazor Form | API Endpoint | Stored Procedure | Generates DOCX | Legacy Python Equivalent |
|---|---|---|---|---|
| `CaseDocket.razor` | `GET /case/docket/{caseNumber}` | `[reports].[DMCMuniEntryCaseDocket]` | No | `crim_getters.CrimCaseDocket.get_docket()` via `get_case_docket_query` |
| `MainWindow.razor` | `GET /dailylist/{listType}/{date}` | `[reports].[DMCMuniEntryArraignment]`, `[reports].[DMCMuniEntrySlated]`, `[reports].[DMCMuniEntryPleas]`, `[reports].[DMCMuniEntryPrelimCommContViolHearings]`, `[reports].[DMCMuniEntryFinalPreTrials]`, `[reports].[DMCMuniEntryBenchTrials]` | No | `main_window.py` / `criminal_caseload_functions.py` daily list loaders |

---

## Add/Secondary & Workflow Stubs (not yet wired to independent endpoints)

| Blazor Form | Status | Generates DOCX | Legacy Python Equivalent |
|---|---|---|---|
| `AddAmendDialogs.razor` | Stub — no backend endpoint yet | No | `secondary/add_community_control_dialog.py`, `add_conditions_dialog.py`, `add_jail_only_dialog.py`, `add_special_bond_conditions_dialog.py`, `charges/add_charge_dialog.py`, `charges/amend_charge_dialog.py` |
| `PretrialHemmeterMattoxMagistrate.razor` | Stub — no backend endpoint yet | Workflow-dependent | `dialogs/workflows/probation_dw_dialogs.PretrialWorkflowDialog`, `admin_judge_dw_dialog.MagistrateAdoptionWorkflowDialog`, `AdminWorkflowDialog` |

---

## All Data-Load Stored Procedures Summary

| Stored Procedure | Purpose | Used By |
|---|---|---|
| `[reports].[DMCMuniEntryCaseSearch]` | Primary criminal/traffic case lookup: charges, defendant info, defense counsel | All criminal/traffic, bond, probation, scheduling/notices, sealing, competency, time-to-pay forms |
| `[reports].[DMCMuniEntryDrivingCaseSearch]` | Driving case lookup: defendant address, license, ticket import data | `DrivingPrivileges.razor` |
| `[reports].[DMCMuniEntryCaseDocket]` | Chronological docket entries for a case | `CaseDocket.razor` |
| `[reports].[DMCMuniEntryArraignment]` | Daily arraignment list | `MainWindow.razor`, `SchedulingEntry.razor` |
| `[reports].[DMCMuniEntrySlated]` | Daily slated list | `MainWindow.razor`, `SchedulingEntry.razor` |
| `[reports].[DMCMuniEntryPleas]` | Daily plea list | `MainWindow.razor` |
| `[reports].[DMCMuniEntryPrelimCommContViolHearings]` | PCVH/FCVH daily list | `MainWindow.razor` |
| `[reports].[DMCMuniEntryFinalPreTrials]` | Final pretrial daily list | `MainWindow.razor` |
| `[reports].[DMCMuniEntryBenchTrials]` | Trials to court daily list | `MainWindow.razor` |
| `[reports].[DMCMuniEntryNotGuiltyReport]` | Not-guilty plea / continuance report by arraignment date | `NotGuiltyReport.razor` |
| `[reports].[DMCMuniEntryEventReport]` | Cases by event type and date | `EventReport.razor` |
| `[reports].[DMCMuniEntryFTAReport]` | FTA-eligible case numbers from an arraignment date | `FtaBatchReport.razor` |
| `[AuthorityCivil].[dbo]` inline SQL | Civil party/plaintiff/defendant lookup (no SP yet) | `CivilFreeformEntry.razor`, `NoticesFreeformCivil.razor` |

---

## Key Notes

- **No database writes.** The Blazor app is read-only from the database's perspective. All forms POST a DTO to the API which returns a DOCX file stream.
- **Template engine.** Templates in `api/Templates/source/*.docx` use Jinja2-style `{{ variable }}` tokens. `DocxTemplateProcessor` handles token replacement at request time — no preprocessing step required.
- **Multi-charge row looping.** Templates requiring one row per charge use `{%tc for charge in charges_list %}` / `{%tc endfor %}` markers. `DocxTemplateProcessor` clones the `<w:tr>` row once per item in `charges_list` (`List<Dictionary<string,string>>`). Affected forms:
  - `LeapAdmissionAlreadyValid` / `LeapValidSentencing` — use `ChargeItemDto` (`Offense`, `Statute`, `Degree`, `Plea`)
  - `LeapSentencing` / `JailCcPlea` — use `SentencingChargeItemDto` (adds `Finding`, `FinesAmount`, `FinesSuspended`, `JailDays`, `JailDaysSuspended`)
- **Civil SP gap.** The two civil forms still rely on legacy inline SQL. A `[reports].[DMCMuniEntryCivilCaseSearch]` stored procedure should be created to match the pattern used for criminal forms.

_Last updated: March 3, 2026_
