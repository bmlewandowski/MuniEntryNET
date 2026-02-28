"""
Preprocesses DOCX templates from resources/Templates into api/Templates.
Converts Jinja2-style {{ variable }} tokens (which Word splits across XML runs)
into single-run {Placeholder} tokens for use with DocumentFormat.OpenXml.

Run once from the repo root (or poc folder root):
    python api/Templates/prepare_templates.py
"""

import zipfile
import re
import shutil
import io
import os

# ---------------------------------------------------------------------------
# Shared sub-maps (reused across multiple templates)
# ---------------------------------------------------------------------------
_DEFENDANT = {
    "defendant.first_name": "DefendantFirstName",
    "defendant.last_name": "DefendantLastName",
}

_JUDICIAL_OFFICER = {
    "judicial_officer.first_name": "JudicialOfficerFirstName",
    "judicial_officer.last_name": "JudicialOfficerLastName",
    "judicial_officer.officer_type": "JudicialOfficerType",
}

_DEFENSE_COUNSEL = {
    "defense_counsel": "DefenseCounsel",
    "defense_counsel_type": "DefenseCounselType",
}

# Single-charge placeholders (Jinja for-loop body fields)
_CHARGE_FIELDS = {
    "charge.offense": "ChargeOffense",
    "charge.statute": "ChargeStatute",
    "charge.degree": "ChargeDegree",
    "charge.plea": "ChargePlea",
    "charge.finding": "ChargeFinding",
    "charge.fines_amount": "ChargeFinesAmount",
    "charge.fines_suspended": "ChargeFinesSuspended",
    "charge.jail_days": "ChargeJailDays",
    "charge.jail_days_suspended": "ChargeJailDaysSuspended",
    "amend_offense_details.motion_disposition": "AmendOffenseMotionDisposition",
}

_BOND_CONDITIONS = {
    "bond_conditions.bond_amount": "BondAmount",
    "bond_conditions.monitoring_type": "MonitoringType",
    "bond_conditions.specialized_docket_type": "SpecializedDocketType",
    "custodial_supervision.supervisor": "CustodialSupervisionSupervisor",
    "no_contact.name": "NoContactName",
    "other_conditions.terms": "OtherConditionsTerms",
    "domestic_violence_conditions.exclusive_possession_to": "ExclusivePossessionTo",
    "domestic_violence_conditions.residence_address": "ResidenceAddress",
    "domestic_violence_conditions.surrender_weapons_date": "SurrenderWeaponsDate",
    "vehicle_seizure.vehicle_make_model": "VehicleMakeModel",
    "vehicle_seizure.vehicle_license_plate": "VehicleLicensePlate",
    "vehicle_seizure.state_opposes": "VehicleSeizureStateOpposes",
    "vehicle_seizure.disposition_motion_to_return": "DispositionMotionToReturn",
    "admin_license_suspension.explanation": "AdminLicenseSuspensionExplanation",
}

_COMMUNITY_CONTROL = {
    "community_control.type_of_control": "CommunityControlType",
    "community_control.term_of_control": "CommunityControlTerm",
    "community_control.specialized_docket_type": "CommunityControlSpecializedDocket",
    "community_control.house_arrest_time": "HouseArrestTime",
    "community_control.alcohol_monitoring_time": "AlcoholMonitoringTime",
    "community_control.gps_exclusion_radius": "GpsExclusionRadius",
    "community_control.gps_exclusion_location": "GpsExclusionLocation",
    "community_control.community_control_community_service_hours": "CommunityServiceHours",
    "community_control.no_contact_with_person": "NoContactWithPerson",
    "community_control.not_within_500_feet_person": "NotWithin500FeetPerson",
    "community_control.other_community_control_conditions": "OtherCommunityControlConditions",
    "community_control.pay_restitution_amount": "PayRestitutionAmount",
    "community_control.pay_restitution_to": "PayRestitutionTo",
}

_JAIL_TERMS = {
    "jail_terms.days_in_jail": "JailTermsDaysInJail",
    "jail_terms.total_jail_days_to_serve": "JailTermsTotalDaysToServe",
    "jail_terms.report_date": "JailTermsReportDate",
    "jail_terms.report_time": "JailTermsReportTime",
    "jail_terms.report_type": "JailTermsReportType",
    "jail_terms.jail_report_days_notes": "JailTermsReportDaysNotes",
    "jail_terms.companion_cases_numbers": "JailTermsCompanionCasesNumbers",
    "jail_terms.companion_cases_sentence_type": "JailTermsCompanionCasesSentenceType",
    "fine_jail_days": "FineJailDays",
}

