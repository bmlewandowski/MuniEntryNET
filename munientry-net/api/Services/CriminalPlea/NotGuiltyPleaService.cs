using Munientry.Shared.Dtos;

namespace Munientry.Api.Services;

public class NotGuiltyPleaService : DocxServiceBase<NotGuiltyPleaDto>, INotGuiltyPleaService
{
    protected override string TemplatePath { get; } =
        TemplateResolver.Get("Not_Guilty_Bond_Template.docx");

    protected override Dictionary<string, object> BuildTokens(NotGuiltyPleaDto dto) => new()
    {
        ["case_number"]                             = dto.CaseNumber ?? "",
        ["defendant.first_name"]                    = dto.DefendantFirstName ?? "",
        ["defendant.last_name"]                     = dto.DefendantLastName ?? "",
        ["defense_counsel"]                         = dto.DefenseCounselName ?? "",
        ["defense_counsel_type"]                    = dto.DefenseCounselType ?? "",
        ["appearance_reason"]                       = dto.AppearanceReason ?? "",
        ["plea_trial_date"]                         = dto.PleaDate?.ToString("MMMM dd, yyyy") ?? "",
        ["charge.offense"]                          = dto.Charges ?? "",
        ["bond_conditions.bond_amount"]             = dto.BondAmount ?? "",
        ["bond_conditions.monitoring_type"]         = dto.MonitoringType ?? "",
        ["bond_conditions.specialized_docket_type"] = dto.SpecializedDocketType ?? "",
        ["court_costs.pay_today_amount"]            = dto.CourtCosts ?? "",
        ["judicial_officer.first_name"]             = dto.JudicialOfficerFirstName ?? "",
        ["judicial_officer.last_name"]              = dto.JudicialOfficerLastName ?? "",
        ["judicial_officer.officer_type"]           = dto.JudicialOfficerType ?? "",
    };
}
