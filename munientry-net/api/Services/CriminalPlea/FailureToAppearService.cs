using Munientry.Shared.Dtos;

namespace Munientry.Api.Services;

public class FailureToAppearService : DocxServiceBase<FailureToAppearDto>, IFailureToAppearService
{
    protected override string TemplatePath { get; } =
        TemplateResolver.Get("Failure_To_Appear_Template.docx");

    protected override Dictionary<string, object> BuildTokens(FailureToAppearDto dto) => new()
    {
        ["case_number"]                          = dto.CaseNumber ?? "",
        ["defendant.first_name"]                 = dto.DefendantFirstName ?? "",
        ["defendant.last_name"]                  = dto.DefendantLastName ?? "",
        ["appearance_reason"]                    = dto.AppearanceReason ?? "",
        ["plea_trial_date"]                      = dto.EntryDate?.ToString("MMMM dd, yyyy") ?? "",
        ["fta_conditions.arrest_warrant_radius"] = dto.ArrestWarrantRadius ?? "",
        ["fta_conditions.bond_type"]             = dto.BondType ?? "",
        ["fta_conditions.bond_amount"]           = dto.BondAmount ?? "",
        ["judicial_officer.first_name"]          = dto.JudicialOfficerFirstName ?? "",
        ["judicial_officer.last_name"]           = dto.JudicialOfficerLastName ?? "",
        ["judicial_officer.officer_type"]        = dto.JudicialOfficerType ?? "",
    };
}