_COURT_COSTS = {
    "court_costs.pay_today_amount": "CourtCostsPayToday",
    "court_costs.monthly_pay_amount": "CourtCostsMonthlyPay",
    "court_costs.balance_due_date": "CourtCostsBalanceDueDate",
    "court_costs.ability_to_pay_time": "CourtCostsAbilityToPayTime",
}

_COMMUNITY_SERVICE = {
    "community_service.hours_of_service": "CommunityServiceHoursOfService",
    "community_service.days_to_complete_service": "CommunityServiceDaysToComplete",
    "community_service.due_date_for_service": "CommunityServiceDueDate",
}

_LICENSE_SUSPENSION = {
    "license_suspension.license_type": "LicenseSuspensionType",
    "license_suspension.suspension_term": "LicenseSuspensionTerm",
    "license_suspension.suspended_date": "LicenseSuspendedDate",
}

# ---------------------------------------------------------------------------
# Token maps: Jinja2 token inner text (normalized, spaces removed) → placeholder
# ---------------------------------------------------------------------------
TIME_TO_PAY_TOKENS = {
    "case_number": "CaseNumber",
    "entry_date": "EntryDate",
    **_DEFENDANT,
    "appearance_date": "AppearanceDate",
    **_JUDICIAL_OFFICER,
}

TRIAL_TO_COURT_TOKENS = {
    "case_number": "CaseNumber",
    **_DEFENDANT,
    "assigned_judge": "AssignedJudge",
    "trial_to_court.date": "TrialToCourtDate",
    "trial_to_court.time": "TrialToCourtTime",
    "trial_to_court.location": "AssignedCourtroom",
    **_JUDICIAL_OFFICER,
    "interpreter_language": "LanguageRequired",
}

DRIVING_PRIVILEGES_TOKENS = {
    "case_number": "CaseNumber",
    **_DEFENDANT,
    "defendant.license_number": "DefendantLicenseNumber",
    "defendant.birth_date": "DefendantBirthDate",
    "defendant.address": "DefendantAddress",
    "defendant.city": "DefendantCity",
    "defendant.state": "DefendantState",
    "defendant.zipcode": "DefendantZipcode",
    "suspension_type": "SuspensionType",
    "suspension_start_date": "SuspensionStartDate",
    "suspension_end_date": "SuspensionEndDate",
    "bmv_cases": "BmvCases",
    "related_traffic_case_number": "RelatedTrafficCaseNumber",
    "employer.privileges_type": "EmployerPrivilegesType",
    "employer.name": "EmployerName",
    "employer.address": "EmployerAddress",
    "employer.city": "EmployerCity",
    "employer.state": "EmployerState",
    "employer.zipcode": "EmployerZipcode",
    "employer.driving_days": "EmployerDrivingDays",
    "employer.driving_hours": "EmployerDrivingHours",
    "employer.other_conditions": "EmployerOtherConditions",
    "additional_information_text": "AdditionalInformationText",
}

NOTICES_FREEFORM_CIVIL_TOKENS = {
    "case_number": "CaseNumber",
    "entry_date": "EntryDate",
    **_DEFENDANT,
    "entry_content_text": "NoticeText",
    "appearance_reason": "AppearanceReason",
    **_DEFENSE_COUNSEL,
    **_JUDICIAL_OFFICER,
    "plea_trial_date": "PleaTrialDate",
}

CIVIL_FREEFORM_ENTRY_TOKENS = {
    "case_number": "CaseNumber",
    "entry_date": "EntryDate",
    "defendant.party_name": "DefendantLastName",
    "plaintiff.party_name": "PlaintiffName",
    "appearance_reason": "AppearanceReason",
    **_DEFENSE_COUNSEL,
    "entry_content_text": "EntryContentText",
    **_JUDICIAL_OFFICER,
}

DIVERSION_TOKENS = {
    "case_number": "CaseNumber",
    **_DEFENDANT,
    **_DEFENSE_COUNSEL,
    "appearance_reason": "AppearanceReason",
    "plea_trial_date": "PleaTrialDate",
    "diversion.program_name": "DiversionProgramName",
    "diversion.diversion_completion_date": "DiversionCompletionDate",
    "diversion.diversion_fine_pay_date": "DiversionFinePayDate",
    "diversion.other_conditions_text": "OtherConditionsText",
    "diversion.pay_restitution_amount": "PayRestitutionAmount",
    "diversion.pay_restitution_to": "PayRestitutionTo",
    **_JUDICIAL_OFFICER,
    **_CHARGE_FIELDS,
}

