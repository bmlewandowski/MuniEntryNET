using Microsoft.AspNetCore.Builder;
using Munientry.DocxTemplating;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<Munientry.Api.Services.DrivingPrivilegesService>();
builder.Services.AddScoped<Munientry.Api.Services.NoticesFreeformCivilService>();
builder.Services.AddScoped<Munientry.Api.Services.CommunityControlTermsNoticesService>();
builder.Services.AddOpenApi();
builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
builder.Services.AddScoped<Munientry.Poc.Api.Services.DrivingCaseService>();
builder.Services.AddScoped<Munientry.Poc.Api.Services.TrialToCourtNoticeService>();
builder.Services.AddScoped<Munientry.Poc.Api.Services.FinalJuryNoticeService>();
builder.Services.AddScoped<Munientry.Poc.Api.Services.BondHearingService>();
builder.Services.AddScoped<Munientry.Poc.Api.Services.ProbationViolationBondService>();
builder.Services.AddScoped<Munientry.Poc.Api.Services.TimeToPayOrderService>();
builder.Services.AddScoped<Munientry.Poc.Api.Services.JurorPaymentService>();
builder.Services.AddScoped<Munientry.Api.Services.CommunityControlTermsNoticesService>();
builder.Services.AddScoped<MuniEntry.Api.Services.FineOnlyPleaService>();
builder.Services.AddScoped<Munientry.Poc.Api.Services.ArraignmentContinuanceService>();
builder.Services.AddScoped<Munientry.Poc.Api.Services.DenyPrivilegesPermitRetestService>();
builder.Services.AddScoped<Munientry.Poc.Api.Services.BondModificationRevocationService>();
builder.Services.AddScoped<Munientry.Poc.Api.Services.CommunityServiceSecondaryService>();
builder.Services.AddScoped<Munientry.Poc.Api.Services.NotGuiltyPleaService>();
builder.Services.AddScoped<Munientry.Poc.Api.Services.NotGuiltyAppearBondSpecialService>();
builder.Services.AddScoped<Munientry.Poc.Api.Services.AppearOnWarrantNoPleaService>();
builder.Services.AddScoped<Munientry.Poc.Api.Services.LeapAdmissionPleaService>();
builder.Services.AddScoped<Munientry.Poc.Api.Services.LeapAdmissionAlreadyValidService>();
builder.Services.AddScoped<Munientry.Poc.Api.Services.SentencingOnlyAlreadyPleadService>();
builder.Services.AddScoped<Munientry.Poc.Api.Services.PleaOnlyFutureSentencingService>();
builder.Services.AddScoped<Munientry.Poc.Api.Services.DiversionDialogService>();
builder.Services.AddScoped<Munientry.Api.Services.CivilFreeformEntryService>();
builder.Services.AddScoped<Munientry.Poc.Api.Services.ICommunityControlTermsService, Munientry.Poc.Api.Services.CommunityControlTermsService>();
builder.Services.AddScoped<Munientry.Poc.Api.Services.ILeapSentencingService, Munientry.Poc.Api.Services.LeapSentencingService>();
builder.Services.AddScoped<Munientry.Api.Services.IFiscalJournalEntryService, Munientry.Api.Services.FiscalJournalEntryService>();
builder.Services.AddScoped<Munientry.Poc.Api.Services.JailCcPleaService>();
builder.Services.AddScoped<Munientry.Poc.Api.Services.LeapValidSentencingService>();
builder.Services.AddScoped<Munientry.Poc.Api.Services.SchedulingEntryService>();
builder.Services.AddScoped<Munientry.Poc.Api.Services.DiversionPleaService>();
builder.Services.AddScoped<Munientry.Poc.Api.Services.CaseSearchService>();
builder.Services.AddScoped<Munientry.Poc.Api.Services.DailyListService>();
builder.Services.AddScoped<Munientry.Poc.Api.Services.GeneralNoticeOfHearingService>();

var app = builder.Build();
app.UseCors();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // available at /openapi/v1.json
}

