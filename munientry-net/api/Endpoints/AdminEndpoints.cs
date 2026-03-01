using Munientry.Api;

namespace Munientry.Api.Endpoints;

/// <summary>
/// Endpoints for administrative court entries: fiscal journal, time-to-pay orders,
/// criminal sealing, competency evaluations, and failure-to-appear warrants.
/// </summary>
internal static class AdminEndpoints
{
    internal static IEndpointRouteBuilder MapAdminEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/fiscaljournalentry", async (Data.FiscalJournalEntryDto dto, Services.IFiscalJournalEntryService service) =>
        {
            service.InsertFiscalJournalEntry(dto);
            return Results.Ok();
        });

        app.MapPost("/timetopayorder", async (Data.TimeToPayOrderDto dto, Services.ITimeToPayOrderService service) =>
        {
            if (dto == null) return Results.BadRequest();
            var templatePath = Path.Combine("Templates", "source", "Time_To_Pay_Template.docx");
            var outputName = $"TimeToPay_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
            try
            {
                var bytes = DocxTemplateProcessor.FillTemplate(templatePath, new Dictionary<string, string>
                {
                    ["case_number"] = dto.CaseNumber ?? "",
                    ["entry_date"] = dto.EntryDate.ToString("MMMM dd, yyyy"),
                    ["defendant.first_name"] = dto.DefendantFirstName ?? "",
                    ["defendant.last_name"] = dto.DefendantLastName ?? "",
                    ["appearance_date"] = dto.AppearanceDate.ToString("MMMM dd, yyyy"),
                    // TODO: populate from authenticated judge context
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

        app.MapPost("/criminalsealing", (Data.CriminalSealingEntryDto dto) =>
        {
            if (dto == null) return Results.BadRequest();
            var templatePath = Path.Combine("Templates", "source", "Criminal_Sealing_Entry_Template.docx");
            var outputName = $"CriminalSealingEntry_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
            try
            {
                var bytes = DocxTemplateProcessor.FillTemplate(templatePath, new Dictionary<string, string>
                {
                    ["case_number"] = dto.CaseNumber ?? "",
                    ["defendant.first_name"] = dto.DefendantFirstName ?? "",
                    ["defendant.last_name"] = dto.DefendantLastName ?? "",
                    ["defense_counsel"] = dto.DefenseCounselName ?? "",
                    ["defense_counsel_type"] = dto.DefenseCounselType ?? "",
                    ["plea_trial_date"] = dto.EntryDate?.ToString("MMMM dd, yyyy") ?? "",
                    ["seal_decision"] = dto.SealDecision ?? "",
                    ["denial_reasons"] = dto.DenialReasons ?? "",
                    ["bci_number"] = dto.BciNumber ?? "",
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

        app.MapPost("/competencyevaluation", (Data.CompetencyEvaluationDto dto) =>
        {
            if (dto == null) return Results.BadRequest();
            var templatePath = Path.Combine("Templates", "source", "Competency_Evaluation_Template.docx");
            var outputName = $"CompetencyEvaluation_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
            try
            {
                var bytes = DocxTemplateProcessor.FillTemplate(templatePath, new Dictionary<string, string>
                {
                    ["case_number"] = dto.CaseNumber ?? "",
                    ["defendant.first_name"] = dto.DefendantFirstName ?? "",
                    ["defendant.last_name"] = dto.DefendantLastName ?? "",
                    ["plea_trial_date"] = dto.EntryDate?.ToString("MMMM dd, yyyy") ?? "",
                    // treatment_type maps the evaluator/evaluation context in the template
                    ["treatment_type"] = dto.EvaluatorName ?? "",
                    // final_pretrial.date/time holds the competency hearing in the template
                    ["final_pretrial.date"] = dto.CompetencyHearingDate?.ToString("MMMM dd, yyyy") ?? "",
                    ["final_pretrial.time"] = dto.CompetencyHearingType ?? "",
                    // jury_trial.date holds the evaluation date in the template
                    ["jury_trial.date"] = dto.EvaluationDate?.ToString("MMMM dd, yyyy") ?? "",
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