DENY_PRIVILEGES_TOKENS = {
    "case_number": "CaseNumber",
    **_DEFENDANT,
    **_JUDICIAL_OFFICER,
    "license_expiration_date": "LicenseExpirationDate",
    "nufc_date": "NufcDate",
    "privileges_grant_date": "PrivilegesGrantDate",
}

NOT_GUILTY_BOND_TOKENS = {
    "case_number": "CaseNumber",
    **_DEFENDANT,
    **_DEFENSE_COUNSEL,
    "appearance_reason": "AppearanceReason",
    "plea_trial_date": "PleaTrialDate",
    **_JUDICIAL_OFFICER,
    **_BOND_CONDITIONS,
    **_CHARGE_FIELDS,
}

NO_PLEA_BOND_TOKENS = {
    "case_number": "CaseNumber",
    **_DEFENDANT,
    **_DEFENSE_COUNSEL,
    "appearance_reason": "AppearanceReason",
    "plea_trial_date": "PleaTrialDate",
    **_JUDICIAL_OFFICER,
    **_BOND_CONDITIONS,
}

PROBATION_VIOLATION_BOND_TOKENS = {
    "case_number": "CaseNumber",
    **_DEFENDANT,
    **_DEFENSE_COUNSEL,
    "appearance_reason": "AppearanceReason",
    "plea_trial_date": "PleaTrialDate",
    **_JUDICIAL_OFFICER,
    "cc_bond_conditions.bond_amount": "BondAmount",
    "cc_bond_conditions.monitoring_type": "MonitoringType",
    "cc_bond_conditions.cc_violation_other_conditions_terms": "OtherConditionsTerms",
}

ARRAIGNMENT_CONTINUE_TOKENS = {
    "case_number": "CaseNumber",
    **_DEFENDANT,
    **_DEFENSE_COUNSEL,
    "appearance_reason": "AppearanceReason",
    "plea_trial_date": "PleaTrialDate",
    "continuance_conditions.current_arraignment_date": "CurrentArraignmentDate",
    "continuance_conditions.new_arraignment_date": "NewArraignmentDate",
    "continuance_conditions.continuance_reason": "ContinuanceReason",
    **_JUDICIAL_OFFICER,
}

BOND_HEARING_TOKENS = {
    "case_number": "CaseNumber",
    **_DEFENDANT,
    **_DEFENSE_COUNSEL,
    "appearance_reason": "AppearanceReason",
    "plea_trial_date": "PleaTrialDate",
    **_JUDICIAL_OFFICER,
    **_BOND_CONDITIONS,
}

FAILURE_TO_APPEAR_TOKENS = {
    "case_number": "CaseNumber",
    **_DEFENDANT,
    "appearance_reason": "AppearanceReason",
    "plea_trial_date": "PleaTrialDate",
    **_JUDICIAL_OFFICER,
    "fta_conditions.bond_amount": "BondAmount",
    "fta_conditions.bond_type": "BondType",
    "fta_conditions.arrest_warrant_radius": "ArrestWarrantRadius",
}

FINE_ONLY_PLEA_TOKENS = {
    "case_number": "CaseNumber",
    **_DEFENDANT,
    **_DEFENSE_COUNSEL,
    "appearance_reason": "AppearanceReason",
    "plea_trial_date": "PleaTrialDate",
    **_JUDICIAL_OFFICER,
    **_CHARGE_FIELDS,
    **_COURT_COSTS,
    **_COMMUNITY_SERVICE,
    **_LICENSE_SUSPENSION,
    "other_conditions.terms": "OtherConditionsTerms",
    "fine_jail_days": "FineJailDays",
}

JAIL_PLEA_TOKENS = {
    "case_number": "CaseNumber",
    **_DEFENDANT,
    **_DEFENSE_COUNSEL,
    "appearance_reason": "AppearanceReason",
    "plea_trial_date": "PleaTrialDate",
    **_JUDICIAL_OFFICER,
    **_CHARGE_FIELDS,
    **_COURT_COSTS,
    **_COMMUNITY_SERVICE,
    **_LICENSE_SUSPENSION,
    **_COMMUNITY_CONTROL,
    **_JAIL_TERMS,
    "other_conditions.terms": "OtherConditionsTerms",
    "impoundment.vehicle_make_model": "ImpoundmentVehicleMakeModel",
    "impoundment.vehicle_license_plate": "ImpoundmentVehicleLicensePlate",
    "impoundment.impound_action": "ImpoundmentAction",
    "impoundment.impound_time": "ImpoundmentTime",
}

