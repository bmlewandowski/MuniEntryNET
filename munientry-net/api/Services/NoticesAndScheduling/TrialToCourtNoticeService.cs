using Munientry.Shared.Dtos;

namespace Munientry.Api.Services;

public class TrialToCourtNoticeService : DocxServiceBase<TrialToCourtNoticeDto>, ITrialToCourtNoticeService
{
    protected override string TemplatePath { get; } =
        TemplateResolver.Get("Trial_To_Court_Hearing_Notice_Template.docx");

    protected override Dictionary<string, object> BuildTokens(TrialToCourtNoticeDto dto) => new()
    {
        ["case_number"]                   = dto.CaseNumber ?? "",
        ["defendant.first_name"]          = dto.DefendantFirstName ?? "",
        ["defendant.last_name"]           = dto.DefendantLastName ?? "",
        ["entry_date"]                    = dto.EntryDate?.ToString("MMMM dd, yyyy") ?? "",
        ["defense_counsel"]               = dto.DefenseCounselName ?? "",
        ["trial_to_court.date"]           = dto.TrialToCourtDate?.ToString("MMMM dd, yyyy") ?? "",
        ["trial_to_court.time"]           = dto.TrialToCourtTime ?? "",
        ["trial_to_court.location"]       = dto.AssignedCourtroom ?? "",
        ["interpreter_required"]          = dto.InterpreterRequired ? "Yes" : "No",
        ["interpreter_language"]          = dto.LanguageRequired ?? "",
        ["date_confirmed_with_counsel"]   = dto.DateConfirmedWithCounsel ? "Yes" : "No",
        ["assigned_judge"]                = "",
        ["judicial_officer.first_name"]   = dto.JudicialOfficerFirstName ?? "",
        ["judicial_officer.last_name"]    = dto.JudicialOfficerLastName ?? "",
        ["judicial_officer.officer_type"] = dto.JudicialOfficerType ?? "",
    };
}
