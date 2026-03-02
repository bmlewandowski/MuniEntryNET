using Munientry.Shared.Dtos;

namespace Munientry.Api.Services;

public class ArraignmentContinuanceService : DocxServiceBase<ArraignmentContinuanceDto>, IArraignmentContinuanceService
{
    protected override string TemplatePath { get; } =
        TemplateResolver.Get("Arraignment_Continue_Template.docx");

    protected override Dictionary<string, object> BuildTokens(ArraignmentContinuanceDto dto) => new()
    {
        ["case_number"]                                         = dto.CaseNumber ?? "",
        ["defendant.first_name"]                                = dto.DefendantFirstName ?? "",
        ["defendant.last_name"]                                 = dto.DefendantLastName ?? "",
        ["defense_counsel"]                                     = dto.DefenseCounselName ?? "",
        ["defense_counsel_type"]                                = dto.DefenseCounselType ?? "",
        ["appearance_reason"]                                   = dto.AppearanceReason ?? "",
        ["continuance_conditions.current_arraignment_date"]     = dto.CurrentArraignmentDate?.ToString("MMMM dd, yyyy") ?? "",
        ["continuance_conditions.new_arraignment_date"]         = dto.NewArraignmentDate?.ToString("MMMM dd, yyyy") ?? "",
        ["continuance_conditions.continuance_reason"]           = dto.ContinuanceReason ?? "",
        ["judicial_officer.first_name"]                         = dto.JudicialOfficerFirstName ?? "",
        ["judicial_officer.last_name"]                          = dto.JudicialOfficerLastName ?? "",
        ["judicial_officer.officer_type"]                       = dto.JudicialOfficerType ?? "",
    };
}
