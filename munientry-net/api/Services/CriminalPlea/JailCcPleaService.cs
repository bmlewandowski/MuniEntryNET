using Munientry.Shared.Dtos;

namespace Munientry.Api.Services;

public class JailCcPleaService : DocxServiceBase<JailCcPleaDto>, IJailCcPleaService
{
    protected override string TemplatePath { get; } =
        TemplateResolver.Get("Jail_Plea_Final_Judgment_Template.docx");

    protected override Dictionary<string, object> BuildTokens(JailCcPleaDto dto) => new()
    {
        ["case_number"]                                          = dto.CaseNumber ?? "",
        ["defendant.first_name"]                                 = dto.DefendantFirstName ?? "",
        ["defendant.last_name"]                                  = dto.DefendantLastName ?? "",
        ["defense_counsel"]                                      = dto.DefenseCounselName ?? "",
        ["defense_counsel_type"]                                 = dto.DefenseCounselType ?? "",
        ["appearance_reason"]                                    = dto.AppearanceReason ?? "",
        ["plea_trial_date"]                                      = dto.Date?.ToString("MMMM dd, yyyy") ?? "",
        ["amend_offense_details.motion_disposition"]             = "",
        ["court_costs.pay_today_amount"]                         = dto.PayToday?.ToString("F2") ?? "",
        ["court_costs.monthly_pay_amount"]                       = dto.MonthlyPay?.ToString("F2") ?? "",
        ["court_costs.balance_due_date"]                         = dto.DueDate?.ToString("MMMM dd, yyyy") ?? "",
        ["court_costs.ability_to_pay_time"]                      = dto.TimeToPay ?? "",
        ["fine_jail_days"]                                       = "",
        ["community_control.type_of_control"]                    = "",
        ["community_control.term_of_control"]                    = "",
        ["community_control.specialized_docket_type"]            = "",
        ["community_control.house_arrest_time"]                  = "",
        ["community_control.alcohol_monitoring_time"]            = "",
        ["community_control.gps_exclusion_radius"]               = "",
        ["community_control.gps_exclusion_location"]             = "",
        ["community_control.community_control_community_service_hours"] = "",
        ["community_control.no_contact_with_person"]             = "",
        ["community_control.not_within_500_feet_person"]         = "",
        ["community_control.other_community_control_conditions"] = "",
        ["community_control.pay_restitution_amount"]             = "",
        ["community_control.pay_restitution_to"]                 = "",
        ["jail_terms.days_in_jail"]                              = "",
        ["jail_terms.total_jail_days_to_serve"]                  = "",
        ["jail_terms.report_date"]                               = "",
        ["jail_terms.report_time"]                               = "",
        ["jail_terms.report_type"]                               = "",
        ["jail_terms.jail_report_days_notes"]                    = "",
        ["jail_terms.companion_cases_numbers"]                   = dto.CompanionCases ?? "",
        ["jail_terms.companion_cases_sentence_type"]             = dto.CompanionCasesSentence ?? "",
        ["license_suspension.license_type"]                      = "",
        ["license_suspension.suspension_term"]                   = "",
        ["license_suspension.suspended_date"]                    = "",
        ["other_conditions.terms"]                               = "",
        ["community_service.hours_of_service"]                   = "",
        ["community_service.days_to_complete_service"]           = "",
        ["community_service.due_date_for_service"]               = "",
        ["impoundment.vehicle_make_model"]                       = "",
        ["impoundment.vehicle_license_plate"]                    = "",
        ["impoundment.impound_action"]                           = "",
        ["impoundment.impound_time"]                             = "",
        ["judicial_officer.first_name"]                          = dto.JudicialOfficerFirstName ?? "",
        ["judicial_officer.last_name"]                           = dto.JudicialOfficerLastName ?? "",
        ["judicial_officer.officer_type"]                        = dto.JudicialOfficerType ?? "",
        ["charges_list"]                                         = ChargeListBuilder.BuildSentencing(
            dto.ChargeItems, dto.Charges, dto.ChargeStatute, dto.ChargeDegree,
            dto.ChargePlea, dto.ChargeFinding, dto.ChargeFinesAmount, dto.ChargeFinesSuspended,
            dto.ChargeJailDays, dto.ChargeJailDaysSuspended),
    };
}