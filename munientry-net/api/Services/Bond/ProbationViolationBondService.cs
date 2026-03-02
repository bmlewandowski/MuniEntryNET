using Munientry.Shared.Dtos;

namespace Munientry.Api.Services;

public class ProbationViolationBondService : DocxServiceBase<ProbationViolationBondDto>, IProbationViolationBondService
{
    protected override string TemplatePath { get; } =
        TemplateResolver.Get("Probation_Violation_Bond_Template.docx");

    protected override Dictionary<string, object> BuildTokens(ProbationViolationBondDto dto) => new()
    {
        ["case_number"]                                          = dto.CaseNumber ?? "",
        ["defendant.first_name"]                                 = dto.DefendantFirstName ?? "",
        ["defendant.last_name"]                                  = dto.DefendantLastName ?? "",
        ["defense_counsel"]                                      = dto.DefenseCounselName ?? "",
        ["defense_counsel_type"]                                 = dto.DefenseCounselType ?? "",
        ["appearance_reason"]                                    = dto.AppearanceReason ?? "",
        ["plea_trial_date"]                                      = "",
        ["cc_bond_conditions.bond_amount"]                       = dto.BondAmount ?? "",
        ["cc_bond_conditions.monitoring_type"]                   = dto.MonitoringType ?? "",
        ["cc_bond_conditions.cc_violation_other_conditions_terms"] = dto.OtherConditions ? "Yes" : "",
        ["judicial_officer.first_name"]                          = dto.JudicialOfficerFirstName ?? "",
        ["judicial_officer.last_name"]                           = dto.JudicialOfficerLastName ?? "",
        ["judicial_officer.officer_type"]                        = dto.JudicialOfficerType ?? "",
    };
}
