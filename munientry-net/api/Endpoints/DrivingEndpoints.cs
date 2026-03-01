using Munientry.DocxTemplating;

namespace Munientry.Api.Endpoints;

/// <summary>
/// Endpoints for driving-license related entries:
/// driving privileges grants and denial/permit-retest orders.
/// </summary>
internal static class DrivingEndpoints
{
    internal static IEndpointRouteBuilder MapDrivingEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/drivingprivileges", async (Data.DrivingPrivilegesDto dto, Services.IDrivingPrivilegesService service) =>
        {
            if (dto == null) return Results.BadRequest();
            var templatePath = Path.Combine("Templates", "source", "Driving_Privileges_Template.docx");
            var outputName = $"DrivingPrivileges_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
            try
            {
                var bytes = DocxTemplateProcessor.FillTemplate(templatePath, new Dictionary<string, string>
                {
                    ["case_number"] = dto.CaseNumber ?? "",
                    ["defendant.first_name"] = dto.DefendantFirstName ?? "",
                    ["defendant.last_name"] = dto.DefendantLastName ?? "",
                    // Fields below not yet in DTO — populated as empty for POC
                    ["defendant.license_number"] = "",
                    ["defendant.birth_date"] = "",
                    ["defendant.address"] = "",
                    ["defendant.city"] = "",
                    ["defendant.state"] = "",
                    ["defendant.zipcode"] = "",
                    ["suspension_type"] = "",
                    ["suspension_start_date"] = "",
                    ["suspension_end_date"] = "",
                    ["bmv_cases"] = "",
                    ["related_traffic_case_number"] = "",
                    ["employer.privileges_type"] = "",
                    ["employer.name"] = "",
                    ["employer.address"] = "",
                    ["employer.city"] = "",
                    ["employer.state"] = "",
                    ["employer.zipcode"] = "",
                    ["employer.driving_days"] = "",
                    ["employer.driving_hours"] = "",
                    ["employer.other_conditions"] = "",
                    ["additional_information_text"] = "",
                });
                // --- DB Save Logic (commented out for isolated DOCX test) ---
                // service.InsertDrivingPrivileges(dto);
                // --- End DB Save Logic ---
                return Results.File(bytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", outputName);
            }
            catch (Exception)
            {
                throw;
            }
        });

        app.MapPost("/denyprivilegespermitretest", (Data.DenyPrivilegesPermitRetestDto dto) =>
        {
            if (dto == null) return Results.BadRequest();
            var templatePath = Path.Combine("Templates", "source", "Deny_Privileges_Template.docx");
            var outputName = $"DenyPrivileges_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
            try
            {
                var bytes = DocxTemplateProcessor.FillTemplate(templatePath, new Dictionary<string, string>
                {
                    ["case_number"] = dto.CaseNumber ?? "",
                    ["defendant.first_name"] = dto.DefendantFirstName ?? "",
                    ["defendant.last_name"] = dto.DefendantLastName ?? "",
                    ["license_expiration_date"] = dto.LicenseExpirationDate?.ToString("MMMM dd, yyyy") ?? "",
                    ["privileges_grant_date"] = dto.PrivilegesGrantDate?.ToString("MMMM dd, yyyy") ?? "",
                    ["nufc_date"] = dto.NufcDate?.ToString("MMMM dd, yyyy") ?? "",
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
