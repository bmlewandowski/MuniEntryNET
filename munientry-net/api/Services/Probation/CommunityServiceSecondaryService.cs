using Munientry.Shared.Dtos;

namespace Munientry.Api.Services;

public class CommunityServiceSecondaryService : DocxServiceBase<CommunityServiceSecondaryDto>, ICommunityServiceSecondaryService
{
    protected override string TemplatePath { get; } =
        TemplateResolver.Get("Freeform_Entry_Template.docx");

    protected override Dictionary<string, object> BuildTokens(CommunityServiceSecondaryDto dto) => new()
    {
        ["case_number"]                   = "",
        ["defendant.first_name"]          = "",
        ["defendant.last_name"]           = "",
        ["entry_date"]                    = DateTime.UtcNow.ToString("MMMM dd, yyyy"),
        ["defense_counsel"]               = "",
        ["defense_counsel_type"]          = "",
        ["judicial_officer.first_name"]   = dto.JudicialOfficerFirstName ?? "",
        ["judicial_officer.last_name"]    = dto.JudicialOfficerLastName ?? "",
        ["judicial_officer.officer_type"] = dto.JudicialOfficerType ?? "",
    };
}
