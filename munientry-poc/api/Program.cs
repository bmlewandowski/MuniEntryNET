using Microsoft.AspNetCore.Builder;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<Munientry.Api.Services.DrivingPrivilegesService>();
builder.Services.AddScoped<Munientry.Api.Services.NoticesFreeformCivilService>();
builder.Services.AddScoped<Munientry.Api.Services.CommunityControlTermsNoticesService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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
builder.Services.AddScoped<Munientry.Api.Services.FiscalJournalEntryService>();
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
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/api/drivingprivileges", async (Munientry.Poc.Api.Data.DrivingPrivilegesDto dto, Munientry.Api.Services.DrivingPrivilegesService service) =>
{
    if (dto == null) return Results.BadRequest();
    var templatePath = Path.Combine("Templates", "Driving_Privileges_Template.docx");
    var outputName = $"DrivingPrivileges_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
    try
    {
        var bytes = GenerateDocxBytes(templatePath, new Dictionary<string, string>
        {
            ["{CaseNumber}"] = dto.CaseNumber ?? "",
            ["{DefendantFirstName}"] = dto.DefendantFirstName ?? "",
            ["{DefendantLastName}"] = dto.DefendantLastName ?? "",
            // Fields below not yet in DTO — populated as empty for POC
            ["{DefendantLicenseNumber}"] = "",
            ["{DefendantBirthDate}"] = "",
            ["{DefendantAddress}"] = "",
            ["{DefendantCity}"] = "",
            ["{DefendantState}"] = "",
            ["{DefendantZipcode}"] = "",
            ["{SuspensionType}"] = "",
            ["{SuspensionStartDate}"] = "",
            ["{SuspensionEndDate}"] = "",
            ["{BmvCases}"] = "",
            ["{RelatedTrafficCaseNumber}"] = "",
            ["{EmployerPrivilegesType}"] = "",
            ["{EmployerName}"] = "",
            ["{EmployerAddress}"] = "",
            ["{EmployerCity}"] = "",
            ["{EmployerState}"] = "",
            ["{EmployerZipcode}"] = "",
            ["{EmployerDrivingDays}"] = "",
            ["{EmployerDrivingHours}"] = "",
            ["{EmployerOtherConditions}"] = "",
            ["{AdditionalInformationText}"] = "",
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
    var templatePath = Path.Combine("Templates", "Notices_Freeform_Civil_Template.docx");
    var outputName = $"NoticesFreeformCivil_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
    try
    {
        var bytes = GenerateDocxBytes(templatePath, new Dictionary<string, string>
        {
            ["{CaseNumber}"] = dto.CaseNumber ?? "",
            ["{EntryDate}"] = dto.EntryDate.ToString("MMMM dd, yyyy"),
            ["{DefendantFirstName}"] = dto.DefendantFirstName ?? "",
            ["{DefendantLastName}"] = dto.DefendantLastName ?? "",
            ["{NoticeText}"] = dto.NoticeText ?? "",
            ["{PleaTrialDate}"] = "",
            ["{JudicialOfficerFirstName}"] = "",
            ["{JudicialOfficerLastName}"] = "",
            ["{JudicialOfficerType}"] = "",
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
    var templatePath = Path.Combine("Templates", "Notice_CC_Violation_Template.docx");
    var outputName = $"CommunityControlTermsNotice_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
    try
    {
        var bytes = GenerateDocxBytes(templatePath, new Dictionary<string, string>
        {
            ["{CaseNumber}"] = dto.CaseNumber ?? "",
            ["{DefendantFirstName}"] = dto.DefendantFirstName ?? "",
            ["{DefendantLastName}"] = dto.DefendantLastName ?? "",
            ["{EntryDate}"] = dto.EntryDate.ToString("MMMM dd, yyyy"),
            ["{SlatedDate}"] = "",
            ["{ViolationHearingDate}"] = dto.HearingDate.ToString("MMMM dd, yyyy"),
            ["{ViolationHearingTime}"] = dto.HearingTime ?? "",
            ["{JudicialOfficerFirstName}"] = "",
            ["{JudicialOfficerLastName}"] = "",
            ["{JudicialOfficerType}"] = "",
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
    var templatePath = Path.Combine("Templates", "Deny_Privileges_Template.docx");
    var outputName = $"DenyPrivileges_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
    try
    {
        var bytes = GenerateDocxBytes(templatePath, new Dictionary<string, string>
        {
            ["{CaseNumber}"] = dto.CaseNumber ?? "",
            ["{DefendantFirstName}"] = dto.DefendantFirstName ?? "",
            ["{DefendantLastName}"] = dto.DefendantLastName ?? "",
            ["{LicenseExpirationDate}"] = dto.LicenseExpirationDate?.ToString("MMMM dd, yyyy") ?? "",
            ["{PrivilegesGrantDate}"] = dto.PrivilegesGrantDate?.ToString("MMMM dd, yyyy") ?? "",
            ["{NufcDate}"] = dto.NufcDate?.ToString("MMMM dd, yyyy") ?? "",
            ["{JudicialOfficerFirstName}"] = "",
            ["{JudicialOfficerLastName}"] = "",
            ["{JudicialOfficerType}"] = "",
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
    var templatePath = Path.Combine("Templates", "Fine_Only_Plea_Final_Judgment_Template.docx");
    var outputName = $"FineOnly_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
    try
    {
        var bytes = GenerateDocxBytes(templatePath, new Dictionary<string, string>
        {
            ["{CaseNumber}"] = dto.CaseNumber ?? "",
            ["{DefendantName}"] = dto.DefendantName ?? "",
            ["{Charge}"] = dto.Charge ?? "",
            ["{FineAmount}"] = dto.FineAmount?.ToString("F2") ?? "",
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
    var templatePath = Path.Combine("Templates", "Jail_Plea_Final_Judgment_Template.docx");
    var outputName = $"JailCcPlea_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
    try
    {
        var bytes = GenerateDocxBytes(templatePath, new Dictionary<string, string>
        {
            ["{CaseNumber}"] = dto.CaseNumber ?? "",
            ["{DefendantFirstName}"] = dto.DefendantFirstName ?? "",
            ["{DefendantLastName}"] = dto.DefendantLastName ?? "",
            ["{DefenseCounsel}"] = dto.DefenseCounselName ?? "",
            ["{DefenseCounselType}"] = dto.DefenseCounselType ?? "",
            ["{AppearanceReason}"] = dto.AppearanceReason ?? "",
            ["{PleaTrialDate}"] = dto.Date?.ToString("MMMM dd, yyyy") ?? "",
            ["{ChargeDegree}"] = "",
            ["{ChargeFinding}"] = "",
            ["{ChargeFinesAmount}"] = "",
            ["{ChargeFinesSuspended}"] = "",
            ["{ChargeJailDays}"] = "",
            ["{ChargeJailDaysSuspended}"] = "",
            ["{ChargeOffense}"] = dto.Charges ?? "",
            ["{ChargePlea}"] = "",
            ["{ChargeStatute}"] = "",
            ["{AmendOffenseMotionDisposition}"] = "",
            ["{CourtCostsPayToday}"] = dto.PayToday?.ToString("F2") ?? "",
            ["{CourtCostsMonthlyPay}"] = dto.MonthlyPay?.ToString("F2") ?? "",
            ["{CourtCostsBalanceDueDate}"] = dto.DueDate?.ToString("MMMM dd, yyyy") ?? "",
            ["{CourtCostsAbilityToPayTime}"] = dto.TimeToPay ?? "",
            ["{FineJailDays}"] = "",
            ["{CommunityControlType}"] = "",
            ["{CommunityControlTerm}"] = "",
            ["{CommunityControlSpecializedDocket}"] = "",
            ["{HouseArrestTime}"] = "",
            ["{AlcoholMonitoringTime}"] = "",
            ["{GpsExclusionRadius}"] = "",
            ["{GpsExclusionLocation}"] = "",
            ["{CommunityServiceHours}"] = "",
            ["{NoContactWithPerson}"] = "",
            ["{NotWithin500FeetPerson}"] = "",
            ["{OtherCommunityControlConditions}"] = "",
            ["{PayRestitutionAmount}"] = "",
            ["{PayRestitutionTo}"] = "",
            ["{JailTermsDaysInJail}"] = "",
            ["{JailTermsTotalDaysToServe}"] = "",
            ["{JailTermsReportDate}"] = "",
            ["{JailTermsReportTime}"] = "",
            ["{JailTermsReportType}"] = "",
            ["{JailTermsReportDaysNotes}"] = "",
            ["{JailTermsCompanionCasesNumbers}"] = dto.CompanionCases ?? "",
            ["{JailTermsCompanionCasesSentenceType}"] = dto.CompanionCasesSentence ?? "",
            ["{LicenseSuspensionType}"] = "",
            ["{LicenseSuspensionTerm}"] = "",
            ["{LicenseSuspendedDate}"] = "",
            ["{OtherConditionsTerms}"] = "",
            ["{CommunityServiceHoursOfService}"] = "",
            ["{CommunityServiceDaysToComplete}"] = "",
            ["{CommunityServiceDueDate}"] = "",
            ["{ImpoundmentVehicleMakeModel}"] = "",
            ["{ImpoundmentVehicleLicensePlate}"] = "",
            ["{ImpoundmentAction}"] = "",
            ["{ImpoundmentTime}"] = "",
            ["{JudicialOfficerFirstName}"] = "",
            ["{JudicialOfficerLastName}"] = "",
            ["{JudicialOfficerType}"] = "",
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
    var templatePath = Path.Combine("Templates", "Leap_Admission_Plea_Valid_Template.docx");
    var outputName = $"LeapValidSentencing_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
    try
    {
        var bytes = GenerateDocxBytes(templatePath, new Dictionary<string, string>
        {
            ["{CaseNumber}"] = dto.CaseNumber ?? "",
            ["{DefendantFirstName}"] = dto.DefendantFirstName ?? "",
            ["{DefendantLastName}"] = dto.DefendantLastName ?? "",
            ["{DefenseCounsel}"] = dto.DefenseCounselName ?? "",
            ["{DefenseCounselType}"] = dto.DefenseCounselType ?? "",
            ["{PleaTrialDate}"] = dto.Date?.ToString("MMMM dd, yyyy") ?? "",
            ["{ChargeOffense}"] = dto.Charges ?? "",
            ["{ChargeDegree}"] = "",
            ["{ChargePlea}"] = "",
            ["{ChargeStatute}"] = "",
            ["{AmendOffenseMotionDisposition}"] = "",
            ["{JudicialOfficerFirstName}"] = "",
            ["{JudicialOfficerLastName}"] = "",
            ["{JudicialOfficerType}"] = "",
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

// Case Search endpoint — calls [reports].[DMCMuniEntryCaseSearch]
// Returns all charges + defendant info for pre-populating criminal dialogs
app.MapGet("/api/case/search/{caseNumber}", async (string caseNumber, Munientry.Poc.Api.Services.CaseSearchService service) =>
{
    var result = await service.SearchCaseAsync(caseNumber);
    return result.Count > 0 ? Results.Ok(result) : Results.NotFound();
});

// Daily case list endpoint — calls one of the 6 scheduled-hearing stored procedures
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
    var templatePath = Path.Combine("Templates", "Trial_To_Court_Hearing_Notice_Template.docx");
    var outputName = $"TrialToCourtNotice_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
    try
    {
        var bytes = GenerateDocxBytes(templatePath, new Dictionary<string, string>
        {
            ["{CaseNumber}"] = dto.CaseNumber ?? "",
            ["{DefendantFirstName}"] = dto.DefendantFirstName ?? "",
            ["{DefendantLastName}"] = dto.DefendantLastName ?? "",
            ["{EntryDate}"] = dto.EntryDate?.ToString("MMMM dd, yyyy") ?? "",
            ["{DefenseCounselName}"] = dto.DefenseCounselName ?? "",
            ["{TrialToCourtDate}"] = dto.TrialToCourtDate?.ToString("MMMM dd, yyyy") ?? "",
            ["{TrialToCourtTime}"] = dto.TrialToCourtTime ?? "",
            ["{AssignedCourtroom}"] = dto.AssignedCourtroom ?? "",
            ["{InterpreterRequired}"] = dto.InterpreterRequired ? "Yes" : "No",
            ["{LanguageRequired}"] = dto.LanguageRequired ?? "",
            ["{DateConfirmedWithCounsel}"] = dto.DateConfirmedWithCounsel ? "Yes" : "No",
            // TODO: populate from authenticated judge context
            ["{AssignedJudge}"] = "",
            ["{JudicialOfficerFirstName}"] = "",
            ["{JudicialOfficerLastName}"] = "",
            ["{JudicialOfficerType}"] = "",
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
app.MapPost("/api/fiscaljournalentry", async (Munientry.Poc.Api.Data.FiscalJournalEntryDto dto, Munientry.Api.Services.FiscalJournalEntryService service) =>
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
    var templatePath = Path.Combine("Templates", "Civil_Freeform_Entry_Template.docx");
    var outputName = $"CivilFreeformEntry_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
    try
    {
        var bytes = GenerateDocxBytes(templatePath, new Dictionary<string, string>
        {
            ["{CaseNumber}"] = dto.CaseNumber ?? "",
            ["{EntryDate}"] = dto.EntryDate.ToString("MMMM dd, yyyy"),
            ["{DefendantLastName}"] = dto.Defendant ?? "",
            ["{PlaintiffName}"] = dto.Plaintiff ?? "",
            ["{AppearanceReason}"] = dto.AppearanceReason ?? "",
            ["{DefenseCounsel}"] = "",
            ["{DefenseCounselType}"] = "",
            ["{EntryContentText}"] = dto.EntryContent ?? "",
            ["{JudicialOfficerFirstName}"] = "",
            ["{JudicialOfficerLastName}"] = "",
            ["{JudicialOfficerType}"] = "",
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
    var templatePath = Path.Combine("Templates", "Final_Jury_Notice_Of_Hearing_Template.docx");
    var outputName = $"FinalJuryNotice_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
    try
    {
        var bytes = GenerateDocxBytes(templatePath, new Dictionary<string, string>
        {
            ["{CaseNumber}"] = dto.CaseNumber ?? "",
            ["{DefendantFirstName}"] = dto.DefendantFirstName ?? "",
            ["{DefendantLastName}"] = dto.DefendantLastName ?? "",
            ["{AssignedJudge}"] = "",
            ["{FinalPretrialDate}"] = dto.FinalJuryDate?.ToString("MMMM dd, yyyy") ?? "",
            ["{FinalPretrialTime}"] = dto.FinalJuryTime ?? "",
            ["{JuryTrialDate}"] = "",
            ["{JuryTrialTime}"] = "",
            ["{JuryTrialLocation}"] = dto.AssignedCourtroom ?? "",
            ["{LanguageRequired}"] = dto.LanguageRequired ?? "",
            ["{JudicialOfficerFirstName}"] = "",
            ["{JudicialOfficerLastName}"] = "",
            ["{JudicialOfficerType}"] = "",
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
    var templatePath = Path.Combine("Templates", "Bond_Hearing_Template.docx");
    var outputName = $"BondHearing_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
    try
    {
        var bytes = GenerateDocxBytes(templatePath, new Dictionary<string, string>
        {
            ["{CaseNumber}"] = dto.CaseNumber ?? "",
            ["{DefendantFirstName}"] = dto.DefendantFirstName ?? "",
            ["{DefendantLastName}"] = dto.DefendantLastName ?? "",
            ["{DefenseCounsel}"] = dto.DefenseCounselName ?? "",
            ["{DefenseCounselType}"] = "",
            ["{AppearanceReason}"] = "",
            ["{PleaTrialDate}"] = dto.EntryDate?.ToString("MMMM dd, yyyy") ?? "",
            ["{BondAmount}"] = dto.BondAmount ?? "",
            ["{MonitoringType}"] = "",
            ["{SpecializedDocketType}"] = "",
            ["{CustodialSupervisionSupervisor}"] = "",
            ["{NoContactName}"] = "",
            ["{OtherConditionsTerms}"] = "",
            ["{ExclusivePossessionTo}"] = "",
            ["{ResidenceAddress}"] = "",
            ["{SurrenderWeaponsDate}"] = "",
            ["{VehicleMakeModel}"] = "",
            ["{VehicleLicensePlate}"] = "",
            ["{VehicleSeizureStateOpposes}"] = "",
            ["{DispositionMotionToReturn}"] = "",
            ["{AdminLicenseSuspensionExplanation}"] = "",
            ["{JudicialOfficerFirstName}"] = "",
            ["{JudicialOfficerLastName}"] = "",
            ["{JudicialOfficerType}"] = "",
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
    var templatePath = Path.Combine("Templates", "Bond_Hearing_Template.docx");
    var outputName = $"BondModificationRevocation_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
    try
    {
        var bytes = GenerateDocxBytes(templatePath, new Dictionary<string, string>
        {
            ["{CaseNumber}"] = dto.CaseNumber ?? "",
            ["{DefendantFirstName}"] = dto.DefendantFirstName ?? "",
            ["{DefendantLastName}"] = dto.DefendantLastName ?? "",
            ["{DefenseCounsel}"] = dto.DefenseCounselName ?? "",
            ["{DefenseCounselType}"] = "",
            ["{AppearanceReason}"] = dto.DecisionOnBond ?? "",
            ["{PleaTrialDate}"] = dto.EntryDate?.ToString("MMMM dd, yyyy") ?? "",
            ["{BondAmount}"] = dto.BondAmount ?? "",
            ["{MonitoringType}"] = "",
            ["{SpecializedDocketType}"] = "",
            ["{CustodialSupervisionSupervisor}"] = "",
            ["{NoContactName}"] = "",
            ["{OtherConditionsTerms}"] = "",
            ["{ExclusivePossessionTo}"] = "",
            ["{ResidenceAddress}"] = "",
            ["{SurrenderWeaponsDate}"] = "",
            ["{VehicleMakeModel}"] = "",
            ["{VehicleLicensePlate}"] = "",
            ["{VehicleSeizureStateOpposes}"] = "",
            ["{DispositionMotionToReturn}"] = "",
            ["{AdminLicenseSuspensionExplanation}"] = "",
            ["{JudicialOfficerFirstName}"] = "",
            ["{JudicialOfficerLastName}"] = "",
            ["{JudicialOfficerType}"] = "",
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
    var templatePath = Path.Combine("Templates", "Freeform_Entry_Template.docx");
    var outputName = $"CommunityServiceSecondary_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
    try
    {
        var bytes = GenerateDocxBytes(templatePath, new Dictionary<string, string>
        {
            ["{CaseNumber}"] = "",
            ["{DefendantFirstName}"] = "",
            ["{DefendantLastName}"] = "",
            ["{EntryDate}"] = DateTime.UtcNow.ToString("MMMM dd, yyyy"),
            ["{DefenseCounsel}"] = "",
            ["{DefenseCounselType}"] = "",
            ["{JudicialOfficerFirstName}"] = "",
            ["{JudicialOfficerLastName}"] = "",
            ["{JudicialOfficerType}"] = "",
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
    var templatePath = Path.Combine("Templates", "Probation_Violation_Bond_Template.docx");
    var outputName = $"ProbationViolationBond_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
    try
    {
        var bytes = GenerateDocxBytes(templatePath, new Dictionary<string, string>
        {
            ["{CaseNumber}"] = dto.CaseNumber ?? "",
            ["{DefendantFirstName}"] = dto.DefendantFirstName ?? "",
            ["{DefendantLastName}"] = dto.DefendantLastName ?? "",
            ["{DefenseCounsel}"] = dto.DefenseCounselName ?? "",
            ["{DefenseCounselType}"] = dto.DefenseCounselType ?? "",
            ["{AppearanceReason}"] = dto.AppearanceReason ?? "",
            ["{BondAmount}"] = dto.BondAmount ?? "",
            ["{MonitoringType}"] = dto.MonitoringType ?? "",
            ["{OtherConditionsTerms}"] = dto.OtherConditions ? "Yes" : "",
            ["{NoContactName}"] = "",
            ["{CustodialSupervisionSupervisor}"] = "",
            ["{SpecializedDocketType}"] = "",
            ["{ExclusivePossessionTo}"] = "",
            ["{ResidenceAddress}"] = "",
            ["{SurrenderWeaponsDate}"] = "",
            ["{VehicleMakeModel}"] = "",
            ["{VehicleLicensePlate}"] = "",
            ["{VehicleSeizureStateOpposes}"] = "",
            ["{DispositionMotionToReturn}"] = "",
            ["{AdminLicenseSuspensionExplanation}"] = "",
            ["{JudicialOfficerFirstName}"] = "",
            ["{JudicialOfficerLastName}"] = "",
            ["{JudicialOfficerType}"] = "",
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
    var templatePath = Path.Combine("Templates", "Time_To_Pay_Template.docx");
    var outputName = $"TimeToPay_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
    try
    {
        var bytes = GenerateDocxBytes(templatePath, new Dictionary<string, string>
        {
            ["{CaseNumber}"] = dto.CaseNumber ?? "",
            ["{EntryDate}"] = dto.EntryDate.ToString("MMMM dd, yyyy"),
            ["{DefendantFirstName}"] = dto.DefendantFirstName ?? "",
            ["{DefendantLastName}"] = dto.DefendantLastName ?? "",
            ["{AppearanceDate}"] = dto.AppearanceDate.ToString("MMMM dd, yyyy"),
            // TODO: populate from authenticated judge context
            ["{JudicialOfficerFirstName}"] = "",
            ["{JudicialOfficerLastName}"] = "",
            ["{JudicialOfficerType}"] = "",
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
    var templatePath = Path.Combine("Templates", "Jury_Payment_Template.docx");
    var outputName = $"JurorPayment_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
    try
    {
        var bytes = GenerateDocxBytes(templatePath, new Dictionary<string, string>
        {
            ["{CaseNumber}"] = dto.CaseNumber ?? "",
            ["{EntryDate}"] = dto.EntryDate.ToString("MMMM dd, yyyy"),
            ["{DefendantFirstName}"] = dto.DefendantFirstName ?? "",
            ["{DefendantLastName}"] = dto.DefendantLastName ?? "",
            ["{TrialDate}"] = dto.TrialDate.ToString("MMMM dd, yyyy"),
            ["{JurorsReported}"] = dto.JurorsReported.ToString(),
            ["{JurorsSeated}"] = dto.JurorsSeated.ToString(),
            ["{JurorsNotSeated}"] = dto.JurorsNotSeated.ToString(),
            ["{JurorsPayNotSeated}"] = dto.JurorsPayNotSeated.ToString(),
            ["{JurorsPaySeated}"] = dto.JurorsPaySeated.ToString(),
            ["{JuryPanelTotalPay}"] = dto.JuryPanelTotalPay.ToString(),
            ["{JudicialOfficerFirstName}"] = "",
            ["{JudicialOfficerLastName}"] = "",
            ["{JudicialOfficerType}"] = "",
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
    var templatePath = Path.Combine("Templates", "No_Plea_Bond_Template.docx");
    var outputName = $"AppearOnWarrantNoPlea_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
    try
    {
        var bytes = GenerateDocxBytes(templatePath, new Dictionary<string, string>
        {
            ["{CaseNumber}"] = dto.CaseNumber ?? "",
            ["{DefendantFirstName}"] = dto.DefendantFirstName ?? "",
            ["{DefendantLastName}"] = dto.DefendantLastName ?? "",
            ["{BondAmount}"] = dto.BondAmount ?? "",
            ["{NoContactName}"] = dto.NoContactName ?? "",
            ["{CustodialSupervisionSupervisor}"] = dto.CustodialSupervisionSupervisor ?? "",
            ["{AdminLicenseSuspensionExplanation}"] = dto.AdminLicenseSuspensionExplanation ?? "",
            ["{VehicleMakeModel}"] = dto.VehicleMakeModel ?? "",
            ["{VehicleLicensePlate}"] = dto.VehicleLicensePlate ?? "",
            ["{VehicleSeizureStateOpposes}"] = dto.StateOpposes ?? "",
            ["{DispositionMotionToReturn}"] = dto.DispositionMotionToReturn ?? "",
            ["{ResidenceAddress}"] = dto.ResidenceAddress ?? "",
            ["{ExclusivePossessionTo}"] = dto.ExclusivePossessionTo ?? "",
            ["{SurrenderWeaponsDate}"] = dto.SurrenderWeaponsDate ?? "",
            ["{JudicialOfficerFirstName}"] = "",
            ["{JudicialOfficerLastName}"] = "",
            ["{JudicialOfficerType}"] = "",
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
    var templatePath = Path.Combine("Templates", "Leap_Admission_Plea_Template.docx");
    var outputName = $"LeapAdmissionPlea_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
    try
    {
        var bytes = GenerateDocxBytes(templatePath, new Dictionary<string, string>
        {
            ["{CaseNumber}"] = dto.CaseNumber ?? "",
            ["{DefendantFirstName}"] = dto.DefendantFirstName ?? "",
            ["{DefendantLastName}"] = dto.DefendantLastName ?? "",
            ["{DefenseCounsel}"] = dto.DefenseCounselName ?? "",
            ["{DefenseCounselType}"] = dto.DefenseCounselType ?? "",
            ["{AppearanceReason}"] = dto.AppearanceReason ?? "",
            ["{PleaDate}"] = dto.PleaDate?.ToString("MMMM dd, yyyy") ?? "",
            ["{Charges}"] = dto.Charges ?? "",
            ["{CourtCosts}"] = dto.CourtCosts ?? "",
            ["{JudicialOfficerFirstName}"] = "",
            ["{JudicialOfficerLastName}"] = "",
            ["{JudicialOfficerType}"] = "",
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
    var templatePath = Path.Combine("Templates", "Leap_Admission_Plea_Valid_Template.docx");
    var outputName = $"LeapAdmissionAlreadyValid_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
    try
    {
        var bytes = GenerateDocxBytes(templatePath, new Dictionary<string, string>
        {
            ["{CaseNumber}"] = dto.CaseNumber ?? "",
            ["{DefendantFirstName}"] = dto.DefendantFirstName ?? "",
            ["{DefendantLastName}"] = dto.DefendantLastName ?? "",
            ["{DefenseCounsel}"] = dto.DefenseCounselName ?? "",
            ["{DefenseCounselType}"] = dto.DefenseCounselType ?? "",
            ["{AppearanceReason}"] = dto.AppearanceReason ?? "",
            ["{AdmissionDate}"] = dto.AdmissionDate?.ToString("MMMM dd, yyyy") ?? "",
            ["{Charges}"] = dto.Charges ?? "",
            ["{CourtCosts}"] = dto.CourtCosts ?? "",
            ["{JudicialOfficerFirstName}"] = "",
            ["{JudicialOfficerLastName}"] = "",
            ["{JudicialOfficerType}"] = "",
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
    var templatePath = Path.Combine("Templates", "Sentencing_Only_Template.docx");
    var outputName = $"SentencingOnly_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
    try
    {
        var bytes = GenerateDocxBytes(templatePath, new Dictionary<string, string>
        {
            ["{CaseNumber}"] = dto.CaseNumber ?? "",
            ["{DefendantFirstName}"] = dto.DefendantFirstName ?? "",
            ["{DefendantLastName}"] = dto.DefendantLastName ?? "",
            ["{DefenseCounsel}"] = dto.DefenseCounselName ?? "",
            ["{DefenseCounselType}"] = dto.DefenseCounselType ?? "",
            ["{AppearanceReason}"] = dto.AppearanceReason ?? "",
            ["{SentencingDate}"] = dto.SentencingDate?.ToString("MMMM dd, yyyy") ?? "",
            ["{Charges}"] = dto.Charges ?? "",
            ["{CourtCosts}"] = dto.CourtCosts ?? "",
            ["{JudicialOfficerFirstName}"] = "",
            ["{JudicialOfficerLastName}"] = "",
            ["{JudicialOfficerType}"] = "",
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
    var templatePath = Path.Combine("Templates", "Plea_Only_Template.docx");
    var outputName = $"PleaOnly_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
    try
    {
        var bytes = GenerateDocxBytes(templatePath, new Dictionary<string, string>
        {
            ["{CaseNumber}"] = dto.CaseNumber ?? "",
            ["{DefendantFirstName}"] = dto.DefendantFirstName ?? "",
            ["{DefendantLastName}"] = dto.DefendantLastName ?? "",
            ["{DefenseCounsel}"] = dto.DefenseCounselName ?? "",
            ["{DefenseCounselType}"] = dto.DefenseCounselType ?? "",
            ["{AppearanceReason}"] = dto.AppearanceReason ?? "",
            ["{PleaDate}"] = dto.PleaDate?.ToString("MMMM dd, yyyy") ?? "",
            ["{SentencingDate}"] = dto.SentencingDate?.ToString("MMMM dd, yyyy") ?? "",
            ["{Charges}"] = dto.Charges ?? "",
            ["{CourtCosts}"] = dto.CourtCosts ?? "",
            ["{JudicialOfficerFirstName}"] = "",
            ["{JudicialOfficerLastName}"] = "",
            ["{JudicialOfficerType}"] = "",
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
    var templatePath = Path.Combine("Templates", "Diversion_Template.docx");
    var outputName = $"DiversionDialog_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
    try
    {
        var bytes = GenerateDocxBytes(templatePath, new Dictionary<string, string>
        {
            ["{CaseNumber}"] = dto.CaseNumber ?? "",
            ["{DefendantFirstName}"] = dto.DefendantFirstName ?? "",
            ["{DefendantLastName}"] = dto.DefendantLastName ?? "",
            ["{DefenseCounsel}"] = dto.DefenseCounselName ?? "",
            ["{DefenseCounselType}"] = dto.DefenseCounselType ?? "",
            ["{AppearanceReason}"] = dto.AppearanceReason ?? "",
            ["{DiversionDate}"] = dto.DiversionDate?.ToString("MMMM dd, yyyy") ?? "",
            ["{Charges}"] = dto.Charges ?? "",
            ["{CourtCosts}"] = dto.CourtCosts ?? "",
            ["{JudicialOfficerFirstName}"] = "",
            ["{JudicialOfficerLastName}"] = "",
            ["{JudicialOfficerType}"] = "",
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
    var templatePath = Path.Combine("Templates", "General_Notice_Of_Hearing_Template.docx");
    var outputName = $"GeneralNoticeOfHearing_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
    try
    {
        var bytes = GenerateDocxBytes(templatePath, new Dictionary<string, string>
        {
            ["{CaseNumber}"] = dto.CaseNumber ?? "",
            ["{EntryDate}"] = dto.EntryDate.ToString("MMMM dd, yyyy"),
            ["{DefendantFirstName}"] = dto.DefendantFirstName ?? "",
            ["{DefendantLastName}"] = dto.DefendantLastName ?? "",
            ["{HearingDate}"] = dto.HearingDate.ToString("MMMM dd, yyyy"),
            ["{HearingTime}"] = dto.HearingTime ?? "",
            ["{AssignedCourtroom}"] = dto.AssignedCourtroom ?? "",
            ["{DefenseCounsel}"] = dto.DefenseCounselName ?? "",
            ["{LanguageRequired}"] = dto.LanguageRequired ?? "",
            ["{JudicialOfficerFirstName}"] = "",
            ["{JudicialOfficerLastName}"] = "",
            ["{JudicialOfficerType}"] = "",
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
    var templatePath = Path.Combine("Templates", "Terms_Of_Community_Control_Template.docx");
    var outputName = $"CommunityControlTerms_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
    try
    {
        var bytes = GenerateDocxBytes(templatePath, new Dictionary<string, string>
        {
            ["{JudicialOfficerFirstName}"] = "",
            ["{JudicialOfficerLastName}"] = "",
            ["{JudicialOfficerType}"] = "",
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
    var templatePath = Path.Combine("Templates", "Leap_Sentencing_Template.docx");
    var outputName = $"LeapSentencing_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
    try
    {
        var bytes = GenerateDocxBytes(templatePath, new Dictionary<string, string>
        {
            ["{CaseNumber}"] = dto.CaseNumber ?? "",
            ["{DefendantFirstName}"] = dto.DefendantFirstName ?? "",
            ["{DefendantLastName}"] = dto.DefendantLastName ?? "",
            ["{DefenseCounsel}"] = dto.DefenseCounselName ?? "",
            ["{DefenseCounselType}"] = dto.DefenseCounselType ?? "",
            ["{CourtCosts}"] = dto.CourtCosts ?? "",
            ["{AbilityToPay}"] = dto.AbilityToPay ?? "",
            ["{PayToday}"] = dto.PayToday ?? "",
            ["{MonthlyPay}"] = dto.MonthlyPay ?? "",
            ["{JailTimeCreditDays}"] = dto.JailTimeCreditDays ?? "",
            ["{FraInFile}"] = dto.FraInFile ?? "",
            ["{FraInCourt}"] = dto.FraInCourt ?? "",
            ["{JudicialOfficerFirstName}"] = "",
            ["{JudicialOfficerLastName}"] = "",
            ["{JudicialOfficerType}"] = "",
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
    var templatePath = Path.Combine("Templates", "Not_Guilty_Bond_Template.docx");
    var outputName = $"NotGuiltyAppearBondSpecial_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
    try
    {
        var bytes = GenerateDocxBytes(templatePath, new Dictionary<string, string>
        {
            ["{CaseNumber}"] = dto.CaseNumber ?? "",
            ["{DefendantFirstName}"] = dto.DefendantFirstName ?? "",
            ["{DefendantLastName}"] = dto.DefendantLastName ?? "",
            ["{BondAmount}"] = dto.BondAmount ?? "",
            ["{NoContactName}"] = dto.NoContactName ?? "",
            ["{CustodialSupervisionSupervisor}"] = dto.CustodialSupervisionSupervisor ?? "",
            ["{AdminLicenseSuspensionExplanation}"] = dto.AdminLicenseSuspensionExplanation ?? "",
            ["{VehicleMakeModel}"] = dto.VehicleMakeModel ?? "",
            ["{VehicleLicensePlate}"] = dto.VehicleLicensePlate ?? "",
            ["{VehicleSeizureStateOpposes}"] = dto.StateOpposes ?? "",
            ["{DispositionMotionToReturn}"] = dto.DispositionMotionToReturn ?? "",
            ["{ResidenceAddress}"] = dto.ResidenceAddress ?? "",
            ["{ExclusivePossessionTo}"] = dto.ExclusivePossessionTo ?? "",
            ["{SurrenderWeaponsDate}"] = dto.SurrenderWeaponsDate ?? "",
            ["{JudicialOfficerFirstName}"] = "",
            ["{JudicialOfficerLastName}"] = "",
            ["{JudicialOfficerType}"] = "",
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
    var templatePath = Path.Combine("Templates", "Fine_Only_Plea_Final_Judgment_Template.docx");
    var outputName = $"FineOnlyPlea_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
    try
    {
        var bytes = GenerateDocxBytes(templatePath, new Dictionary<string, string>
        {
            ["{CaseNumber}"] = dto.CaseNumber ?? "",
            ["{DefendantFirstName}"] = dto.DefendantFirstName ?? "",
            ["{DefendantLastName}"] = dto.DefendantLastName ?? "",
            ["{DefenseCounsel}"] = dto.DefenseCounselName ?? "",
            ["{DefenseCounselType}"] = dto.DefenseCounselType ?? "",
            ["{AppearanceReason}"] = dto.AppearanceReason ?? "",
            ["{Charges}"] = dto.Charges ?? "",
            ["{CourtCosts}"] = dto.CourtCosts ?? "",
            ["{TimeToPay}"] = dto.TimeToPay ?? "",
            ["{PayToday}"] = dto.PayToday?.ToString("F2") ?? "",
            ["{MonthlyPay}"] = dto.MonthlyPay?.ToString("F2") ?? "",
            ["{FraInFile}"] = dto.FraInFile ?? "",
            ["{FraInCourt}"] = dto.FraInCourt ?? "",
            ["{FineAmount}"] = dto.FineAmount?.ToString("F2") ?? "",
            ["{JudicialOfficerFirstName}"] = "",
            ["{JudicialOfficerLastName}"] = "",
            ["{JudicialOfficerType}"] = "",
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
    var templatePath = Path.Combine("Templates", "Not_Guilty_Bond_Template.docx");
    var outputName = $"NotGuiltyPlea_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
    try
    {
        var bytes = GenerateDocxBytes(templatePath, new Dictionary<string, string>
        {
            ["{CaseNumber}"] = dto.CaseNumber ?? "",
            ["{DefendantFirstName}"] = dto.DefendantFirstName ?? "",
            ["{DefendantLastName}"] = dto.DefendantLastName ?? "",
            ["{DefenseCounsel}"] = dto.DefenseCounselName ?? "",
            ["{DefenseCounselType}"] = dto.DefenseCounselType ?? "",
            ["{AppearanceReason}"] = dto.AppearanceReason ?? "",
            ["{PleaDate}"] = dto.PleaDate?.ToString("MMMM dd, yyyy") ?? "",
            ["{Charges}"] = dto.Charges ?? "",
            ["{BondAmount}"] = dto.BondAmount ?? "",
            ["{MonitoringType}"] = dto.MonitoringType ?? "",
            ["{SpecializedDocketType}"] = dto.SpecializedDocketType ?? "",
            ["{CourtCosts}"] = dto.CourtCosts ?? "",
            ["{JudicialOfficerFirstName}"] = "",
            ["{JudicialOfficerLastName}"] = "",
            ["{JudicialOfficerType}"] = "",
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
    var templatePath = Path.Combine("Templates", "Arraignment_Continue_Template.docx");
    var outputName = $"ArraignmentContinuance_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
    try
    {
        var bytes = GenerateDocxBytes(templatePath, new Dictionary<string, string>
        {
            ["{CaseNumber}"] = dto.CaseNumber ?? "",
            ["{DefendantFirstName}"] = dto.DefendantFirstName ?? "",
            ["{DefendantLastName}"] = dto.DefendantLastName ?? "",
            ["{DefenseCounsel}"] = dto.DefenseCounselName ?? "",
            ["{DefenseCounselType}"] = dto.DefenseCounselType ?? "",
            ["{AppearanceReason}"] = dto.AppearanceReason ?? "",
            ["{CurrentArraignmentDate}"] = dto.CurrentArraignmentDate?.ToString("MMMM dd, yyyy") ?? "",
            ["{NewArraignmentDate}"] = dto.NewArraignmentDate?.ToString("MMMM dd, yyyy") ?? "",
            ["{ContinuanceReason}"] = dto.ContinuanceReason ?? "",
            ["{JudicialOfficerFirstName}"] = "",
            ["{JudicialOfficerLastName}"] = "",
            ["{JudicialOfficerType}"] = "",
        });
        return Results.File(bytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", outputName);
    }
    catch (Exception ex)
    {
        return Results.Problem($"DOCX generation failed: {ex.Message}");
    }
});

// Scheduling Entry POST endpoint
app.MapPost("/api/schedulingentry", (Munientry.Poc.Api.Data.SchedulingEntryDto dto) =>
{
    if (dto == null) return Results.BadRequest();
    var templatePath = Path.Combine("Templates", "Scheduling_Entry_Template_Rohrer.docx");
    var outputName = $"SchedulingEntry_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
    try
    {
        var bytes = GenerateDocxBytes(templatePath, new Dictionary<string, string>
        {
            ["{CaseNumber}"] = dto.CaseNumber ?? "",
            ["{DefendantFirstName}"] = dto.DefendantFirstName ?? "",
            ["{DefendantLastName}"] = dto.DefendantLastName ?? "",
            ["{ArrestSummonsDate}"] = dto.ArrestSummonsDate?.ToString("MMMM dd, yyyy") ?? "",
            ["{DefenseCounsel}"] = dto.DefenseCounsel ?? "",
            ["{HighestCharge}"] = dto.HighestCharge ?? "",
            ["{DaysInJail}"] = (dto.DaysInJail ?? 0).ToString(),
            ["{ContinuanceDays}"] = (dto.ContinuanceDays ?? 0).ToString(),
            ["{PretrialDate}"] = dto.PretrialDate?.ToString("MMMM dd, yyyy") ?? "",
            ["{FinalPretrialDate}"] = dto.FinalPretrialDate?.ToString("MMMM dd, yyyy") ?? "",
            ["{FinalPretrialTime}"] = dto.FinalPretrialTime ?? "",
            ["{JuryTrialDate}"] = dto.JuryTrialDate?.ToString("MMMM dd, yyyy") ?? "",
            ["{LanguageRequired}"] = dto.LanguageRequired ?? "",
            ["{JudicialOfficerFirstName}"] = "",
            ["{JudicialOfficerLastName}"] = "",
            ["{JudicialOfficerType}"] = "",
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
    var templatePath = Path.Combine("Templates", "Diversion_Template.docx");
    var outputName = $"DiversionPlea_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
    try
    {
        var bytes = GenerateDocxBytes(templatePath, new Dictionary<string, string>
        {
            ["{CaseNumber}"] = dto.CaseNumber ?? "",
            ["{DefendantFirstName}"] = dto.DefendantFirstName ?? "",
            ["{DefendantLastName}"] = dto.DefendantLastName ?? "",
            ["{DefenseCounsel}"] = dto.DefenseCounselName ?? "",
            ["{DefenseCounselType}"] = dto.DefenseCounselType ?? "",
            ["{AppearanceReason}"] = dto.AppearanceReason ?? "",
            ["{Charges}"] = dto.Charges ?? "",
            ["{DiversionType}"] = dto.DiversionType ?? "",
            ["{DiversionCompletionDate}"] = dto.DiversionCompletionDate?.ToString("MMMM dd, yyyy") ?? "",
            ["{DiversionFinePayDate}"] = dto.DiversionFinePayDate?.ToString("MMMM dd, yyyy") ?? "",
            ["{PayRestitutionTo}"] = dto.PayRestitutionTo ?? "",
            ["{PayRestitutionAmount}"] = dto.PayRestitutionAmount?.ToString("F2") ?? "",
            ["{OtherConditionsText}"] = dto.OtherConditionsText ?? "",
            ["{JudicialOfficerFirstName}"] = "",
            ["{JudicialOfficerLastName}"] = "",
            ["{JudicialOfficerType}"] = "",
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
    var templatePath = Path.Combine("Templates", "Trial_Sentencing_Template.docx");
    var outputName = $"TrialSentencing_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
    try
    {
        var bytes = GenerateDocxBytes(templatePath, new Dictionary<string, string>
        {
            ["{CaseNumber}"] = dto.CaseNumber ?? "",
            ["{EntryDate}"] = dto.EntryDate?.ToString("MMMM dd, yyyy") ?? "",
            ["{DefendantFirstName}"] = dto.DefendantFirstName ?? "",
            ["{DefendantLastName}"] = dto.DefendantLastName ?? "",
            ["{DefenseCounsel}"] = dto.DefenseCounselName ?? "",
            ["{ArrestSummonsDate}"] = dto.ArrestSummonsDate?.ToString("MMMM dd, yyyy") ?? "",
            ["{HighestCharge}"] = dto.HighestCharge ?? "",
            ["{DaysInJail}"] = (dto.DaysInJail ?? 0).ToString(),
            ["{ContinuanceDays}"] = (dto.ContinuanceDays ?? 0).ToString(),
            ["{TrialToCourtDate}"] = dto.TrialToCourtDate?.ToString("MMMM dd, yyyy") ?? "",
            ["{TrialToCourtTime}"] = dto.TrialToCourtTime ?? "",
            ["{AssignedCourtroom}"] = dto.AssignedCourtroom ?? "",
            ["{LanguageRequired}"] = dto.LanguageRequired ?? "",
            ["{JudicialOfficerFirstName}"] = "",
            ["{JudicialOfficerLastName}"] = "",
            ["{JudicialOfficerType}"] = "",
        });
        return Results.File(bytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", outputName);
    }
    catch (Exception ex)
    {
        return Results.Problem($"DOCX generation failed: {ex.Message}");
    }
});

app.Run();

// Shared DOCX generation helper — no third-party dependencies, uses BCL ZipArchive via Open XML SDK
static byte[] CreateDocxFromContent(string title, params (string Label, string Value)[] fields)
{
    using var ms = new MemoryStream();
    using (var doc = WordprocessingDocument.Create(ms, DocumentFormat.OpenXml.WordprocessingDocumentType.Document))
    {
        var mainPart = doc.AddMainDocumentPart();
        var body = new Body();
        body.AppendChild(new Paragraph(
            new Run(new RunProperties(new Bold()), new Text(title) { Space = SpaceProcessingModeValues.Preserve })
        ));
        body.AppendChild(new Paragraph());
        foreach (var (label, value) in fields)
            body.AppendChild(new Paragraph(
                new Run(new RunProperties(new Bold()), new Text($"{label}: ") { Space = SpaceProcessingModeValues.Preserve }),
                new Run(new Text(value) { Space = SpaceProcessingModeValues.Preserve })
            ));
        mainPart.Document = new Document(body);
        mainPart.Document.Save();
    }
    return ms.ToArray();
}

static byte[] GenerateDocxBytes(string templatePath, Dictionary<string, string> replacements)
{
    var ms = new MemoryStream();
    using (var fs = File.OpenRead(templatePath))
        fs.CopyTo(ms);
    using (var wordDoc = WordprocessingDocument.Open(ms, isEditable: true))
    {
        var body = wordDoc.MainDocumentPart!.Document.Body!;
        foreach (var textEl in body.Descendants<Text>())
            foreach (var (placeholder, value) in replacements)
                textEl.Text = textEl.Text.Replace(placeholder, value);
        wordDoc.MainDocumentPart.Document.Save();
    }
    return ms.ToArray();
}