SENTENCING_ONLY_TOKENS = {
    "case_number": "CaseNumber",
    **_DEFENDANT,
    **_DEFENSE_COUNSEL,
    "appearance_reason": "AppearanceReason",
    "plea_trial_date": "PleaTrialDate",
    "plea_date": "PleaDate",
    **_JUDICIAL_OFFICER,
    **_CHARGE_FIELDS,
    **_COURT_COSTS,
    **_COMMUNITY_SERVICE,
    **_LICENSE_SUSPENSION,
    **_COMMUNITY_CONTROL,
    **_JAIL_TERMS,
    "other_conditions.terms": "OtherConditionsTerms",
    "impoundment.vehicle_make_model": "ImpoundmentVehicleMakeModel",
    "impoundment.vehicle_license_plate": "ImpoundmentVehicleLicensePlate",
    "impoundment.impound_action": "ImpoundmentAction",
    "impoundment.impound_time": "ImpoundmentTime",
}

TRIAL_SENTENCING_TOKENS = {
    "case_number": "CaseNumber",
    **_DEFENDANT,
    **_DEFENSE_COUNSEL,
    "plea_trial_date": "PleaTrialDate",
    **_JUDICIAL_OFFICER,
    **_CHARGE_FIELDS,
    **_COURT_COSTS,
    **_COMMUNITY_SERVICE,
    **_LICENSE_SUSPENSION,
    **_COMMUNITY_CONTROL,
    **_JAIL_TERMS,
    "other_conditions.terms": "OtherConditionsTerms",
    "impoundment.vehicle_make_model": "ImpoundmentVehicleMakeModel",
    "impoundment.vehicle_license_plate": "ImpoundmentVehicleLicensePlate",
    "impoundment.impound_action": "ImpoundmentAction",
    "impoundment.impound_time": "ImpoundmentTime",
}

LEAP_ADMISSION_PLEA_TOKENS = {
    "case_number": "CaseNumber",
    **_DEFENDANT,
    **_DEFENSE_COUNSEL,
    "appearance_reason": "AppearanceReason",
    "plea_trial_date": "PleaTrialDate",
    "leap_sentencing_date": "LeapSentencingDate",
    **_JUDICIAL_OFFICER,
    **_CHARGE_FIELDS,
}

LEAP_ADMISSION_PLEA_VALID_TOKENS = {
    "case_number": "CaseNumber",
    **_DEFENDANT,
    **_DEFENSE_COUNSEL,
    "plea_trial_date": "PleaTrialDate",
    **_JUDICIAL_OFFICER,
    **_CHARGE_FIELDS,
}

LEAP_SENTENCING_TOKENS = {
    "case_number": "CaseNumber",
    **_DEFENDANT,
    **_DEFENSE_COUNSEL,
    "plea_trial_date": "PleaTrialDate",
    "leap_plea_date": "LeapPleaDate",
    **_JUDICIAL_OFFICER,
    **_CHARGE_FIELDS,
    **_COURT_COSTS,
    **_COMMUNITY_SERVICE,
    **_LICENSE_SUSPENSION,
    "other_conditions.terms": "OtherConditionsTerms",
    "fine_jail_days": "FineJailDays",
}

PLEA_ONLY_TOKENS = {
    "case_number": "CaseNumber",
    **_DEFENDANT,
    **_DEFENSE_COUNSEL,
    "appearance_reason": "AppearanceReason",
    "plea_trial_date": "PleaTrialDate",
    **_JUDICIAL_OFFICER,
    **_CHARGE_FIELDS,
    "future_sentencing.plea_only_bond_amount": "PleaOnlyBondAmount",
}

