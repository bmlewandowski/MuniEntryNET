using Munientry.DocxTemplating;

namespace Munientry.Api.Endpoints;

/// <summary>
/// Endpoints for bond-related entries: initial hearings, modifications/revocations,
/// and probation-violation bond orders.
/// </summary>
internal static class BondEndpoints
{
    internal static IEndpointRouteBuilder MapBondEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/bondhearing", (Data.BondHearingDto dto) =>
        {
            if (dto == null) return Results.BadRequest();
            var templatePath = Path.Combine("Templates", "source", "Bond_Hearing_Template.docx");
            var outputName = $"BondHearing_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
            try
            {
                var bytes = DocxTemplateProcessor.FillTemplate(templatePath, new Dictionary<string, string>
                {
                    ["case_number"] = dto.CaseNumber ?? "",
                    ["defendant.first_name"] = dto.DefendantFirstName ?? "",
                    ["defendant.last_name"] = dto.DefendantLastName ?? "",
                    ["defense_counsel"] = dto.DefenseCounselName ?? "",
                    ["defense_counsel_type"] = "",
                    ["appearance_reason"] = "",
                    ["plea_trial_date"] = dto.EntryDate?.ToString("MMMM dd, yyyy") ?? "",
                    ["bond_conditions.bond_amount"] = dto.BondAmount ?? "",
                    ["bond_conditions.monitoring_type"] = "",
                    ["bond_conditions.specialized_docket_type"] = "",
                    ["custodial_supervision.supervisor"] = "",
                    ["no_contact.name"] = "",
                    ["other_conditions.terms"] = "",
                    ["domestic_violence_conditions.exclusive_possession_to"] = "",
                    ["domestic_violence_conditions.residence_address"] = "",
                    ["domestic_violence_conditions.surrender_weapons_date"] = "",
                    ["vehicle_seizure.vehicle_make_model"] = "",
                    ["vehicle_seizure.vehicle_license_plate"] = "",
                    ["vehicle_seizure.state_opposes"] = "",
                    ["vehicle_seizure.disposition_motion_to_return"] = "",
                    ["admin_license_suspension.explanation"] = "",
                    ["judicial_officer.first_name"] = "",
                    ["judicial_officer.last_name"] = "",
                    ["judicial_officer.officer_type"] = "",
                });
                return Results.File(bytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", outputName);
            }
            catch (Exception)
            {
                throw;
            }
        });

        app.MapPost("/bondmodificationrevocation", (Data.BondModificationRevocationDto dto) =>
        {
            if (dto == null) return Results.BadRequest();
            var templatePath = Path.Combine("Templates", "source", "Bond_Hearing_Template.docx");
            var outputName = $"BondModificationRevocation_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
            try
            {
                var bytes = DocxTemplateProcessor.FillTemplate(templatePath, new Dictionary<string, string>
                {
                    ["case_number"] = dto.CaseNumber ?? "",
                    ["defendant.first_name"] = dto.DefendantFirstName ?? "",
                    ["defendant.last_name"] = dto.DefendantLastName ?? "",
                    ["defense_counsel"] = dto.DefenseCounselName ?? "",
                    ["defense_counsel_type"] = "",
                    ["appearance_reason"] = dto.DecisionOnBond ?? "",
                    ["plea_trial_date"] = dto.EntryDate?.ToString("MMMM dd, yyyy") ?? "",
                    ["bond_conditions.bond_amount"] = dto.BondAmount ?? "",
                    ["bond_conditions.monitoring_type"] = "",
                    ["bond_conditions.specialized_docket_type"] = "",
                    ["custodial_supervision.supervisor"] = "",
                    ["no_contact.name"] = "",
                    ["other_conditions.terms"] = "",
                    ["domestic_violence_conditions.exclusive_possession_to"] = "",
                    ["domestic_violence_conditions.residence_address"] = "",
                    ["domestic_violence_conditions.surrender_weapons_date"] = "",
                    ["vehicle_seizure.vehicle_make_model"] = "",
                    ["vehicle_seizure.vehicle_license_plate"] = "",
                    ["vehicle_seizure.state_opposes"] = "",
                    ["vehicle_seizure.disposition_motion_to_return"] = "",
                    ["admin_license_suspension.explanation"] = "",
                    ["judicial_officer.first_name"] = "",
                    ["judicial_officer.last_name"] = "",
                    ["judicial_officer.officer_type"] = "",
                });
                return Results.File(bytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", outputName);
            }
            catch (Exception)
            {
                throw;
            }
        });

        app.MapPost("/probationviolationbond", (Data.ProbationViolationBondDto dto) =>
        {
            if (dto == null) return Results.BadRequest();
            var templatePath = Path.Combine("Templates", "source", "Probation_Violation_Bond_Template.docx");
            var outputName = $"ProbationViolationBond_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
            try
            {
                var bytes = DocxTemplateProcessor.FillTemplate(templatePath, new Dictionary<string, string>
                {
                    ["case_number"] = dto.CaseNumber ?? "",
                    ["defendant.first_name"] = dto.DefendantFirstName ?? "",
                    ["defendant.last_name"] = dto.DefendantLastName ?? "",
                    ["defense_counsel"] = dto.DefenseCounselName ?? "",
                    ["defense_counsel_type"] = dto.DefenseCounselType ?? "",
                    ["appearance_reason"] = dto.AppearanceReason ?? "",
                    ["plea_trial_date"] = "",
                    ["cc_bond_conditions.bond_amount"] = dto.BondAmount ?? "",
                    ["cc_bond_conditions.monitoring_type"] = dto.MonitoringType ?? "",
                    ["cc_bond_conditions.cc_violation_other_conditions_terms"] = dto.OtherConditions ? "Yes" : "",
                    ["judicial_officer.first_name"] = "",
                    ["judicial_officer.last_name"] = "",
                    ["judicial_officer.officer_type"] = "",
                });
                return Results.File(bytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", outputName);
            }
            catch (Exception)
            {
                throw;
            }
        });

        return app;
    }
}