app.MapPost("/api/drivingprivileges", async (Munientry.Poc.Api.Data.DrivingPrivilegesDto dto, Munientry.Api.Services.DrivingPrivilegesService service) =>
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
            // Fields below not yet in DTO â€” populated as empty for POC
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
    catch (Exception ex)
    {
        return Results.Problem($"DOCX generation failed: {ex.Message}");
    }
});
app.MapPost("/api/noticesfreeformcivil", (Munientry.Poc.Api.Data.NoticesFreeformCivilDto dto) =>
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
    catch (Exception ex)
    {
        return Results.Problem($"DOCX generation failed: {ex.Message}");
    }
});
app.MapPost("/api/communitycontroltermsnotices", (Munientry.Poc.Api.Data.CommunityControlTermsNoticesDto dto) =>
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
    catch (Exception ex)
    {
        return Results.Problem($"DOCX generation failed: {ex.Message}");
    }
});

app.MapPost("/api/denyprivilegespermitretest", (Munientry.Poc.Api.Data.DenyPrivilegesPermitRetestDto dto) =>
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
    catch (Exception ex)
    {
        return Results.Problem($"DOCX generation failed: {ex.Message}");
    }
});

app.MapGet("/api/fineonly/{caseNumber}", (string caseNumber) =>
{
    return Results.Ok(new {
        CaseNumber = caseNumber,
        DefendantName = "Jane Doe",
        Charge = "Speeding",
        FineAmount = 150.00m
    });
});

app.MapPost("/api/fineonly", async (HttpRequest req) =>
{
    var dto = await req.ReadFromJsonAsync<Munientry.Poc.Api.Data.FineOnlyDto>();
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
    catch (Exception ex)
    {
        return Results.Problem($"DOCX generation failed: {ex.Message}");
    }
});

app.MapGet("/api/case/{id}", (string id) => Results.Ok(new
{
    caseNumber = id,
    defendant = "John Doe",
    charges = new[] { new { charge = "Speeding", statute = "S-101" } }
}));

app.MapGet("/api/diversion/{id}", (string id) => Results.Ok(new
{
    caseNumber = id,
    defendantFirstName = "Jane",
    defendantLastName = "Roe",
    diversionCompletionDate = DateTime.UtcNow.AddDays(90).ToString("yyyy-MM-dd"),
    diversionFinePayDate = DateTime.UtcNow.AddDays(120).ToString("yyyy-MM-dd"),
    charges = new[] { new { charge = "Speeding", statute = "S-101" }, new { charge = "No Insurance", statute = "I-202" } }
}));


app.MapPost("/api/jailccplea", (Munientry.Poc.Api.Data.JailCcPleaDto dto) =>
{
    if (dto == null) return Results.BadRequest();
    var templatePath = Path.Combine("Templates", "source", "Jail_Plea_Final_Judgment_Template.docx");
    var outputName = $"JailCcPlea_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
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
            ["plea_trial_date"] = dto.Date?.ToString("MMMM dd, yyyy") ?? "",
            ["charge.offense"] = dto.Charges ?? "",
            ["charge.statute"] = dto.ChargeStatute ?? "",
            ["charge.degree"] = dto.ChargeDegree ?? "",
            ["charge.plea"] = dto.ChargePlea ?? "",
            ["charge.finding"] = dto.ChargeFinding ?? "",
            ["charge.fines_amount"] = dto.ChargeFinesAmount ?? "",
            ["charge.fines_suspended"] = dto.ChargeFinesSuspended ?? "",
            ["charge.jail_days"] = dto.ChargeJailDays ?? "",
            ["charge.jail_days_suspended"] = dto.ChargeJailDaysSuspended ?? "",
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
        });
        return Results.File(bytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", outputName);
    }
    catch (Exception ex)
    {
        return Results.Problem($"DOCX generation failed: {ex.Message}");
    }
});
app.MapPost("/api/leapvalidsentencing", (Munientry.Poc.Api.Data.LeapValidSentencingDto dto) =>
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
    catch (Exception ex)
    {
        return Results.Problem($"DOCX generation failed: {ex.Message}");
    }
});
app.MapPost("/api/diversion", async (HttpRequest req) =>
{
    var body = await req.ReadFromJsonAsync<object>();
    return Results.Ok(body);
});