JURY_PAYMENT_TOKENS = {
    "case_number": "CaseNumber",
    **_DEFENDANT,
    **_JUDICIAL_OFFICER,
    "plea_trial_date": "PleaTrialDate",
    "trial_date": "TrialDate",
    "jurors_reported": "JurorsReported",
    "jurors_reported_word": "JurorsReportedWord",
    "jurors_seated": "JurorsSeated",
    "jurors_seated_word": "JurorsSeatedWord",
    "jurors_not_seated": "JurorsNotSeated",
    "jurors_not_seated_word": "JurorsNotSeatedWord",
    "jurors_pay_not_seated": "JurorsPayNotSeated",
    "jurors_pay_not_seated_word": "JurorsPayNotSeatedWord",
    "jurors_pay_seated": "JurorsPaySeated",
    "jurors_pay_seated_word": "JurorsPaySeatedWord",
    "jury_panel_total_pay": "JuryPanelTotalPay",
    "jury_panel_total_pay_word": "JuryPanelTotalPayWord",
}

FINAL_JURY_NOTICE_TOKENS = {
    "case_number": "CaseNumber",
    **_DEFENDANT,
    "assigned_judge": "AssignedJudge",
    "final_pretrial.date": "FinalPretrialDate",
    "final_pretrial.time": "FinalPretrialTime",
    "jury_trial.date": "JuryTrialDate",
    "jury_trial.time": "JuryTrialTime",
    "jury_trial.location": "JuryTrialLocation",
    "interpreter_language": "LanguageRequired",
    **_JUDICIAL_OFFICER,
}

GENERAL_NOTICE_OF_HEARING_TOKENS = {
    "case_number": "CaseNumber",
    **_DEFENDANT,
    "assigned_judge": "AssignedJudge",
    "hearing.date": "HearingDate",
    "hearing.time": "HearingTime",
    "hearing.type": "HearingType",
    "hearing.location": "HearingLocation",
    "interpreter_language": "LanguageRequired",
    **_JUDICIAL_OFFICER,
}

SCHEDULING_ENTRY_TOKENS = {
    "case_number": "CaseNumber",
    **_DEFENDANT,
    "pretrial.date": "PretrialDate",
    "final_pretrial.date": "FinalPretrialDate",
    "final_pretrial.time": "FinalPretrialTime",
    "jury_trial.date": "JuryTrialDate",
    "jury_trial.time": "JuryTrialTime",
    "interpreter_language": "LanguageRequired",
}

FREEFORM_ENTRY_TOKENS = {
    "case_number": "CaseNumber",
    **_DEFENDANT,
    **_DEFENSE_COUNSEL,
    "appearance_reason": "AppearanceReason",
    "plea_trial_date": "PleaTrialDate",
    "entry_content_text": "EntryContentText",
    **_JUDICIAL_OFFICER,
}

ADMIN_FISCAL_TOKENS = {
    "account_name": "AccountName",
    "account_number": "AccountNumber",
    "subaccount_name": "SubaccountName",
    "subaccount_number": "SubaccountNumber",
    "disbursement_reason": "DisbursementReason",
    "disbursement_amount": "DisbursementAmount",
    "disbursement_vendor": "DisbursementVendor",
    "invoice_number": "InvoiceNumber",
    **_JUDICIAL_OFFICER,
}

COMPETENCY_EVALUATION_TOKENS = {
    "case_number": "CaseNumber",
    **_DEFENDANT,
    "plea_trial_date": "PleaTrialDate",
    "treatment_type": "TreatmentType",
    "final_pretrial.date": "FinalPretrialDate",
    "final_pretrial.time": "FinalPretrialTime",
    "jury_trial.date": "JuryTrialDate",
    **_JUDICIAL_OFFICER,
}

CRIMINAL_SEALING_TOKENS = {
    "case_number": "CaseNumber",
    **_DEFENDANT,
    **_DEFENSE_COUNSEL,
    "plea_trial_date": "PleaTrialDate",
    "seal_decision": "SealDecision",
    "sealing_type": "SealingType",
    "offense_date": "OffenseDate",
    "offense_seal_list": "OffenseSealList",
    "denial_reasons": "DenialReasons",
    "state_response": "StateResponse",
    "bci_number": "BciNumber",
    "fbi_number": "FbiNumber",
    **_JUDICIAL_OFFICER,
}

TERMS_OF_COMMUNITY_CONTROL_TOKENS = {
    "case_number": "CaseNumber",
    **_DEFENDANT,
    "entry_date": "EntryDate",
    "term_of_control": "TermOfControl",
    "specialized_docket_type": "SpecializedDocketType",
    "community_service_hours": "CommunityServiceHours",
    "jail_days": "JailDays",
    "jail_report_date": "JailReportDate",
    "jail_report_time": "JailReportTime",
    "no_contact_with_person": "NoContactWithPerson",
    "report_frequency": "ReportFrequency",
    "scram_days": "ScramDays",
    **_JUDICIAL_OFFICER,
}

