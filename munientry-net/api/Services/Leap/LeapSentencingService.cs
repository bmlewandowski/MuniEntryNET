using Munientry.Shared.Dtos;

namespace Munientry.Api.Services;

public class LeapSentencingService : DocxServiceBase<LeapSentencingDto>, ILeapSentencingService
{
    protected override string TemplatePath { get; } =
        TemplateResolver.Get("Leap_Sentencing_Template.docx");

    protected override Dictionary<string, object> BuildTokens(LeapSentencingDto dto) => new()
    {
        ["case_number"]                     = dto.CaseNumber ?? "",
        ["defendant.first_name"]            = dto.DefendantFirstName ?? "",
        ["defendant.last_name"]             = dto.DefendantLastName ?? "",
        ["defense_counsel"]                 = dto.DefenseCounselName ?? "",
        ["defense_counsel_type"]            = dto.DefenseCounselType ?? "",
        ["court_costs.pay_today_amount"]    = dto.PayToday ?? "",
        ["court_costs.monthly_pay_amount"]  = dto.MonthlyPay ?? "",
        ["court_costs.ability_to_pay_time"] = dto.AbilityToPay ?? "",
        ["leap_plea_date"]                  = dto.LeapPleaDate?.ToString("MMMM dd, yyyy") ?? "",
        ["plea_trial_date"]                 = dto.PleaTrialDate?.ToString("MMMM dd, yyyy") ?? "",
        ["judicial_officer.first_name"]     = dto.JudicialOfficerFirstName ?? "",
        ["judicial_officer.last_name"]      = dto.JudicialOfficerLastName ?? "",
        ["judicial_officer.officer_type"]   = dto.JudicialOfficerType ?? "",
        ["charges_list"]                    = ChargeListBuilder.BuildSentencing(
            dto.ChargeItems, dto.ChargeOffense, dto.ChargeStatute, dto.ChargeDegree,
            dto.ChargePlea, dto.ChargeFinding, dto.ChargeFinesAmount, dto.ChargeFinesSuspended),
    };
}
