using Munientry.DocxTemplating;

namespace Munientry.Api.Endpoints;

/// <summary>
/// Endpoints for hearing notices, scheduling entries, jury management, and trial notices.
/// </summary>
internal static class NoticesAndSchedulingEndpoints
{
    internal static IEndpointRouteBuilder MapNoticesAndSchedulingEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/trialtocourt", async (Data.TrialToCourtNoticeDto dto, Services.ITrialToCourtNoticeService service) =>
        {
            if (dto == null) return Results.BadRequest();
            var templatePath = Path.Combine("Templates", "source", "Trial_To_Court_Hearing_Notice_Template.docx");
            var outputName = $"TrialToCourtNotice_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
            try
            {
                var bytes = DocxTemplateProcessor.FillTemplate(templatePath, new Dictionary<string, string>
                {
                    ["case_number"] = dto.CaseNumber ?? "",
                    ["defendant.first_name"] = dto.DefendantFirstName ?? "",
                    ["defendant.last_name"] = dto.DefendantLastName ?? "",
                    ["entry_date"] = dto.EntryDate?.ToString("MMMM dd, yyyy") ?? "",
                    ["defense_counsel"] = dto.DefenseCounselName ?? "",
                    ["trial_to_court.date"] = dto.TrialToCourtDate?.ToString("MMMM dd, yyyy") ?? "",
                    ["trial_to_court.time"] = dto.TrialToCourtTime ?? "",
                    ["trial_to_court.location"] = dto.AssignedCourtroom ?? "",
                    ["interpreter_required"] = dto.InterpreterRequired ? "Yes" : "No",
                    ["interpreter_language"] = dto.LanguageRequired ?? "",
                    ["date_confirmed_with_counsel"] = dto.DateConfirmedWithCounsel ? "Yes" : "No",
                    // TODO: populate from authenticated judge context
                    ["assigned_judge"] = "",
                    ["judicial_officer.first_name"] = "",
                    ["judicial_officer.last_name"] = "",
                    ["judicial_officer.officer_type"] = "",
                });
                // --- DB Save Logic (commented out for isolated DOCX test) ---
                // await service.SaveToDatabaseAsync(dto);
                // --- End DB Save Logic ---
                return Results.File(bytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", outputName);
            }
            catch (Exception)
            {
                throw;
            }
        });

