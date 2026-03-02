using Munientry.Shared.Dtos;

namespace Munientry.Api.Services;

public class BondHearingService : DocxServiceBase<BondHearingDto>, IBondHearingService
{
    protected override string TemplatePath { get; } =
        TemplateResolver.Get("Bond_Hearing_Template.docx");

    protected override Dictionary<string, object> BuildTokens(BondHearingDto dto) => new()
    {
        ["case_number"]                                          = dto.CaseNumber ?? "",
        ["defendant.first_name"]                                 = dto.DefendantFirstName ?? "",
        ["defendant.last_name"]                                  = dto.DefendantLastName ?? "",
        ["defense_counsel"]                                      = dto.DefenseCounselName ?? "",
        ["defense_counsel_type"]                                 = "",
        ["appearance_reason"]                                    = "",
        ["plea_trial_date"]                                      = dto.EntryDate?.ToString("MMMM dd, yyyy") ?? "",
        ["bond_conditions.bond_amount"]                          = dto.BondAmount ?? "",
        ["bond_conditions.monitoring_type"]                      = "",
        ["bond_conditions.specialized_docket_type"]              = "",
        ["custodial_supervision.supervisor"]                     = "",
        ["no_contact.name"]                                      = "",
        ["other_conditions.terms"]                               = "",
        ["domestic_violence_conditions.exclusive_possession_to"] = "",
        ["domestic_violence_conditions.residence_address"]       = "",
        ["domestic_violence_conditions.surrender_weapons_date"]  = "",
        ["vehicle_seizure.vehicle_make_model"]                   = "",
        ["vehicle_seizure.vehicle_license_plate"]                = "",
        ["vehicle_seizure.state_opposes"]                        = "",
        ["vehicle_seizure.disposition_motion_to_return"]         = "",
        ["admin_license_suspension.explanation"]                 = "",
        ["judicial_officer.first_name"]                          = dto.JudicialOfficerFirstName ?? "",
        ["judicial_officer.last_name"]                           = dto.JudicialOfficerLastName ?? "",
        ["judicial_officer.officer_type"]                        = dto.JudicialOfficerType ?? "",
    };
}
