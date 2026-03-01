using Munientry.DocxTemplating;

namespace Munientry.Api.Endpoints;

/// <summary>
/// Endpoints for LEAP (Limiting Environmental Alcohol Penalties) program entries.
/// </summary>
internal static class LeapEndpoints
{
    internal static IEndpointRouteBuilder MapLeapEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/leapadmissionplea", (Data.LeapAdmissionPleaDto dto) =>
        {
            if (dto == null) return Results.BadRequest();
            var templatePath = Path.Combine("Templates", "source", "Leap_Admission_Plea_Template.docx");
            var outputName = $"LeapAdmissionPlea_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
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
                    ["charge.statute"] = dto.ChargeStatute ?? "",
                    ["charge.degree"] = dto.ChargeDegree ?? "",
                    ["charge.plea"] = dto.ChargePlea ?? "",
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

        app.MapPost("/leapadmissionalreadyvalid", (Data.LeapAdmissionAlreadyValidDto dto) =>
        {
            if (dto == null) return Results.BadRequest();
            var templatePath = Path.Combine("Templates", "source", "Leap_Admission_Plea_Valid_Template.docx");
            var outputName = $"LeapAdmissionAlreadyValid_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
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
                    ["plea_trial_date"] = dto.AdmissionDate?.ToString("MMMM dd, yyyy") ?? "",
                    ["charge.offense"] = dto.Charges ?? "",
                    ["charge.statute"] = dto.ChargeStatute ?? "",
                    ["charge.degree"] = dto.ChargeDegree ?? "",
                    ["charge.plea"] = dto.ChargePlea ?? "",
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

        app.MapPost("/leapvalidsentencing", (Data.LeapValidSentencingDto dto) =>
        {
            if (dto == null) return Results.BadRequest();
            var templatePath = Path.Combine("Templates", "source", "Leap_Admission_Plea_Valid_Template.docx");
            var outputName = $"LeapValidSentencing_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
            try
            {
                var bytes = DocxTemplateProcessor.FillTemplate(templatePath, new Dictionary<string, string>
                {
                    ["case_number"] = dto.CaseNumber ?? "",
                    ["defendant.first_name"] = dto.DefendantFirstName ?? "",
                    ["defendant.last_name"] = dto.DefendantLastName ?? "",
                    ["defense_counsel"] = dto.DefenseCounselName ?? "",
                    ["defense_counsel_type"] = dto.DefenseCounselType ?? "",
                    ["plea_trial_date"] = dto.Date?.ToString("MMMM dd, yyyy") ?? "",
                    ["charge.offense"] = dto.Charges ?? "",
                    ["charge.statute"] = dto.ChargeStatute ?? "",
                    ["charge.degree"] = dto.ChargeDegree ?? "",
                    ["charge.plea"] = dto.ChargePlea ?? "",
                    ["amend_offense_details.motion_disposition"] = "",
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

        app.MapPost("/leapsentencing", (Data.LeapSentencingDto dto) =>
        {
            if (dto == null) return Results.BadRequest();
            var templatePath = Path.Combine("Templates", "source", "Leap_Sentencing_Template.docx");
            var outputName = $"LeapSentencing_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
            try
            {
                var bytes = DocxTemplateProcessor.FillTemplate(templatePath, new Dictionary<string, string>
                {
                    ["case_number"] = dto.CaseNumber ?? "",
                    ["defendant.first_name"] = dto.DefendantFirstName ?? "",
                    ["defendant.last_name"] = dto.DefendantLastName ?? "",
                    ["defense_counsel"] = dto.DefenseCounselName ?? "",
                    ["defense_counsel_type"] = dto.DefenseCounselType ?? "",
                    ["court_costs.pay_today_amount"] = dto.PayToday ?? "",
                    ["court_costs.monthly_pay_amount"] = dto.MonthlyPay ?? "",
                    ["court_costs.ability_to_pay_time"] = dto.AbilityToPay ?? "",
                    ["leap_plea_date"] = dto.LeapPleaDate?.ToString("MMMM dd, yyyy") ?? "",
                    ["plea_trial_date"] = dto.PleaTrialDate?.ToString("MMMM dd, yyyy") ?? "",
                    ["charge.offense"] = dto.ChargeOffense ?? "",
                    ["charge.statute"] = dto.ChargeStatute ?? "",
                    ["charge.degree"] = dto.ChargeDegree ?? "",
                    ["charge.plea"] = dto.ChargePlea ?? "",
                    ["charge.finding"] = dto.ChargeFinding ?? "",
                    ["charge.fines_amount"] = dto.ChargeFinesAmount ?? "",
                    ["charge.fines_suspended"] = dto.ChargeFinesSuspended ?? "",
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
