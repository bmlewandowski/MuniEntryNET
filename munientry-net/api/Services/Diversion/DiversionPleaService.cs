using Munientry.Shared.Dtos;

namespace Munientry.Api.Services;

public class DiversionPleaService : DocxServiceBase<DiversionPleaDto>, IDiversionPleaService
{
    protected override string TemplatePath { get; } =
        TemplateResolver.Get("Diversion_Template.docx");

    protected override Dictionary<string, object> BuildTokens(DiversionPleaDto dto) => new()
    {
        ["case_number"]                               = dto.CaseNumber ?? "",
        ["defendant.first_name"]                      = dto.DefendantFirstName ?? "",
        ["defendant.last_name"]                       = dto.DefendantLastName ?? "",
        ["defense_counsel"]                           = dto.DefenseCounselName ?? "",
        ["defense_counsel_type"]                      = dto.DefenseCounselType ?? "",
        ["appearance_reason"]                         = dto.AppearanceReason ?? "",
        ["charge.offense"]                            = dto.Charges ?? "",
        ["diversion.program_name"]                    = dto.DiversionType ?? "",
        ["diversion.diversion_completion_date"]       = dto.DiversionCompletionDate?.ToString("MMMM dd, yyyy") ?? "",
        ["diversion.diversion_fine_pay_date"]         = dto.DiversionFinePayDate?.ToString("MMMM dd, yyyy") ?? "",
        ["diversion.pay_restitution_to"]              = dto.PayRestitutionTo ?? "",
        ["diversion.pay_restitution_amount"]          = dto.PayRestitutionAmount?.ToString("F2") ?? "",
        ["diversion.other_conditions_text"]           = dto.OtherConditionsText ?? "",
        ["judicial_officer.first_name"]               = dto.JudicialOfficerFirstName ?? "",
        ["judicial_officer.last_name"]                = dto.JudicialOfficerLastName ?? "",
        ["judicial_officer.officer_type"]             = dto.JudicialOfficerType ?? "",
    };
}
