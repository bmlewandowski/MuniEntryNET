using Munientry.Api;

namespace Munientry.Api.Endpoints;

/// <summary>
/// Endpoints for community control / probation entries:
/// CC terms notices, CC terms orders, and community service secondary entries.
/// </summary>
internal static class ProbationEndpoints
{
    internal static IEndpointRouteBuilder MapProbationEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/communitycontroltermsnotices", (Data.CommunityControlTermsNoticesDto dto) =>
        {
            if (dto == null) return Results.BadRequest();
            var templatePath = Path.Combine("Templates", "source", "Notice_CC_Violation_Template.docx");
            var outputName = $"CommunityControlTermsNotice_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
            try
            {
                var bytes = DocxTemplateProcessor.FillTemplate(templatePath, new Dictionary<string, string>
                {
                    ["case_number"] = dto.CaseNumber ?? "",
                    ["defendant.first_name"] = dto.DefendantFirstName ?? "",
                    ["defendant.last_name"] = dto.DefendantLastName ?? "",
                    ["entry_date"] = dto.EntryDate.ToString("MMMM dd, yyyy"),
                    ["slated_date"] = "",
                    ["violation_hearing_date"] = dto.HearingDate.ToString("MMMM dd, yyyy"),
                    ["violation_hearing_time"] = dto.HearingTime ?? "",
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

        app.MapPost("/communitycontrolterms", (Data.CommunityControlTermsDto dto) =>
        {
            if (dto == null) return Results.BadRequest();
            var templatePath = Path.Combine("Templates", "source", "Terms_Of_Community_Control_Template.docx");
            var outputName = $"CommunityControlTerms_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
            try
            {
                var bytes = DocxTemplateProcessor.FillTemplate(templatePath, new Dictionary<string, string>
                {
                    ["case_number"] = dto.CaseNumber ?? "",
                    ["defendant.first_name"] = dto.DefendantFirstName ?? "",
                    ["defendant.last_name"] = dto.DefendantLastName ?? "",
                    ["entry_date"] = dto.EntryDate?.ToString("MMMM dd, yyyy") ?? "",
                    ["term_of_control"] = dto.TermOfControl ?? "",
                    ["report_frequency"] = dto.ReportFrequency ?? "",
                    ["jail_days"] = dto.JailDaysToServe?.ToString() ?? "",
                    ["jail_report_date"] = dto.JailReportDate?.ToString("MMMM dd, yyyy") ?? "",
                    ["jail_report_time"] = dto.JailReportTime ?? "",
                    ["no_contact_with_person"] = dto.NoContactWith ?? "",
                    ["community_service_hours"] = dto.CommunityServiceHours?.ToString() ?? "",
                    ["scram_days"] = dto.ScramDays?.ToString() ?? "",
                    ["specialized_docket_type"] = dto.SpecializedDocketType ?? "",
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

        app.MapPost("/communityservicesecondary", (Data.CommunityServiceSecondaryDto dto) =>
        {
            if (dto == null) return Results.BadRequest();
            var templatePath = Path.Combine("Templates", "source", "Freeform_Entry_Template.docx");
            var outputName = $"CommunityServiceSecondary_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
            try
            {
                var bytes = DocxTemplateProcessor.FillTemplate(templatePath, new Dictionary<string, string>
                {
                    ["case_number"] = "",
                    ["defendant.first_name"] = "",
                    ["defendant.last_name"] = "",
                    ["entry_date"] = DateTime.UtcNow.ToString("MMMM dd, yyyy"),
                    ["defense_counsel"] = "",
                    ["defense_counsel_type"] = "",
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
