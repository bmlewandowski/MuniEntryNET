# Forms Mapping Reference

This document provides a reference mapping between the legacy Python dialogs and the new .NET Blazor forms implemented in the MuniEntry POC project.

---

## Scheduling
| Legacy Python Dialog                      | New .NET Blazor Form                |
|-------------------------------------------|-------------------------------------|
| final_jury_hearing_notice_dialog.py       | FinalJuryNoticeOfHearing            |
| general_hearing_notice_dialog.py          | GeneralNoticeOfHearing              |
| sched_entry_dialogs.py                    | SchedulingEntryHearings             |
| trial_to_court_hearing_notice_dialog.py   | TrialToCourtNoticeOfHearing         |

## Driving
| Legacy Python Dialog         | New .NET Blazor Form   |
|-----------------------------|------------------------|
| driving_privileges_dialog.py | DrivingPrivileges      |

## Criminal
| Legacy Python Dialog           | New .NET Blazor Form         |
|-------------------------------|------------------------------|
| no_plea_bond_dialog.py        | AppearOnWarrantNoPlea        |
| arraignment_continue_dialog.py| ArraignmentFtaBond           |
| bond_hearing_dialog.py        | BondModificationRevocation   |
| community_service_secondary.py| CommunityServiceSecondary    |
| diversion_dialog.py           | DiversionPlea                |
| fine_only_plea_dialog.py      | FineOnlyPleaOnly             |
| jail_cc_plea_dialog.py        | JailCcPlea                   |
| leap_plea_dialog.py           | LeapAdmissionPlea            |
| leap_plea_valid_dialog.py     | LeapValidSentencing          |
| not_guilty_bond_dialog.py     | NotGuiltyAppearBondSpecial   |
| not_guilty_plea_dialog.py     | NotGuiltyPlea                |
| deny_privileges_dialog.py     | SealingDenyPrivileges        |
| trial_sentencing_dialog.py    | TrialSentencing              |

## Probation
| Legacy Python Dialog             | New .NET Blazor Form           |
|----------------------------------|--------------------------------|
| terms_comm_control_dialog.py      | CommunityControlTermsNotices   |
| probation_violation_bond_dialog.py| ProbationViolationBond         |
| notice_cc_violation_dialog.py     | CommunityControlTermsNotices   |

## Admin
| Legacy Python Dialog      | New .NET Blazor Form   |
|--------------------------|------------------------|
| admin_fiscal_dialog.py   | AdminWorkflowsFiscal   |
| jury_payment_dialog.py   | JurorPayment           |
| time_to_pay_dialog.py    | TimeToPayOrder         |

## Notices
| Legacy Python Dialog   | New .NET Blazor Form   |
|-----------------------|------------------------|
| civ_freeform_dialog.py| NoticesFreeformCivil   |

## Misc/Other
| Legacy Python Dialog         | New .NET Blazor Form                |
|-----------------------------|-------------------------------------|
| competency_dialog.py         | CompetencyCivilSealingJuror         |
| criminal_sealing_dialog.py   | CompetencyCivilSealingJuror         |
| add_charge_dialog.py         | AddAmendDialogs                     |
| amend_charge_dialog.py       | AddAmendDialogs                     |
| admin_judge_dw_dialog.py     | PretrialHemmeterMattoxMagistrate    |
| bunner_dw_dialog.py          | PretrialHemmeterMattoxMagistrate    |
| rohrer_dw_dialog.py          | PretrialHemmeterMattoxMagistrate    |
| probation_dw_dialogs.py      | PretrialHemmeterMattoxMagistrate    |

---

## Notes
- All new forms are implemented as .razor pages in `client/Pages/<Category>/`.
- Legacy dialogs are located in the Python source under `munientry/builders/` and `munientry/dialogs/workflows/`.
- The mapping is 1:1 or composite, matching field order and logic for migration.
- This document should be updated as new forms are added or mappings change.