NOTICE_CC_VIOLATION_TOKENS = {
    "case_number": "CaseNumber",
    **_DEFENDANT,
    "entry_date": "EntryDate",
    "slated_date": "SlatedDate",
    "violation_hearing_date": "ViolationHearingDate",
    "violation_hearing_time": "ViolationHearingTime",
    **_JUDICIAL_OFFICER,
}

# Each entry: (output_filename, token_map) or (output_filename, token_map, source_filename)
TEMPLATES = [
    ("Time_To_Pay_Template.docx", TIME_TO_PAY_TOKENS),
    ("Trial_To_Court_Hearing_Notice_Template.docx", TRIAL_TO_COURT_TOKENS),
    ("Driving_Privileges_Template.docx", DRIVING_PRIVILEGES_TOKENS),
    ("Notices_Freeform_Civil_Template.docx", NOTICES_FREEFORM_CIVIL_TOKENS, "Freeform_Entry_Template.docx"),
    ("Civil_Freeform_Entry_Template.docx", CIVIL_FREEFORM_ENTRY_TOKENS),
    ("Diversion_Template.docx", DIVERSION_TOKENS),
    ("Deny_Privileges_Template.docx", DENY_PRIVILEGES_TOKENS),
    ("Not_Guilty_Bond_Template.docx", NOT_GUILTY_BOND_TOKENS),
    ("No_Plea_Bond_Template.docx", NO_PLEA_BOND_TOKENS),
    ("Probation_Violation_Bond_Template.docx", PROBATION_VIOLATION_BOND_TOKENS),
    ("Arraignment_Continue_Template.docx", ARRAIGNMENT_CONTINUE_TOKENS),
    ("Bond_Hearing_Template.docx", BOND_HEARING_TOKENS),
    ("Failure_To_Appear_Template.docx", FAILURE_TO_APPEAR_TOKENS),
    ("Fine_Only_Plea_Final_Judgment_Template.docx", FINE_ONLY_PLEA_TOKENS),
    ("Jail_Plea_Final_Judgment_Template.docx", JAIL_PLEA_TOKENS),
    ("Sentencing_Only_Template.docx", SENTENCING_ONLY_TOKENS),
    ("Trial_Sentencing_Template.docx", TRIAL_SENTENCING_TOKENS),
    ("Leap_Admission_Plea_Template.docx", LEAP_ADMISSION_PLEA_TOKENS),
    ("Leap_Admission_Plea_Valid_Template.docx", LEAP_ADMISSION_PLEA_VALID_TOKENS),
    ("Leap_Sentencing_Template.docx", LEAP_SENTENCING_TOKENS),
    ("Plea_Only_Template.docx", PLEA_ONLY_TOKENS),
    ("Jury_Payment_Template.docx", JURY_PAYMENT_TOKENS),
    ("Final_Jury_Notice_Of_Hearing_Template.docx", FINAL_JURY_NOTICE_TOKENS),
    ("General_Notice_Of_Hearing_Template.docx", GENERAL_NOTICE_OF_HEARING_TOKENS),
    ("Scheduling_Entry_Template_Rohrer.docx", SCHEDULING_ENTRY_TOKENS),
    ("Scheduling_Entry_Template_Fowler.docx", SCHEDULING_ENTRY_TOKENS),
    ("Scheduling_Entry_Template_Hemmeter.docx", SCHEDULING_ENTRY_TOKENS),
    ("Freeform_Entry_Template.docx", FREEFORM_ENTRY_TOKENS),
    ("Admin_Fiscal_Template.docx", ADMIN_FISCAL_TOKENS),
    ("Competency_Evaluation_Template.docx", COMPETENCY_EVALUATION_TOKENS),
    ("Criminal_Sealing_Entry_Template.docx", CRIMINAL_SEALING_TOKENS),
    ("Terms_Of_Community_Control_Template.docx", TERMS_OF_COMMUNITY_CONTROL_TOKENS),
    ("Notice_CC_Violation_Template.docx", NOTICE_CC_VIOLATION_TOKENS),
]

RESOURCES_DIR = os.path.abspath(
    os.path.join(os.path.dirname(__file__), "source")
)
OUTPUT_DIR = os.path.abspath(os.path.dirname(__file__))


