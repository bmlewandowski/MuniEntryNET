# MuniEntry POC Setup & Forms Guide

## Project Setup

To set up and run the MuniEntry Proof-of-Concept project, use the following commands from the `munientry-poc` directory:

```sh
docker compose build api   # Build the API service
docker compose build client   # Build the Blazor client app
docker compose up --no-build # Start all services (API and client)
```

- The client will be available at [http://localhost:3000](http://localhost:3000) (or as configured in your docker/nginx setup).
- The API will be available at [http://localhost:5000](http://localhost:5000) (or as configured).

## Included Forms (Side Navigation)

Below are the main forms available from the side navigation, along with the legacy Python forms they replace:

| Blazor Form Page | Legacy Python Dialog/UI |
|------------------|------------------------|
| Criminal > Fine Only Plea | fine_only_plea_dialog_ui.py |
| Criminal > Diversion Plea | diversion_plea_dialog_ui.py |
| Criminal > Not Guilty Plea | not_guilty_plea_dialog_ui.py |
| Criminal > Leap Admission Already Valid | leap_admission_already_valid_dialog_ui.py |
| Criminal > Leap Sentencing | leap_sentencing_dialog_ui.py |
| Criminal > Leap Valid Sentencing | leap_valid_sentencing_dialog_ui.py |
| Criminal > Sentencing Only Already Plead | sentencing_only_already_plead_dialog_ui.py |
| Criminal > Trial Sentencing | trial_sentencing_dialog_ui.py |
| Criminal > Plea Only Future Sentencing | plea_only_future_sentencing_dialog_ui.py |
| Criminal > Deny Privileges Permit Retest | deny_privileges_permit_retest_dialog_ui.py |
| Criminal > Community Service Secondary | community_service_secondary_dialog_ui.py |
| Criminal > Arraignment FTA Bond | arraignment_fta_bond_dialog_ui.py |
| Criminal > Bond Modification Revocation | bond_modification_revocation_dialog_ui.py |
| Criminal > Arraignment Continuance | arraignment_continuance_dialog_ui.py |
| Criminal > Appear On Warrant No Plea | appear_on_warrant_no_plea_dialog_ui.py |
| Notices > Civil Freeform Entry | civil_freeform_entry_dialog_ui.py |
| Notices > Notices Freeform Civil | notices_freeform_civil_dialog_ui.py |
| Scheduling > Trial To Court Notice Of Hearing | trial_to_court_notice_of_hearing_dialog_ui.py |
| Scheduling > General Notice Of Hearing | general_notice_of_hearing_dialog_ui.py |
| Scheduling > Final Jury Notice Of Hearing | final_jury_notice_of_hearing_dialog_ui.py |
| Scheduling > Bond Hearing | bond_hearing_dialog_ui.py |
| Probation > Probation Violation Bond | probation_violation_bond_dialog_ui.py |
| Probation > Community Control Terms Notices | community_control_terms_notices_dialog_ui.py |
| Probation > Community Control Terms | community_control_terms_dialog_ui.py |
| Driving > Driving Privileges | driving_privileges_dialog_ui.py |
| Add Secondary > Add/Amend Dialogs | add_amend_dialogs_ui.py |
| Admin > Workflows Fiscal | workflows_fiscal_dialog_ui.py |
| Admin > Time To Pay Order | time_to_pay_order_dialog_ui.py |
| Admin > Juror Payment | juror_payment_dialog_ui.py |
| Workflows > Pretrial Hemmeter Mattox Magistrate | pretrial_hemmeter_mattox_magistrate_dialog_ui.py |

> **Note:** This list is based on the current navigation and may change as new forms are added or legacy dialogs are replaced.

## Additional Notes
- Make sure Docker Desktop is running before executing the above commands.
- For development, you may want to stop and rebuild containers after making changes to the client or API code.
- For more details, see the documentation in the `docs/` and `docsource/` folders.
