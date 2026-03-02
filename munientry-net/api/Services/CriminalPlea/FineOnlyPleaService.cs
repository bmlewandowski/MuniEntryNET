using Munientry.Shared.Dtos;

namespace Munientry.Api.Services;

public class FineOnlyPleaService : DocxServiceBase<FineOnlyPleaDto>, IFineOnlyPleaService
{
    protected override string TemplatePath { get; } =
        TemplateResolver.Get("Fine_Only_Plea_Final_Judgment_Template.docx");

    protected override Dictionary<string, object> BuildTokens(FineOnlyPleaDto dto) => new()
    {
        ["case_number"]                    = dto.CaseNumber ?? "",
        ["defendant.first_name"]           = dto.DefendantFirstName ?? "",
        ["defendant.last_name"]            = dto.DefendantLastName ?? "",
        ["defense_counsel"]                = dto.DefenseCounselName ?? "",
        ["defense_counsel_type"]           = dto.DefenseCounselType ?? "",
        ["appearance_reason"]              = dto.AppearanceReason ?? "",
        ["charge.offense"]                 = dto.Charges ?? "",
        ["charge.statute"]                 = dto.ChargeStatute ?? "",
        ["charge.degree"]                  = dto.ChargeDegree ?? "",
        ["charge.plea"]                    = dto.ChargePlea ?? "",
        ["charge.finding"]                 = dto.ChargeFinding ?? "",
        ["charge.fines_amount"]            = dto.ChargeFinesAmount ?? "",
        ["charge.fines_suspended"]         = dto.ChargeFinesSuspended ?? "",
        ["court_costs.pay_today_amount"]   = dto.PayToday?.ToString("F2") ?? "",
        ["court_costs.monthly_pay_amount"] = dto.MonthlyPay?.ToString("F2") ?? "",
        ["fra_in_file"]                    = dto.FraInFile ?? "",
        ["fra_in_court"]                   = dto.FraInCourt ?? "",
        ["fine_amount"]                    = dto.FineAmount?.ToString("F2") ?? "",
        ["judicial_officer.first_name"]    = dto.JudicialOfficerFirstName ?? "",
        ["judicial_officer.last_name"]     = dto.JudicialOfficerLastName ?? "",
        ["judicial_officer.officer_type"]  = dto.JudicialOfficerType ?? "",
    };
}
