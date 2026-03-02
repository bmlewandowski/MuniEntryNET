using Munientry.Shared.Dtos;

namespace Munientry.Api.Services;

public class FinalJuryNoticeService : DocxServiceBase<FinalJuryNoticeDto>, IFinalJuryNoticeService
{
    protected override string TemplatePath { get; } =
        TemplateResolver.Get("Final_Jury_Notice_Of_Hearing_Template.docx");

    protected override Dictionary<string, object> BuildTokens(FinalJuryNoticeDto dto) => new()
    {
        ["case_number"]                   = dto.CaseNumber ?? "",
        ["defendant.first_name"]          = dto.DefendantFirstName ?? "",
        ["defendant.last_name"]           = dto.DefendantLastName ?? "",
        ["assigned_judge"]                = "",
        ["final_pretrial.date"]           = dto.FinalJuryDate?.ToString("MMMM dd, yyyy") ?? "",
        ["final_pretrial.time"]           = dto.FinalJuryTime ?? "",
        ["jury_trial.date"]               = "",
        ["jury_trial.time"]               = "",
        ["jury_trial.location"]           = dto.AssignedCourtroom ?? "",
        ["interpreter_language"]          = dto.LanguageRequired ?? "",
        ["judicial_officer.first_name"]   = dto.JudicialOfficerFirstName ?? "",
        ["judicial_officer.last_name"]    = dto.JudicialOfficerLastName ?? "",
        ["judicial_officer.officer_type"] = dto.JudicialOfficerType ?? "",
    };
}
