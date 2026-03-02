using Munientry.Shared.Dtos;

namespace Munientry.Api.Services;

public class CriminalFreeformEntryService : DocxServiceBase<CriminalFreeformEntryDto>, ICriminalFreeformEntryService
{
    protected override string TemplatePath { get; } =
        TemplateResolver.Get("Freeform_Entry_Template.docx");

    protected override Dictionary<string, object> BuildTokens(CriminalFreeformEntryDto dto) => new()
    {
        ["case_number"]                   = dto.CaseNumber ?? "",
        ["defendant.first_name"]          = dto.DefendantFirstName ?? "",
        ["defendant.last_name"]           = dto.DefendantLastName ?? "",
        ["defense_counsel"]               = dto.DefenseCounselName ?? "",
        ["defense_counsel_type"]          = dto.DefenseCounselType ?? "",
        ["appearance_reason"]             = "",
        ["plea_trial_date"]               = dto.EntryDate?.ToString("MMMM dd, yyyy") ?? "",
        ["entry_content_text"]            = dto.EntryContent ?? "",
        ["judicial_officer.first_name"]   = dto.JudicialOfficerFirstName ?? "",
        ["judicial_officer.last_name"]    = dto.JudicialOfficerLastName ?? "",
        ["judicial_officer.officer_type"] = dto.JudicialOfficerType ?? "",
    };
}