// Case Search endpoint â€” calls [reports].[DMCMuniEntryCaseSearch]
// Returns all charges + defendant info for pre-populating criminal dialogs
app.MapGet("/api/case/search/{caseNumber}", async (string caseNumber, Munientry.Poc.Api.Services.CaseSearchService service) =>
{
    var result = await service.SearchCaseAsync(caseNumber);
    return result.Count > 0 ? Results.Ok(result) : Results.NotFound();
});

// Daily case list endpoint â€” calls one of the 6 scheduled-hearing stored procedures
// listType: arraignments, slated, pleas, pcvh_fcvh, final_pretrial, trials_to_court
// date format: yyyy-MM-dd
app.MapGet("/api/dailylist/{listType}/{date}", async (string listType, string date, Munientry.Poc.Api.Services.DailyListService service) =>
{
    if (!DateTime.TryParse(date, out var reportDate))
        return Results.BadRequest("Invalid date format. Use yyyy-MM-dd.");
    try
    {
        var result = await service.GetDailyListAsync(listType, reportDate);
        return Results.Ok(result);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

app.MapGet("/api/drivingcase/{caseNumber}", async (string caseNumber, Munientry.Poc.Api.Services.DrivingCaseService service) =>
{
    var result = await service.GetDrivingCaseInfoAsync(caseNumber);
    return result is not null ? Results.Ok(result) : Results.NotFound();
});

app.MapPost("/api/trialtocourt", async (Munientry.Poc.Api.Data.TrialToCourtNoticeDto dto, Munientry.Poc.Api.Services.TrialToCourtNoticeService service) =>
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
    catch (Exception ex)
    {
        return Results.Problem($"DOCX generation failed: {ex.Message}");
    }
});

// Fiscal Journal Entry POST endpoint
app.MapPost("/api/fiscaljournalentry", async (Munientry.Poc.Api.Data.FiscalJournalEntryDto dto, Munientry.Api.Services.IFiscalJournalEntryService service) =>
{
    service.InsertFiscalJournalEntry(dto);
    return Results.Ok();
});

// Civil Freeform Entry  POST endpoint

// GET endpoint to fetch CivilFreeformEntry by case number
app.MapGet("/api/civilfreeformentry/{caseNumber}", (string caseNumber) =>
{
    // For demo: return mock data. Replace with DB fetch logic as needed.
    return Results.Ok(new Munientry.Poc.Api.Data.CivilFreeformEntryDto {
        EntryDate = DateTime.Today,
        Plaintiff = "Sample Plaintiff",
        Defendant = "Sample Defendant",
        CaseNumber = caseNumber,
        AppearanceReason = "Sample Reason",
        EntryContent = "Sample entry content."
    });
});

app.MapPost("/api/civilfreeformentry", (Munientry.Poc.Api.Data.CivilFreeformEntryDto dto) =>
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
    catch (Exception ex)
    {
        return Results.Problem($"DOCX generation failed: {ex.Message}");
    }
});

// Final Jury Notice POST endpoint
app.MapPost("/api/finaljury", (Munientry.Poc.Api.Data.FinalJuryNoticeDto dto) =>
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
    catch (Exception ex)
    {
        return Results.Problem($"DOCX generation failed: {ex.Message}");
    }
});

// Bond Hearing POST endpoint
app.MapPost("/api/bondhearing", (Munientry.Poc.Api.Data.BondHearingDto dto) =>
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
    catch (Exception ex)
    {
        return Results.Problem($"DOCX generation failed: {ex.Message}");
    }
});

