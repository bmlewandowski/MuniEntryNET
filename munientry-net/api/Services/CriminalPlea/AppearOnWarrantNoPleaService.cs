using Munientry.Shared.Dtos;

namespace Munientry.Api.Services;

public class AppearOnWarrantNoPleaService : DocxServiceBase<AppearOnWarrantNoPleaDto>, IAppearOnWarrantNoPleaService
{
    protected override string TemplatePath { get; } =
        TemplateResolver.Get("No_Plea_Bond_Template.docx");

    protected override Dictionary<string, object> BuildTokens(AppearOnWarrantNoPleaDto dto) => new()
    {
        ["case_number"]                                          = dto.CaseNumber ?? "",
        ["defendant.first_name"]                                 = dto.DefendantFirstName ?? "",
        ["defendant.last_name"]                                  = dto.DefendantLastName ?? "",
        ["bond_conditions.bond_amount"]                          = dto.BondAmount ?? "",
        ["no_contact.name"]                                      = dto.NoContactName ?? "",
        ["custodial_supervision.supervisor"]                     = dto.CustodialSupervisionSupervisor ?? "",
        ["admin_license_suspension.explanation"]                 = dto.AdminLicenseSuspensionExplanation ?? "",
        ["vehicle_seizure.vehicle_make_model"]                   = dto.VehicleMakeModel ?? "",
        ["vehicle_seizure.vehicle_license_plate"]                = dto.VehicleLicensePlate ?? "",
        ["vehicle_seizure.state_opposes"]                        = dto.StateOpposes ?? "",
        ["vehicle_seizure.disposition_motion_to_return"]         = dto.DispositionMotionToReturn ?? "",
        ["domestic_violence_conditions.residence_address"]       = dto.ResidenceAddress ?? "",
        ["domestic_violence_conditions.exclusive_possession_to"] = dto.ExclusivePossessionTo ?? "",
        ["domestic_violence_conditions.surrender_weapons_date"]  = dto.SurrenderWeaponsDate ?? "",
        ["judicial_officer.first_name"]                          = dto.JudicialOfficerFirstName ?? "",
        ["judicial_officer.last_name"]                           = dto.JudicialOfficerLastName ?? "",
        ["judicial_officer.officer_type"]                        = dto.JudicialOfficerType ?? "",
    };
}
