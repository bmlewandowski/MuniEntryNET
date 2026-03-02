using Munientry.Shared.Dtos;

namespace Munientry.Api.Services;

public class CriminalSealingEntryService : DocxServiceBase<CriminalSealingEntryDto>, ICriminalSealingEntryService
{
    protected override string TemplatePath { get; } =
        TemplateResolver.Get("Criminal_Sealing_Entry_Template.docx");

    protected override Dictionary<string, object> BuildTokens(CriminalSealingEntryDto dto) => new()
    {
        ["case_number"]                   = dto.CaseNumber ?? "",
        ["defendant.first_name"]          = dto.DefendantFirstName ?? "",
        ["defendant.last_name"]           = dto.DefendantLastName ?? "",
        ["defense_counsel"]               = dto.DefenseCounselName ?? "",
        ["defense_counsel_type"]          = dto.DefenseCounselType ?? "",
        ["plea_trial_date"]               = dto.EntryDate?.ToString("MMMM dd, yyyy") ?? "",
        ["seal_decision"]                 = dto.SealDecision ?? "",
        ["denial_reasons"]                = dto.DenialReasons ?? "",
        ["bci_number"]                    = dto.BciNumber ?? "",
        ["judicial_officer.first_name"]   = dto.JudicialOfficerFirstName ?? "",
        ["judicial_officer.last_name"]    = dto.JudicialOfficerLastName ?? "",
        ["judicial_officer.officer_type"] = dto.JudicialOfficerType ?? "",
    };
}
