using Munientry.DocxTemplating;

namespace Munientry.Api.Endpoints;

/// <summary>
/// Endpoints for civil entry forms.
/// </summary>
internal static class CivilEndpoints
{
    internal static IEndpointRouteBuilder MapCivilEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/noticesfreeformcivil", (Data.NoticesFreeformCivilDto dto) =>
        {
            if (dto == null) return Results.BadRequest();
            var templatePath = Path.Combine("Templates", "source", "Notices_Freeform_Civil_Template.docx");
            var outputName = $"NoticesFreeformCivil_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
            try
            {
                var bytes = DocxTemplateProcessor.FillTemplate(templatePath, new Dictionary<string, string>
                {
                    ["case_number"] = dto.CaseNumber ?? "",
                    ["entry_date"] = dto.EntryDate.ToString("MMMM dd, yyyy"),
                    ["defendant.first_name"] = dto.DefendantFirstName ?? "",
                    ["defendant.last_name"] = dto.DefendantLastName ?? "",
                    ["entry_content_text"] = dto.NoticeText ?? "",
                    ["plea_trial_date"] = "",
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

        app.MapPost("/civilfreeformentry", (Data.CivilFreeformEntryDto dto) =>
        {
            if (dto == null) return Results.BadRequest();
            var templatePath = Path.Combine("Templates", "source", "Civil_Freeform_Entry_Template.docx");
            var outputName = $"CivilFreeformEntry_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
            try
            {
                var bytes = DocxTemplateProcessor.FillTemplate(templatePath, new Dictionary<string, string>
                {
                    ["case_number"] = dto.CaseNumber ?? "",
                    ["entry_date"] = dto.EntryDate.ToString("MMMM dd, yyyy"),
                    ["defendant.last_name"] = dto.Defendant ?? "",
                    ["plaintiff.party_name"] = dto.Plaintiff ?? "",
                    ["appearance_reason"] = dto.AppearanceReason ?? "",
                    ["defense_counsel"] = "",
                    ["defense_counsel_type"] = "",
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
}