// Bond Modification Revocation POST endpoint
app.MapPost("/api/bondmodificationrevocation", (Munientry.Poc.Api.Data.BondModificationRevocationDto dto) =>
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
    catch (Exception ex)
    {
        return Results.Problem($"DOCX generation failed: {ex.Message}");
    }
});

// Community Service Secondary POST endpoint
app.MapPost("/api/communityservicesecondary", (Munientry.Poc.Api.Data.CommunityServiceSecondaryDto dto) =>
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
    catch (Exception ex)
    {
        return Results.Problem($"DOCX generation failed: {ex.Message}");
    }
});

// Probation Violation Bond POST endpoint
app.MapPost("/api/probationviolationbond", (Munientry.Poc.Api.Data.ProbationViolationBondDto dto) =>
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
    catch (Exception ex)
    {
        return Results.Problem($"DOCX generation failed: {ex.Message}");
    }
});

// Time To Pay Order POST endpoint
app.MapPost("/api/timetopayorder", async (Munientry.Poc.Api.Data.TimeToPayOrderDto dto, Munientry.Poc.Api.Services.TimeToPayOrderService service) =>
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
    catch (Exception ex)
    {
        return Results.Problem($"DOCX generation failed: {ex.Message}");
    }
});

// Juror Payment POST endpoint
app.MapPost("/api/jurorpayment", (Munientry.Poc.Api.Data.JurorPaymentDto dto) =>
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
    catch (Exception ex)
    {
        return Results.Problem($"DOCX generation failed: {ex.Message}");
    }
});

// Appear on Warrant (No Plea) POST endpoint
app.MapPost("/api/appearonwarrantnoplea", (Munientry.Poc.Api.Data.AppearOnWarrantNoPleaDto dto) =>
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
    catch (Exception ex)
    {
        return Results.Problem($"DOCX generation failed: {ex.Message}");
    }
});

// LEAP Plea Admission Dialog POST endpoint
app.MapPost("/api/leapadmissionplea", (Munientry.Poc.Api.Data.LeapAdmissionPleaDto dto) =>
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
    catch (Exception ex)
    {
        return Results.Problem($"DOCX generation failed: {ex.Message}");
    }
});

// LEAP Admission - Already Valid Dialog POST endpoint
app.MapPost("/api/leapadmissionalreadyvalid", (Munientry.Poc.Api.Data.LeapAdmissionAlreadyValidDto dto) =>
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
    catch (Exception ex)
    {
        return Results.Problem($"DOCX generation failed: {ex.Message}");
    }
});

// Sentencing Only - Already Plead Dialog POST endpoint
app.MapPost("/api/sentencingonlyalreadypead", (Munientry.Poc.Api.Data.SentencingOnlyAlreadyPleadDto dto) =>
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
    catch (Exception ex)
    {
        return Results.Problem($"DOCX generation failed: {ex.Message}");
    }
});

// Plea Only - Future Sentencing Dialog POST endpoint
app.MapPost("/api/pleaonlyfuturesentencing", (Munientry.Poc.Api.Data.PleaOnlyFutureSentencingDto dto) =>
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
            ["plea_trial_date"] = dto.PleaDate?.ToString("MMMM dd, yyyy") ?? "",
            ["plea_trial_date"] = dto.SentencingDate?.ToString("MMMM dd, yyyy") ?? "",
            ["charge.offense"] = dto.Charges ?? "",
            ["court_costs.pay_today_amount"] = dto.CourtCosts ?? "",
            ["judicial_officer.first_name"] = "",
            ["judicial_officer.last_name"] = "",
            ["judicial_officer.officer_type"] = "",
        });
        return Results.File(bytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", outputName);
    }
    catch (Exception ex)
    {
        return Results.Problem($"DOCX generation failed: {ex.Message}");
    }
});

// Diversion Dialog POST endpoint
app.MapPost("/api/diversiondialog", (Munientry.Poc.Api.Data.DiversionDialogDto dto) =>
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
    catch (Exception ex)
    {
        return Results.Problem($"DOCX generation failed: {ex.Message}");
    }
});


