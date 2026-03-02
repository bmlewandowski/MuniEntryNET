using Munientry.Shared.Dtos;

namespace Munientry.Api.Services;

public class CivilFreeformEntryService : DocxServiceBase<CivilFreeformEntryDto>, ICivilFreeformEntryService
{
    protected override string TemplatePath { get; } =
        TemplateResolver.Get("Civil_Freeform_Entry_Template.docx");

    protected override Dictionary<string, object> BuildTokens(CivilFreeformEntryDto dto) => new()
    {
        ["case_number"]                   = dto.CaseNumber ?? "",
        ["entry_date"]                    = dto.EntryDate.ToString("MMMM dd, yyyy"),
        ["defendant.last_name"]           = dto.Defendant ?? "",
        ["plaintiff.party_name"]          = dto.Plaintiff ?? "",
        ["appearance_reason"]             = dto.AppearanceReason ?? "",
        ["defense_counsel"]               = "",
        ["defense_counsel_type"]          = "",
        ["entry_content_text"]            = dto.EntryContent ?? "",
        ["judicial_officer.first_name"]   = dto.JudicialOfficerFirstName ?? "",
        ["judicial_officer.last_name"]    = dto.JudicialOfficerLastName ?? "",
        ["judicial_officer.officer_type"] = dto.JudicialOfficerType ?? "",
    };
}