def normalize_token_key(inner: str) -> str:
    """Remove all whitespace from inside a {{ }} token to get the canonical key."""
    return re.sub(r"\s+", "", inner)


def strip_jinja_blocks(text: str) -> str:
    """
    Strip Jinja2 control-flow blocks from plain text (not XML).
    For {% if %}...{% elif/else %}...{% endif %} keeps only the FIRST branch.
    For {% for %}...{% endfor %} keeps the loop body once.
    Remaining bare {% %} tags are removed.
    """
    # Keep first branch of if/elif/else/endif blocks
    def keep_first_branch(m):
        full = m.group(0)
        # Remove the opening {% if ... %} tag
        inner = re.sub(r'^\{%-?\s*if\b.*?%\}', '', full, count=1, flags=re.DOTALL)
        # Find where the first elif/else/endif starts
        end = re.search(r'\{%-?\s*(?:elif|else|endif)\b', inner)
        return inner[:end.start()].strip() if end else inner.strip()
    text = re.sub(
        r'\{%-?\s*if\b.*?%\}.*?\{%-?\s*endif\s*-?%\}',
        keep_first_branch, text, flags=re.DOTALL
    )
    # Keep body of for loops (render once)
    def keep_loop_body(m):
        full = m.group(0)
        inner = re.sub(r'^\{%-?\s*for\b.*?%\}', '', full, count=1, flags=re.DOTALL)
        inner = re.sub(r'\{%-?\s*endfor\s*-?%\}$', '', inner)
        return inner.strip()
    text = re.sub(
        r'\{%-?\s*for\b.*?%\}.*?\{%-?\s*endfor\s*-?%\}',
        keep_loop_body, text, flags=re.DOTALL
    )
    # Strip any remaining bare control tags
    text = re.sub(r'\{%.*?%\}', '', text, flags=re.DOTALL)
    return text


def replace_jinja_tokens_in_xml(xml: str, token_map: dict) -> str:
    """
    Replaces Jinja2 {{ variable }} tokens in document XML.

    Word splits tokens across multiple <w:t> runs. We handle this by:
    1. Collecting the full text of each <w:p> paragraph by concatenating all <w:t> values.
    2. If the combined text contains {{ ... }}, rebuild the paragraph using a
       'run merger': scan through <w:r> runs and buffer any that span a token
       boundary, then emit a single merged run with the replacement text.
    """
    W = "http://schemas.openxmlformats.org/wordprocessingml/2006/main"

    def get_run_text(run_xml: str) -> str:
        parts = re.findall(r"<w:t\b[^>]*>(.*?)</w:t>", run_xml, re.DOTALL)
        return "".join(parts)

    def get_first_rpr(run_xml: str) -> str:
        m = re.search(r"<w:rPr>.*?</w:rPr>", run_xml, re.DOTALL)
        return m.group(0) if m else ""

    def make_run(rpr: str, text: str) -> str:
        if not text:
            return ""
        safe = text.replace("&", "&amp;").replace("<", "&lt;").replace(">", "&gt;")
        space = ' xml:space="preserve"' if text != text.strip() else ""
        return f"<w:r>{rpr}<w:t{space}>{safe}</w:t></w:r>"

    def process_paragraph(para_xml: str) -> str:
        # Check if this paragraph contains any Jinja2 tokens at all
        all_text_parts = re.findall(r"<w:t\b[^>]*>(.*?)</w:t>", para_xml, re.DOTALL)
        combined = "".join(all_text_parts)

        if "{{" not in combined and "{%" not in combined:
            return para_xml  # nothing to do

        # Normalise: collapse spaces inside {{ ... }} for matching
        def norm_combined(text):
            return re.sub(
                r"\{\{(.*?)\}\}",
                lambda m: "{{" + normalize_token_key(m.group(1)) + "}}",
                text,
                flags=re.DOTALL,
            )

        normalised = norm_combined(combined)

        # Apply token replacements on the normalised combined text
        result = normalised
        for inner_key, placeholder in token_map.items():
            result = result.replace("{{" + inner_key + "}}", "{" + placeholder + "}")

        # Strip Jinja2 control blocks from the combined text before deciding
        result_stripped = strip_jinja_blocks(result)

        if result_stripped == normalised and result == normalised:
            return para_xml  # tokens present but none in our map — leave alone

        # --- Rebuild the paragraph ---
        # Keep <w:pPr> unchanged
        ppr_match = re.search(r"<w:pPr>.*?</w:pPr>", para_xml, re.DOTALL)
        ppr = ppr_match.group(0) if ppr_match else ""

        # Extract all <w:r> runs (simple, non-recursive)
        runs = re.findall(r"<w:r[ >].*?</w:r>", para_xml, re.DOTALL)

        # Merge all run text, tracking positions
        run_texts = [get_run_text(r) for r in runs]
        rpr_from_first = get_first_rpr(runs[0]) if runs else ""

        # The combined text after merging all runs
        full_text = "".join(run_texts)

        # Normalise combined text the same way we did above
        norm_full = norm_combined(full_text)
        # Apply replacements
        replaced_text = norm_full
        for inner_key, placeholder in token_map.items():
            replaced_text = replaced_text.replace(
                "{{" + inner_key + "}}", "{" + placeholder + "}"
            )
        # Strip Jinja2 control blocks intelligently (keep first if/elif branch)
        replaced_text = strip_jinja_blocks(replaced_text)

        if replaced_text.strip() == full_text.strip() and not re.search(r'\{%', full_text):
            return para_xml

        # --- Emit new runs: keep non-token literal segments, single run per token ---
        # Split on {Placeholder} boundaries so we can reuse formatting from the
        # original first run for the replacement runs.
        parts = re.split(r"(\{[A-Za-z]+\})", replaced_text)
        new_runs = []
        for part in parts:
            if not part:
                continue
            if re.fullmatch(r"\{[A-Za-z]+\}", part):
                # Token replacement — use first run's rPr
                new_runs.append(make_run(rpr_from_first, part))
            else:
                # Literal text — use first run's rPr for now (preserves basic style)
                new_runs.append(make_run(rpr_from_first, part))

        # Reconstruct the paragraph
        p_tag = re.match(r"<w:p( [^>]*)?>", para_xml)
        open_tag = p_tag.group(0) if p_tag else "<w:p>"
        return open_tag + ppr + "".join(new_runs) + "</w:p>"

    # Split XML into paragraphs, process each, reassemble
    def replace_para(m):
        return process_paragraph(m.group(0))

    return re.sub(r"<w:p[ >].*?</w:p>", replace_para, xml, flags=re.DOTALL)


