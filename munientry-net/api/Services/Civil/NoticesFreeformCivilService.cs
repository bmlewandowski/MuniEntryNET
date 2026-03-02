using Munientry.Shared.Dtos;

namespace Munientry.Api.Services;

public class NoticesFreeformCivilService : DocxServiceBase<NoticesFreeformCivilDto>, INoticesFreeformCivilService
{
    protected override string TemplatePath { get; } =
        TemplateResolver.Get("Civil_Freeform_Entry_Template.docx");

    protected override Dictionary<string, object> BuildTokens(NoticesFreeformCivilDto dto) => new()
    {
        ["case_number"]                   = dto.CaseNumber ?? "",
        ["entry_date"]                    = dto.EntryDate.ToString("MMMM dd, yyyy"),
        ["defendant.first_name"]          = dto.DefendantFirstName ?? "",
        ["defendant.last_name"]           = dto.DefendantLastName ?? "",
        ["entry_content_text"]            = dto.NoticeText ?? "",
        ["plea_trial_date"]               = "",
        ["judicial_officer.first_name"]   = dto.JudicialOfficerFirstName ?? "",
        ["judicial_officer.last_name"]    = dto.JudicialOfficerLastName ?? "",
        ["judicial_officer.officer_type"] = dto.JudicialOfficerType ?? "",
    };
}