// General Notice of Hearing POST endpoint
app.MapPost("/api/generalnoticeofhearing", (Munientry.Poc.Api.Data.GeneralNoticeOfHearingDto dto) =>
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
    catch (Exception ex)
    {
        return Results.Problem($"DOCX generation failed: {ex.Message}");
    }
});

// Community Control Terms POST endpoint
app.MapPost("/api/communitycontrolterms", (Munientry.Poc.Api.Data.CommunityControlTermsDto dto) =>
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
    catch (Exception ex)
    {
        return Results.Problem($"DOCX generation failed: {ex.Message}");
    }
});

// LEAP Sentencing POST endpoint
app.MapPost("/api/leapsentencing", (Munientry.Poc.Api.Data.LeapSentencingDto dto) =>
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
    catch (Exception ex)
    {
        return Results.Problem($"DOCX generation failed: {ex.Message}");
    }
});

// Not Guilty Plea / Appear on Warrant / Bond Modification Special Bond Conditions POST endpoint
app.MapPost("/api/notguiltyappearbondspecial", (Munientry.Poc.Api.Data.NotGuiltyAppearBondSpecialDto dto) =>
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
    catch (Exception ex)
    {
        return Results.Problem($"DOCX generation failed: {ex.Message}");
    }
});

// Fine Only Plea POST endpoint
app.MapPost("/api/fineonlyplea", (MuniEntry.Api.Data.FineOnlyPleaDto dto) =>
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
            ["court_costs.pay_today_amount"] = dto.CourtCosts ?? "",
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
    catch (Exception ex)
    {
        return Results.Problem($"DOCX generation failed: {ex.Message}");
    }
});

// Not Guilty Plea POST endpoint
app.MapPost("/api/notguiltyplea", (Munientry.Poc.Api.Data.NotGuiltyPleaDto dto) =>
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
    catch (Exception ex)
    {
        return Results.Problem($"DOCX generation failed: {ex.Message}");
    }
});

// Arraignment Continuance POST endpoint
app.MapPost("/api/arraignmentcontinuance", (Munientry.Poc.Api.Data.ArraignmentContinuanceDto dto) =>
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
    catch (Exception ex)
    {
        return Results.Problem($"DOCX generation failed: {ex.Message}");
    }
});

// Scheduling Entry POST endpoint
// Template is selected by JudicialOfficer: "Rohrer" â†’ Rohrer template (Tuesday trial),
// "Fowler" â†’ Fowler template (Thursday trial), "Hemmeter" â†’ Hemmeter template.
// Mirrors Python SchedulingEntryCreator.set_template() which keys on dialog_name.
app.MapPost("/api/schedulingentry", (Munientry.Poc.Api.Data.SchedulingEntryDto dto) =>
{
    if (dto == null) return Results.BadRequest();
    var templateFile = (dto.JudicialOfficer?.Trim().ToLower()) switch
    {
        "fowler"   => "Scheduling_Entry_Template_Fowler.docx",
        "hemmeter" => "Scheduling_Entry_Template_Hemmeter.docx",
        _          => "Scheduling_Entry_Template_Rohrer.docx"  // default / "rohrer"
    };
    var templatePath = Path.Combine("Templates", templateFile);
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
    catch (Exception ex)
    {
        return Results.Problem($"DOCX generation failed: {ex.Message}");
    }
});

// Diversion Plea POST endpoint
app.MapPost("/api/diversionplea", (Munientry.Poc.Api.Data.DiversionPleaDto dto) =>
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
    catch (Exception ex)
    {
        return Results.Problem($"DOCX generation failed: {ex.Message}");
    }
});

// Trial Sentencing POST endpoint
app.MapPost("/api/trialsentencing", (Munientry.Poc.Api.Data.FinalJuryNoticeDto dto) =>
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
    catch (Exception ex)
    {
        return Results.Problem($"DOCX generation failed: {ex.Message}");
    }
});

app.Run();

public partial class Program { }
