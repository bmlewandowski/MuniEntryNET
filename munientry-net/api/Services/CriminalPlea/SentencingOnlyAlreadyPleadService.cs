using Munientry.Shared.Dtos;

namespace Munientry.Api.Services;

public class SentencingOnlyAlreadyPleadService : DocxServiceBase<SentencingOnlyAlreadyPleadDto>, ISentencingOnlyAlreadyPleadService
{
    protected override string TemplatePath { get; } =
        TemplateResolver.Get("Sentencing_Only_Template.docx");

    protected override Dictionary<string, object> BuildTokens(SentencingOnlyAlreadyPleadDto dto) => new()
    {
        ["case_number"]                   = dto.CaseNumber ?? "",
        ["defendant.first_name"]          = dto.DefendantFirstName ?? "",
        ["defendant.last_name"]           = dto.DefendantLastName ?? "",
        ["defense_counsel"]               = dto.DefenseCounselName ?? "",
        ["defense_counsel_type"]          = dto.DefenseCounselType ?? "",
        ["appearance_reason"]             = dto.AppearanceReason ?? "",
        ["plea_trial_date"]               = dto.SentencingDate?.ToString("MMMM dd, yyyy") ?? "",
        ["charge.offense"]                = dto.Charges ?? "",
        ["court_costs.pay_today_amount"]  = dto.CourtCosts ?? "",
        ["judicial_officer.first_name"]   = dto.JudicialOfficerFirstName ?? "",
        ["judicial_officer.last_name"]    = dto.JudicialOfficerLastName ?? "",
        ["judicial_officer.officer_type"] = dto.JudicialOfficerType ?? "",
    };
}