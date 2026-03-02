using Munientry.Shared.Dtos;

namespace Munientry.Api.Services;

public class GeneralNoticeOfHearingService : DocxServiceBase<GeneralNoticeOfHearingDto>, IGeneralNoticeOfHearingService
{
    protected override string TemplatePath { get; } =
        TemplateResolver.Get("General_Notice_Of_Hearing_Template.docx");

    protected override Dictionary<string, object> BuildTokens(GeneralNoticeOfHearingDto dto) => new()
    {
        ["case_number"]                   = dto.CaseNumber ?? "",
        ["entry_date"]                    = dto.EntryDate.ToString("MMMM dd, yyyy"),
        ["defendant.first_name"]          = dto.DefendantFirstName ?? "",
        ["defendant.last_name"]           = dto.DefendantLastName ?? "",
        ["hearing.date"]                  = dto.HearingDate.ToString("MMMM dd, yyyy"),
        ["hearing.time"]                  = dto.HearingTime ?? "",
        ["hearing.location"]              = dto.AssignedCourtroom ?? "",
        ["defense_counsel"]               = dto.DefenseCounselName ?? "",
        ["interpreter_language"]          = dto.LanguageRequired ?? "",
        ["judicial_officer.first_name"]   = dto.JudicialOfficerFirstName ?? "",
        ["judicial_officer.last_name"]    = dto.JudicialOfficerLastName ?? "",
        ["judicial_officer.officer_type"] = dto.JudicialOfficerType ?? "",
    };
}