def process_template(src_path: str, dst_path: str, token_map: dict) -> None:
    print(f"Processing: {os.path.basename(src_path)}")
    with open(src_path, "rb") as f:
        src_bytes = f.read()

    in_buf = io.BytesIO(src_bytes)
    out_buf = io.BytesIO()

    with zipfile.ZipFile(in_buf, "r") as zin:
        with zipfile.ZipFile(out_buf, "w", compression=zipfile.ZIP_DEFLATED) as zout:
            for item in zin.infolist():
                data = zin.read(item.filename)
                # Process all word XML parts (document body, headers, footers, footnotes)
                if (item.filename.startswith("word/") and item.filename.endswith(".xml")
                        and not any(item.filename.endswith(s) for s in
                                    ("/styles.xml", "/settings.xml", "/fontTable.xml",
                                     "/webSettings.xml", "/numbering.xml",
                                     "/theme/theme1.xml"))):
                    xml = data.decode("utf-8")
                    xml = replace_jinja_tokens_in_xml(xml, token_map)
                    # Strip any remaining bare Jinja2 control tags not caught at paragraph level
                    xml = re.sub(r'\{%.*?%\}', '', xml, flags=re.DOTALL)
                    # Strip any unreplaced {{ variable }} tokens (list-index access, unknown vars, etc.)
                    xml = re.sub(r'\{\{[^{}<>]*?\}\}', '', xml, flags=re.DOTALL)
                    data = xml.encode("utf-8")
                zout.writestr(item, data)

    with open(dst_path, "wb") as f:
        f.write(out_buf.getvalue())
    print(f"  -> Written to: {dst_path}")


def main():
    for entry in TEMPLATES:
        filename, token_map = entry[0], entry[1]
        src_filename = entry[2] if len(entry) > 2 else filename
        src = os.path.join(RESOURCES_DIR, src_filename)
        dst = os.path.join(OUTPUT_DIR, filename)
        if not os.path.exists(src):
            print(f"  SKIPPING (not found): {src}")
            continue
        process_template(src, dst, token_map)
    print("Done.")


if __name__ == "__main__":
    main()
