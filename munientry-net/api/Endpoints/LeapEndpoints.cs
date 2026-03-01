using Munientry.Api;

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
                var chargesList = BuildChargesList(
                    dto.ChargeItems, dto.Charges, dto.ChargeStatute, dto.ChargeDegree, dto.ChargePlea);
                var bytes = DocxTemplateProcessor.FillTemplate(templatePath, new Dictionary<string, object>
                {
                    ["case_number"] = dto.CaseNumber ?? "",
                    ["defendant.first_name"] = dto.DefendantFirstName ?? "",
                    ["defendant.last_name"] = dto.DefendantLastName ?? "",
                    ["defense_counsel"] = dto.DefenseCounselName ?? "",
                    ["defense_counsel_type"] = dto.DefenseCounselType ?? "",
                    ["appearance_reason"] = dto.AppearanceReason ?? "",
                    ["plea_trial_date"] = dto.AdmissionDate?.ToString("MMMM dd, yyyy") ?? "",
                    ["charges_list"] = chargesList,
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
                var bytes = DocxTemplateProcessor.FillTemplate(templatePath, new Dictionary<string, object>
                {
                    ["case_number"] = dto.CaseNumber ?? "",
                    ["defendant.first_name"] = dto.DefendantFirstName ?? "",
                    ["defendant.last_name"] = dto.DefendantLastName ?? "",
                    ["defense_counsel"] = dto.DefenseCounselName ?? "",
                    ["defense_counsel_type"] = dto.DefenseCounselType ?? "",
                    ["plea_trial_date"] = dto.Date?.ToString("MMMM dd, yyyy") ?? "",
                    ["charges_list"] = BuildChargesList(
                        dto.ChargeItems, dto.Charges, dto.ChargeStatute, dto.ChargeDegree, dto.ChargePlea),
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
                var bytes = DocxTemplateProcessor.FillTemplate(templatePath, new Dictionary<string, object>
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
                    ["judicial_officer.first_name"] = "",
                    ["judicial_officer.last_name"] = "",
                    ["judicial_officer.officer_type"] = "",
                    ["charges_list"] = BuildSentencingChargesList(
                        dto.ChargeItems, dto.ChargeOffense, dto.ChargeStatute, dto.ChargeDegree,
                        dto.ChargePlea, dto.ChargeFinding, dto.ChargeFinesAmount, dto.ChargeFinesSuspended),
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
    /// Builds the list passed as <c>charges_list</c> for table-row looping.
    /// If <paramref name="items"/> is non-empty those are used; otherwise a single
    /// row is built from the legacy flat fields for backward compatibility.
    /// </summary>
    private static List<Dictionary<string, string>> BuildChargesList(
        List<Data.ChargeItemDto> items,
        string? offense, string? statute, string? degree, string? plea)
    {
        if (items.Count > 0)
            return items
                .Select(c => new Dictionary<string, string>
                {
                    ["offense"] = c.Offense,
                    ["statute"] = c.Statute,
                    ["degree"]  = c.Degree,
                    ["plea"]    = c.Plea,
                })
                .ToList();

        // Legacy single-charge fallback for old clients that only populate flat fields
        return new()
        {
            new() { ["offense"] = offense ?? "", ["statute"] = statute ?? "",
                    ["degree"]  = degree  ?? "", ["plea"]    = plea    ?? "" },
        };
    }

    /// <summary>
    /// Builds the <c>charges_list</c> for LEAP sentencing — includes finding and fines.
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
