using Munientry.Api;

namespace Munientry.Api.Endpoints;

/// <summary>
/// Endpoints for diversion program entries.
/// </summary>
internal static class DiversionEndpoints
{
    internal static IEndpointRouteBuilder MapDiversionEndpoints(this IEndpointRouteBuilder app)
    {
        // Stub — body is echoed back; replace with DOCX generation when DTO is finalised.
        app.MapPost("/diversion", async (HttpRequest req) =>
        {
            var body = await req.ReadFromJsonAsync<object>();
            return Results.Ok(body);
        });

        app.MapPost("/diversiondialog", (Data.DiversionDialogDto dto) =>
        {
            if (dto == null) return Results.BadRequest();
            var templatePath = Path.Combine("Templates", "source", "Diversion_Template.docx");
            var outputName = $"DiversionDialog_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
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
                    ["plea_trial_date"] = dto.DiversionDate?.ToString("MMMM dd, yyyy") ?? "",
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

        app.MapPost("/diversionplea", (Data.DiversionPleaDto dto) =>
        {
            if (dto == null) return Results.BadRequest();
            var templatePath = Path.Combine("Templates", "source", "Diversion_Template.docx");
            var outputName = $"DiversionPlea_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
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
                    ["diversion.program_name"] = dto.DiversionType ?? "",
                    ["diversion.diversion_completion_date"] = dto.DiversionCompletionDate?.ToString("MMMM dd, yyyy") ?? "",
                    ["diversion.diversion_fine_pay_date"] = dto.DiversionFinePayDate?.ToString("MMMM dd, yyyy") ?? "",
                    ["diversion.pay_restitution_to"] = dto.PayRestitutionTo ?? "",
                    ["diversion.pay_restitution_amount"] = dto.PayRestitutionAmount?.ToString("F2") ?? "",
                    ["diversion.other_conditions_text"] = dto.OtherConditionsText ?? "",
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
