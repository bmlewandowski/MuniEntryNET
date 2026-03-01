using Munientry.Api;

namespace Munientry.Api.Endpoints;

/// <summary>
/// Endpoints for criminal plea, arraignment, and sentencing entry forms.
/// </summary>
internal static class CriminalPleaEndpoints
{
    internal static IEndpointRouteBuilder MapCriminalPleaEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/fineonly", async (HttpRequest req) =>
        {
            var dto = await req.ReadFromJsonAsync<Data.FineOnlyDto>();
            if (dto == null) return Results.BadRequest();
            var templatePath = Path.Combine("Templates", "source", "Fine_Only_Plea_Final_Judgment_Template.docx");
            var outputName = $"FineOnly_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
            try
            {
                var bytes = DocxTemplateProcessor.FillTemplate(templatePath, new Dictionary<string, string>
                {
                    ["case_number"] = dto.CaseNumber ?? "",
                    ["defendant.first_name"] = dto.DefendantName ?? "",
                    ["charge.offense"] = dto.Charge ?? "",
                    ["fine_amount"] = dto.FineAmount?.ToString("F2") ?? "",
                });
                return Results.File(bytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", outputName);
            }
            catch (Exception)
            {
                throw;
            }
        });

        app.MapPost("/jailccplea", (Data.JailCcPleaDto dto) =>
        {
            if (dto == null) return Results.BadRequest();
            var templatePath = Path.Combine("Templates", "source", "Jail_Plea_Final_Judgment_Template.docx");
            var outputName = $"JailCcPlea_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
            try
            {
                var bytes = DocxTemplateProcessor.FillTemplate(templatePath, new Dictionary<string, object>
                {
                    ["case_number"] = dto.CaseNumber ?? "",
                    ["defendant.first_name"] = dto.DefendantFirstName ?? "",
                    ["defendant.last_name"] = dto.DefendantLastName ?? "",
                    ["defense_counsel"] = dto.DefenseCounselName ?? "",
                    ["defense_counsel_type"] = dto.DefenseCounselType ?? "",
                    ["appearance_reason"] = dto.AppearanceReason ?? "",
                    ["plea_trial_date"] = dto.Date?.ToString("MMMM dd, yyyy") ?? "",
                    ["amend_offense_details.motion_disposition"] = "",
                    ["court_costs.pay_today_amount"] = dto.PayToday?.ToString("F2") ?? "",
                    ["court_costs.monthly_pay_amount"] = dto.MonthlyPay?.ToString("F2") ?? "",
                    ["court_costs.balance_due_date"] = dto.DueDate?.ToString("MMMM dd, yyyy") ?? "",
                    ["court_costs.ability_to_pay_time"] = dto.TimeToPay ?? "",
                    ["fine_jail_days"] = "",
                    ["community_control.type_of_control"] = "",
                    ["community_control.term_of_control"] = "",
                    ["community_control.specialized_docket_type"] = "",
                    ["community_control.house_arrest_time"] = "",
                    ["community_control.alcohol_monitoring_time"] = "",
                    ["community_control.gps_exclusion_radius"] = "",
                    ["community_control.gps_exclusion_location"] = "",
                    ["community_control.community_control_community_service_hours"] = "",
                    ["community_control.no_contact_with_person"] = "",
                    ["community_control.not_within_500_feet_person"] = "",
                    ["community_control.other_community_control_conditions"] = "",
                    ["community_control.pay_restitution_amount"] = "",
                    ["community_control.pay_restitution_to"] = "",
                    ["jail_terms.days_in_jail"] = "",
                    ["jail_terms.total_jail_days_to_serve"] = "",
                    ["jail_terms.report_date"] = "",
                    ["jail_terms.report_time"] = "",
                    ["jail_terms.report_type"] = "",
                    ["jail_terms.jail_report_days_notes"] = "",
                    ["jail_terms.companion_cases_numbers"] = dto.CompanionCases ?? "",
                    ["jail_terms.companion_cases_sentence_type"] = dto.CompanionCasesSentence ?? "",
                    ["license_suspension.license_type"] = "",
                    ["license_suspension.suspension_term"] = "",
                    ["license_suspension.suspended_date"] = "",
                    ["other_conditions.terms"] = "",
                    ["community_service.hours_of_service"] = "",
                    ["community_service.days_to_complete_service"] = "",
                    ["community_service.due_date_for_service"] = "",
                    ["impoundment.vehicle_make_model"] = "",
                    ["impoundment.vehicle_license_plate"] = "",
                    ["impoundment.impound_action"] = "",
                    ["impoundment.impound_time"] = "",
                    ["judicial_officer.first_name"] = "",
                    ["judicial_officer.last_name"] = "",
                    ["judicial_officer.officer_type"] = "",
                    ["charges_list"] = BuildSentencingChargesList(
                        dto.ChargeItems, dto.Charges, dto.ChargeStatute, dto.ChargeDegree,
                        dto.ChargePlea, dto.ChargeFinding, dto.ChargeFinesAmount, dto.ChargeFinesSuspended,
                        dto.ChargeJailDays, dto.ChargeJailDaysSuspended),
                });
                return Results.File(bytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", outputName);
            }
            catch (Exception)
            {
                throw;
            }
        });

        app.MapPost("/notguiltyappearbondspecial", (Data.NotGuiltyAppearBondSpecialDto dto) =>
        {
            if (dto == null) return Results.BadRequest();
            var templatePath = Path.Combine("Templates", "source", "Not_Guilty_Bond_Template.docx");
            var outputName = $"NotGuiltyAppearBondSpecial_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
            try
            {
                var bytes = DocxTemplateProcessor.FillTemplate(templatePath, new Dictionary<string, string>
                {
                    ["case_number"] = dto.CaseNumber ?? "",
                    ["defendant.first_name"] = dto.DefendantFirstName ?? "",
                    ["defendant.last_name"] = dto.DefendantLastName ?? "",
                    ["bond_conditions.bond_amount"] = dto.BondAmount ?? "",
                    ["no_contact.name"] = dto.NoContactName ?? "",
                    ["custodial_supervision.supervisor"] = dto.CustodialSupervisionSupervisor ?? "",
                    ["admin_license_suspension.explanation"] = dto.AdminLicenseSuspensionExplanation ?? "",
                    ["vehicle_seizure.vehicle_make_model"] = dto.VehicleMakeModel ?? "",
                    ["vehicle_seizure.vehicle_license_plate"] = dto.VehicleLicensePlate ?? "",
                    ["vehicle_seizure.state_opposes"] = dto.StateOpposes ?? "",
                    ["vehicle_seizure.disposition_motion_to_return"] = dto.DispositionMotionToReturn ?? "",
                    ["domestic_violence_conditions.residence_address"] = dto.ResidenceAddress ?? "",
                    ["domestic_violence_conditions.exclusive_possession_to"] = dto.ExclusivePossessionTo ?? "",
                    ["domestic_violence_conditions.surrender_weapons_date"] = dto.SurrenderWeaponsDate ?? "",
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

        app.MapPost("/fineonlyplea", (Data.FineOnlyPleaDto dto) =>
        {
            if (dto == null) return Results.BadRequest();
            var templatePath = Path.Combine("Templates", "source", "Fine_Only_Plea_Final_Judgment_Template.docx");
            var outputName = $"FineOnlyPlea_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
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
                    ["charge.offense"] = dto.Charges ?? "",
                    ["charge.statute"] = dto.ChargeStatute ?? "",
                    ["charge.degree"] = dto.ChargeDegree ?? "",
                    ["charge.plea"] = dto.ChargePlea ?? "",
                    ["charge.finding"] = dto.ChargeFinding ?? "",
                    ["charge.fines_amount"] = dto.ChargeFinesAmount ?? "",
                    ["charge.fines_suspended"] = dto.ChargeFinesSuspended ?? "",
                    ["court_costs.pay_today_amount"] = dto.PayToday?.ToString("F2") ?? "",
                    ["court_costs.monthly_pay_amount"] = dto.MonthlyPay?.ToString("F2") ?? "",
                    ["fra_in_file"] = dto.FraInFile ?? "",
                    ["fra_in_court"] = dto.FraInCourt ?? "",
                    ["fine_amount"] = dto.FineAmount?.ToString("F2") ?? "",
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

        app.MapPost("/notguiltyplea", (Data.NotGuiltyPleaDto dto) =>
        {
            if (dto == null) return Results.BadRequest();
            var templatePath = Path.Combine("Templates", "source", "Not_Guilty_Bond_Template.docx");
            var outputName = $"NotGuiltyPlea_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
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
                    ["plea_trial_date"] = dto.PleaDate?.ToString("MMMM dd, yyyy") ?? "",
                    ["charge.offense"] = dto.Charges ?? "",
                    ["bond_conditions.bond_amount"] = dto.BondAmount ?? "",
                    ["bond_conditions.monitoring_type"] = dto.MonitoringType ?? "",
                    ["bond_conditions.specialized_docket_type"] = dto.SpecializedDocketType ?? "",
                    ["court_costs.pay_today_amount"] = dto.CourtCosts ?? "",
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

        app.MapPost("/arraignmentcontinuance", (Data.ArraignmentContinuanceDto dto) =>
        {
            if (dto == null) return Results.BadRequest();
            var templatePath = Path.Combine("Templates", "source", "Arraignment_Continue_Template.docx");
            var outputName = $"ArraignmentContinuance_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
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
                    ["continuance_conditions.current_arraignment_date"] = dto.CurrentArraignmentDate?.ToString("MMMM dd, yyyy") ?? "",
                    ["continuance_conditions.new_arraignment_date"] = dto.NewArraignmentDate?.ToString("MMMM dd, yyyy") ?? "",
                    ["continuance_conditions.continuance_reason"] = dto.ContinuanceReason ?? "",
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

        app.MapPost("/sentencingonlyalreadypead", (Data.SentencingOnlyAlreadyPleadDto dto) =>
        {
            if (dto == null) return Results.BadRequest();
            var templatePath = Path.Combine("Templates", "source", "Sentencing_Only_Template.docx");
            var outputName = $"SentencingOnly_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
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
                    ["plea_trial_date"] = dto.SentencingDate?.ToString("MMMM dd, yyyy") ?? "",
                    ["charge.offense"] = dto.Charges ?? "",
                    ["court_costs.pay_today_amount"] = dto.CourtCosts ?? "",
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

        app.MapPost("/pleaonlyfuturesentencing", (Data.PleaOnlyFutureSentencingDto dto) =>
        {
            if (dto == null) return Results.BadRequest();
            var templatePath = Path.Combine("Templates", "source", "Plea_Only_Template.docx");
            var outputName = $"PleaOnly_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
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
                    ["plea_trial_date"] = dto.SentencingDate?.ToString("MMMM dd, yyyy") ?? "",
                    ["charge.offense"] = dto.Charges ?? "",
                    ["court_costs.pay_today_amount"] = dto.CourtCosts ?? "",
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

        app.MapPost("/appearonwarrantnoplea", (Data.AppearOnWarrantNoPleaDto dto) =>
        {
            if (dto == null) return Results.BadRequest();
            var templatePath = Path.Combine("Templates", "source", "No_Plea_Bond_Template.docx");
            var outputName = $"AppearOnWarrantNoPlea_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
            try
            {
                var bytes = DocxTemplateProcessor.FillTemplate(templatePath, new Dictionary<string, string>
                {
                    ["case_number"] = dto.CaseNumber ?? "",
                    ["defendant.first_name"] = dto.DefendantFirstName ?? "",
                    ["defendant.last_name"] = dto.DefendantLastName ?? "",
                    ["bond_conditions.bond_amount"] = dto.BondAmount ?? "",
                    ["no_contact.name"] = dto.NoContactName ?? "",
                    ["custodial_supervision.supervisor"] = dto.CustodialSupervisionSupervisor ?? "",
                    ["admin_license_suspension.explanation"] = dto.AdminLicenseSuspensionExplanation ?? "",
                    ["vehicle_seizure.vehicle_make_model"] = dto.VehicleMakeModel ?? "",
                    ["vehicle_seizure.vehicle_license_plate"] = dto.VehicleLicensePlate ?? "",
                    ["vehicle_seizure.state_opposes"] = dto.StateOpposes ?? "",
                    ["vehicle_seizure.disposition_motion_to_return"] = dto.DispositionMotionToReturn ?? "",
                    ["domestic_violence_conditions.residence_address"] = dto.ResidenceAddress ?? "",
                    ["domestic_violence_conditions.exclusive_possession_to"] = dto.ExclusivePossessionTo ?? "",
                    ["domestic_violence_conditions.surrender_weapons_date"] = dto.SurrenderWeaponsDate ?? "",
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

        app.MapPost("/trialsentencing", (Data.FinalJuryNoticeDto dto) =>
        {
            if (dto == null) return Results.BadRequest();
            var templatePath = Path.Combine("Templates", "source", "Trial_Sentencing_Template.docx");
            var outputName = $"TrialSentencing_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
            try
            {
                var bytes = DocxTemplateProcessor.FillTemplate(templatePath, new Dictionary<string, string>
                {
                    ["case_number"] = dto.CaseNumber ?? "",
                    ["entry_date"] = dto.EntryDate?.ToString("MMMM dd, yyyy") ?? "",
                    ["defendant.first_name"] = dto.DefendantFirstName ?? "",
                    ["defendant.last_name"] = dto.DefendantLastName ?? "",
                    ["defense_counsel"] = dto.DefenseCounselName ?? "",
                    ["arrest_summons_date"] = dto.ArrestSummonsDate?.ToString("MMMM dd, yyyy") ?? "",
                    ["highest_charge"] = dto.HighestCharge ?? "",
                    ["days_in_jail"] = (dto.DaysInJail ?? 0).ToString(),
                    ["continuance_days"] = (dto.ContinuanceDays ?? 0).ToString(),
                    ["trial_to_court.date"] = dto.TrialToCourtDate?.ToString("MMMM dd, yyyy") ?? "",
                    ["trial_to_court.time"] = dto.TrialToCourtTime ?? "",
                    ["trial_to_court.location"] = dto.AssignedCourtroom ?? "",
                    ["interpreter_language"] = dto.LanguageRequired ?? "",
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

        app.MapPost("/failuretooappear", (Data.FailureToAppearDto dto) =>
        {
            if (dto == null) return Results.BadRequest();
            var templatePath = Path.Combine("Templates", "source", "Failure_To_Appear_Template.docx");
            var outputName = $"FailureToAppear_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
            try
            {
                var bytes = DocxTemplateProcessor.FillTemplate(templatePath, new Dictionary<string, string>
                {
                    ["case_number"] = dto.CaseNumber ?? "",
                    ["defendant.first_name"] = dto.DefendantFirstName ?? "",
                    ["defendant.last_name"] = dto.DefendantLastName ?? "",
                    ["appearance_reason"] = dto.AppearanceReason ?? "",
                    ["plea_trial_date"] = dto.EntryDate?.ToString("MMMM dd, yyyy") ?? "",
                    ["fta_conditions.arrest_warrant_radius"] = dto.ArrestWarrantRadius ?? "",
                    ["fta_conditions.bond_type"] = dto.BondType ?? "",
                    ["fta_conditions.bond_amount"] = dto.BondAmount ?? "",
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

        app.MapPost("/freeformentry", (Data.CriminalFreeformEntryDto dto) =>
        {
            if (dto == null) return Results.BadRequest();
            var templatePath = Path.Combine("Templates", "source", "Freeform_Entry_Template.docx");
            var outputName = $"FreeformEntry_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
            try
            {
                var bytes = DocxTemplateProcessor.FillTemplate(templatePath, new Dictionary<string, string>
                {
                    ["case_number"] = dto.CaseNumber ?? "",
                    ["defendant.first_name"] = dto.DefendantFirstName ?? "",
                    ["defendant.last_name"] = dto.DefendantLastName ?? "",
                    ["defense_counsel"] = dto.DefenseCounselName ?? "",
                    ["defense_counsel_type"] = dto.DefenseCounselType ?? "",
                    ["appearance_reason"] = "",
                    ["plea_trial_date"] = dto.EntryDate?.ToString("MMMM dd, yyyy") ?? "",
                    ["entry_content_text"] = dto.EntryContent ?? "",
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

    /// <summary>
    /// Builds the <c>charges_list</c> for jail/sentencing forms.
    /// Falls back to legacy flat fields when <paramref name="items"/> is empty.
    /// </summary>
    private static List<Dictionary<string, string>> BuildSentencingChargesList(
        List<Data.SentencingChargeItemDto> items,
        string? offense, string? statute, string? degree, string? plea,
        string? finding, string? finesAmount, string? finesSuspended,
        string? jailDays = null, string? jailDaysSuspended = null)
    {
        if (items.Count > 0)
            return items
                .Select(c => new Dictionary<string, string>
                {
                    ["offense"]             = c.Offense,
                    ["statute"]             = c.Statute,
                    ["degree"]              = c.Degree,
                    ["plea"]                = c.Plea,
                    ["finding"]             = c.Finding,
                    ["fines_amount"]        = c.FinesAmount,
                    ["fines_suspended"]     = c.FinesSuspended,
                    ["jail_days"]           = c.JailDays,
                    ["jail_days_suspended"] = c.JailDaysSuspended,
                })
                .ToList();

        return new()
        {
            new()
            {
                ["offense"]             = offense          ?? "",
                ["statute"]             = statute          ?? "",
                ["degree"]              = degree           ?? "",
                ["plea"]                = plea             ?? "",
                ["finding"]             = finding          ?? "",
                ["fines_amount"]        = finesAmount      ?? "",
                ["fines_suspended"]     = finesSuspended   ?? "",
                ["jail_days"]           = jailDays         ?? "",
                ["jail_days_suspended"] = jailDaysSuspended ?? "",
            },
        };
    }
}
