using Munientry.Shared.Dtos;

namespace Munientry.Api.Services;

// TrialSentencingService reuses FinalJuryNoticeDto (same DTO as FinalJuryNoticeService).
public class TrialSentencingService : DocxServiceBase<FinalJuryNoticeDto>, ITrialSentencingService
{
    protected override string TemplatePath { get; } =
        TemplateResolver.Get("Trial_Sentencing_Template.docx");

    protected override Dictionary<string, object> BuildTokens(FinalJuryNoticeDto dto) => new()
    {
        ["case_number"]                   = dto.CaseNumber ?? "",
        ["entry_date"]                    = dto.EntryDate?.ToString("MMMM dd, yyyy") ?? "",
        ["defendant.first_name"]          = dto.DefendantFirstName ?? "",
        ["defendant.last_name"]           = dto.DefendantLastName ?? "",
        ["defense_counsel"]               = dto.DefenseCounselName ?? "",
        ["arrest_summons_date"]           = dto.ArrestSummonsDate?.ToString("MMMM dd, yyyy") ?? "",
        ["highest_charge"]                = dto.HighestCharge ?? "",
        ["days_in_jail"]                  = (dto.DaysInJail ?? 0).ToString(),
        ["continuance_days"]              = (dto.ContinuanceDays ?? 0).ToString(),
        ["trial_to_court.date"]           = dto.TrialToCourtDate?.ToString("MMMM dd, yyyy") ?? "",
        ["trial_to_court.time"]           = dto.TrialToCourtTime ?? "",
        ["trial_to_court.location"]       = dto.AssignedCourtroom ?? "",
        ["interpreter_language"]          = dto.LanguageRequired ?? "",
        ["judicial_officer.first_name"]   = dto.JudicialOfficerFirstName ?? "",
        ["judicial_officer.last_name"]    = dto.JudicialOfficerLastName ?? "",
        ["judicial_officer.officer_type"] = dto.JudicialOfficerType ?? "",
    };
}
