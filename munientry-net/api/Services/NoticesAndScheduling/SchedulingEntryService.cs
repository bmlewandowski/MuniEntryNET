using Microsoft.Extensions.Options;
using Munientry.Api.Options;
using Munientry.Shared.Dtos;

namespace Munientry.Api.Services;

public class SchedulingEntryService : ISchedulingEntryService
{
    private readonly SchedulingOptions _opts;

    public SchedulingEntryService(IOptions<SchedulingOptions> options)
        => _opts = options.Value;

    // Template path is dynamic (keyed by judicial officer) so DocxServiceBase is not used.
    public byte[] GenerateDocx(SchedulingEntryDto dto)
    {
        var key = dto.JudicialOfficer?.Trim() ?? "";
        var templateFile = _opts.JudgeTemplates.TryGetValue(key, out var t)
            ? t
            : _opts.DefaultTemplate;
        return DocxTemplateProcessor.FillTemplate(
            TemplateResolver.Get(templateFile),
            BuildTokens(dto));
    }

    private static Dictionary<string, string> BuildTokens(SchedulingEntryDto dto) => new()
    {
        ["case_number"]                   = dto.CaseNumber ?? "",
        ["defendant.first_name"]          = dto.DefendantFirstName ?? "",
        ["defendant.last_name"]           = dto.DefendantLastName ?? "",
        ["arrest_summons_date"]           = dto.ArrestSummonsDate?.ToString("MMMM dd, yyyy") ?? "",
        ["defense_counsel"]               = dto.DefenseCounsel ?? "",
        ["highest_charge"]                = dto.HighestCharge ?? "",
        ["days_in_jail"]                  = (dto.DaysInJail ?? 0).ToString(),
        ["continuance_days"]              = (dto.ContinuanceDays ?? 0).ToString(),
        ["pretrial.date"]                 = dto.PretrialDate?.ToString("MMMM dd, yyyy") ?? "",
        ["final_pretrial.date"]           = dto.FinalPretrialDate?.ToString("MMMM dd, yyyy") ?? "",
        ["final_pretrial.time"]           = dto.FinalPretrialTime ?? "",
        ["jury_trial.date"]               = dto.JuryTrialDate?.ToString("MMMM dd, yyyy") ?? "",
        ["jury_trial.time"]               = "",
        ["interpreter_language"]          = dto.LanguageRequired ?? "",
        ["judicial_officer.first_name"]   = dto.JudicialOfficerFirstName ?? "",
        ["judicial_officer.last_name"]    = dto.JudicialOfficerLastName ?? "",
        ["judicial_officer.officer_type"] = dto.JudicialOfficerType ?? "",
    };
}