        app.MapPost("/finaljury", (Data.FinalJuryNoticeDto dto) =>
        {
            if (dto == null) return Results.BadRequest();
            var templatePath = Path.Combine("Templates", "source", "Final_Jury_Notice_Of_Hearing_Template.docx");
            var outputName = $"FinalJuryNotice_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
            try
            {
                var bytes = DocxTemplateProcessor.FillTemplate(templatePath, new Dictionary<string, string>
                {
                    ["case_number"] = dto.CaseNumber ?? "",
                    ["defendant.first_name"] = dto.DefendantFirstName ?? "",
                    ["defendant.last_name"] = dto.DefendantLastName ?? "",
                    ["assigned_judge"] = "",
                    ["final_pretrial.date"] = dto.FinalJuryDate?.ToString("MMMM dd, yyyy") ?? "",
                    ["final_pretrial.time"] = dto.FinalJuryTime ?? "",
                    ["jury_trial.date"] = "",
                    ["jury_trial.time"] = "",
                    ["jury_trial.location"] = dto.AssignedCourtroom ?? "",
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

        app.MapPost("/generalnoticeofhearing", (Data.GeneralNoticeOfHearingDto dto) =>
        {
            if (dto == null) return Results.BadRequest();
            var templatePath = Path.Combine("Templates", "source", "General_Notice_Of_Hearing_Template.docx");
            var outputName = $"GeneralNoticeOfHearing_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
            try
            {
                var bytes = DocxTemplateProcessor.FillTemplate(templatePath, new Dictionary<string, string>
                {
                    ["case_number"] = dto.CaseNumber ?? "",
                    ["entry_date"] = dto.EntryDate.ToString("MMMM dd, yyyy"),
                    ["defendant.first_name"] = dto.DefendantFirstName ?? "",
                    ["defendant.last_name"] = dto.DefendantLastName ?? "",
                    ["hearing.date"] = dto.HearingDate.ToString("MMMM dd, yyyy"),
                    ["hearing.time"] = dto.HearingTime ?? "",
                    ["hearing.location"] = dto.AssignedCourtroom ?? "",
                    ["defense_counsel"] = dto.DefenseCounselName ?? "",
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

        // Template is selected by JudicialOfficer; mirrors Python SchedulingEntryCreator.set_template().
        app.MapPost("/schedulingentry", (Data.SchedulingEntryDto dto) =>
        {
            if (dto == null) return Results.BadRequest();
            var templateFile = (dto.JudicialOfficer?.Trim().ToLower()) switch
            {
                "fowler"   => "Scheduling_Entry_Template_Fowler.docx",
                "hemmeter" => "Scheduling_Entry_Template_Hemmeter.docx",
                _          => "Scheduling_Entry_Template_Rohrer.docx"
            };
            var templatePath = Path.Combine("Templates", "source", templateFile);
            var outputName = $"SchedulingEntry_{dto.JudicialOfficer}_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
            try
            {
                var bytes = DocxTemplateProcessor.FillTemplate(templatePath, new Dictionary<string, string>
                {
                    ["case_number"] = dto.CaseNumber ?? "",
                    ["defendant.first_name"] = dto.DefendantFirstName ?? "",
                    ["defendant.last_name"] = dto.DefendantLastName ?? "",
                    ["arrest_summons_date"] = dto.ArrestSummonsDate?.ToString("MMMM dd, yyyy") ?? "",
                    ["defense_counsel"] = dto.DefenseCounsel ?? "",
                    ["highest_charge"] = dto.HighestCharge ?? "",
                    ["days_in_jail"] = (dto.DaysInJail ?? 0).ToString(),
                    ["continuance_days"] = (dto.ContinuanceDays ?? 0).ToString(),
                    ["pretrial.date"] = dto.PretrialDate?.ToString("MMMM dd, yyyy") ?? "",
                    ["final_pretrial.date"] = dto.FinalPretrialDate?.ToString("MMMM dd, yyyy") ?? "",
                    ["final_pretrial.time"] = dto.FinalPretrialTime ?? "",
                    ["jury_trial.date"] = dto.JuryTrialDate?.ToString("MMMM dd, yyyy") ?? "",
                    ["jury_trial.time"] = "",
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

        app.MapPost("/jurorpayment", (Data.JurorPaymentDto dto) =>
        {
            if (dto == null) return Results.BadRequest();
            var templatePath = Path.Combine("Templates", "source", "Jury_Payment_Template.docx");
            var outputName = $"JurorPayment_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
            try
            {
                var bytes = DocxTemplateProcessor.FillTemplate(templatePath, new Dictionary<string, string>
                {
                    ["case_number"] = dto.CaseNumber ?? "",
                    ["entry_date"] = dto.EntryDate.ToString("MMMM dd, yyyy"),
                    ["defendant.first_name"] = dto.DefendantFirstName ?? "",
                    ["defendant.last_name"] = dto.DefendantLastName ?? "",
                    ["trial_date"] = dto.TrialDate.ToString("MMMM dd, yyyy"),
                    ["jurors_reported"] = dto.JurorsReported.ToString(),
                    ["jurors_seated"] = dto.JurorsSeated.ToString(),
                    ["jurors_not_seated"] = dto.JurorsNotSeated.ToString(),
                    ["jurors_pay_not_seated"] = dto.JurorsPayNotSeated.ToString(),
                    ["jurors_pay_seated"] = dto.JurorsPaySeated.ToString(),
                    ["jury_panel_total_pay"] = dto.JuryPanelTotalPay.ToString(),
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
