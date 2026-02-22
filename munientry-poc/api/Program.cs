using Microsoft.AspNetCore.Builder;
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
app.MapPost("/api/noticesfreeformcivil", async (Munientry.Poc.Api.Data.NoticesFreeformCivilDto dto, Munientry.Api.Services.NoticesFreeformCivilService service) =>
{
    service.InsertNoticesFreeformCivil(dto);
    return Results.Ok();
});
app.MapPost("/api/communitycontroltermsnotices", async (Munientry.Poc.Api.Data.CommunityControlTermsNoticesDto dto, Munientry.Api.Services.CommunityControlTermsNoticesService service) =>
{
    service.InsertCommunityControlTermsNotices(dto);
    return Results.Ok();
});

app.MapPost("/api/denyprivilegespermitretest", async (Munientry.Poc.Api.Data.DenyPrivilegesPermitRetestDto dto, Munientry.Poc.Api.Services.DenyPrivilegesPermitRetestService service) =>
{
    if (dto == null) return Results.BadRequest();
    await service.SaveToDatabaseAsync(dto);
    return Results.Ok(new { status = "saved", dto });
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


app.MapPost("/api/jailccplea", async (Munientry.Poc.Api.Data.JailCcPleaDto dto, Munientry.Poc.Api.Services.JailCcPleaService service) =>
{
    await service.AddJailCcPleaAsync(dto);
    return Results.Ok();
});
app.MapPost("/api/leapvalidsentencing", async (Munientry.Poc.Api.Data.LeapValidSentencingDto dto, Munientry.Poc.Api.Services.LeapValidSentencingService service) =>
{
    await service.AddLeapValidSentencingAsync(dto);
    return Results.Ok();
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
app.MapPost("/api/civilfreeformentry", async (Munientry.Poc.Api.Data.CivilFreeformEntryDto dto, Munientry.Api.Services.CivilFreeformEntryService service) =>
{
    service.InsertCivilFreeformEntry(dto);
    return Results.Ok();
});

// Final Jury Notice POST endpoint
app.MapPost("/api/finaljury", async (Munientry.Poc.Api.Data.FinalJuryNoticeDto dto, Munientry.Poc.Api.Services.FinalJuryNoticeService service) =>
{
    if (dto == null) return Results.BadRequest();
    await service.SaveToDatabaseAsync(dto);
    return Results.Ok(new { status = "saved", dto });
});

// Bond Hearing POST endpoint
app.MapPost("/api/bondhearing", async (Munientry.Poc.Api.Data.BondHearingDto dto, Munientry.Poc.Api.Services.BondHearingService service) =>
{
    if (dto == null) return Results.BadRequest();
    await service.SaveToDatabaseAsync(dto);
    return Results.Ok(new { status = "saved", dto });
});

// Bond Modification Revocation POST endpoint
app.MapPost("/api/bondmodificationrevocation", async (Munientry.Poc.Api.Data.BondModificationRevocationDto dto, Munientry.Poc.Api.Services.BondModificationRevocationService service) =>
{
    if (dto == null) return Results.BadRequest();
    await service.SaveToDatabaseAsync(dto);
    return Results.Ok(new { status = "saved", dto });
});

// Community Service Secondary POST endpoint
app.MapPost("/api/communityservicesecondary", async (Munientry.Poc.Api.Data.CommunityServiceSecondaryDto dto, Munientry.Poc.Api.Services.CommunityServiceSecondaryService service) =>
{
    if (dto == null) return Results.BadRequest();
    await service.SaveToDatabaseAsync(dto);
    return Results.Ok(new { status = "saved", dto });
});

// Probation Violation Bond POST endpoint
app.MapPost("/api/probationviolationbond", async (Munientry.Poc.Api.Data.ProbationViolationBondDto dto, Munientry.Poc.Api.Services.ProbationViolationBondService service) =>
{
    if (dto == null) return Results.BadRequest();
    await service.SaveToDatabaseAsync(dto);
    return Results.Ok(new { status = "saved", dto });
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
app.MapPost("/api/jurorpayment", async (Munientry.Poc.Api.Data.JurorPaymentDto dto, Munientry.Poc.Api.Services.JurorPaymentService service) =>
{
    if (dto == null) return Results.BadRequest();
    await service.SaveToDatabaseAsync(dto);
    return Results.Ok(new { status = "saved", dto });
});

// Appear on Warrant (No Plea) POST endpoint
app.MapPost("/api/appearonwarrantnoplea", async (Munientry.Poc.Api.Data.AppearOnWarrantNoPleaDto dto, Munientry.Poc.Api.Services.AppearOnWarrantNoPleaService service) =>
{
    if (dto == null) return Results.BadRequest();
    await service.SaveToDatabaseAsync(dto);
    return Results.Ok(new { status = "saved", dto });
});

// LEAP Plea Admission Dialog POST endpoint
app.MapPost("/api/leapadmissionplea", async (Munientry.Poc.Api.Data.LeapAdmissionPleaDto dto, Munientry.Poc.Api.Services.LeapAdmissionPleaService service) =>
{
    if (dto == null) return Results.BadRequest();
    var entryId = await service.CreateLeapAdmissionPleaEntryAsync(dto);
    return Results.Ok(new { status = "saved", entryId, dto });
});

// LEAP Admission - Already Valid Dialog POST endpoint
app.MapPost("/api/leapadmissionalreadyvalid", (Munientry.Poc.Api.Data.LeapAdmissionAlreadyValidDto dto, Munientry.Poc.Api.Services.LeapAdmissionAlreadyValidService service) =>
{
    if (dto == null) return Results.BadRequest();
    service.Save(dto);
    return Results.Ok(new { status = "saved", dto });
});

// Sentencing Only - Already Plead Dialog POST endpoint
app.MapPost("/api/sentencingonlyalreadypead", (Munientry.Poc.Api.Data.SentencingOnlyAlreadyPleadDto dto, Munientry.Poc.Api.Services.SentencingOnlyAlreadyPleadService service) =>
{
    if (dto == null) return Results.BadRequest();
    service.Save(dto);
    return Results.Ok(new { status = "saved", dto });
});

// Plea Only - Future Sentencing Dialog POST endpoint
app.MapPost("/api/pleaonlyfuturesentencing", (Munientry.Poc.Api.Data.PleaOnlyFutureSentencingDto dto, Munientry.Poc.Api.Services.PleaOnlyFutureSentencingService service) =>
{
    if (dto == null) return Results.BadRequest();
    service.Save(dto);
    return Results.Ok(new { status = "saved", dto });
});

// Diversion Dialog POST endpoint
app.MapPost("/api/diversiondialog", (Munientry.Poc.Api.Data.DiversionDialogDto dto, Munientry.Poc.Api.Services.DiversionDialogService service) =>
{
    if (dto == null) return Results.BadRequest();
    service.Save(dto);
    return Results.Ok(new { status = "saved", dto });
});


// General Notice of Hearing POST endpoint
app.MapPost("/api/generalnoticeofhearing", async (Munientry.Poc.Api.Data.GeneralNoticeOfHearingDto dto, Munientry.Poc.Api.Services.GeneralNoticeOfHearingService service) =>
{
    if (dto == null) return Results.BadRequest();
    await service.SaveToDatabaseAsync(dto);
    return Results.Ok(new { status = "saved", dto });
});

// Community Control Terms POST endpoint
app.MapPost("/api/communitycontrolterms", async (Munientry.Poc.Api.Data.CommunityControlTermsDto dto, Munientry.Poc.Api.Services.ICommunityControlTermsService service) =>
{
    if (dto == null) return Results.BadRequest();
    var entryId = await service.CreateCommunityControlTermsEntryAsync(dto);
    return Results.Ok(new { status = "saved", entryId, dto });
});

// LEAP Sentencing POST endpoint
app.MapPost("/api/leapsentencing", async (Munientry.Poc.Api.Data.LeapSentencingDto dto, Munientry.Poc.Api.Services.ILeapSentencingService service) =>
{
    if (dto == null) return Results.BadRequest();
    var entryId = await service.CreateLeapSentencingEntryAsync(dto);
    return Results.Ok(new { status = "saved", entryId, dto });
});

// Not Guilty Plea / Appear on Warrant / Bond Modification Special Bond Conditions POST endpoint
app.MapPost("/api/notguiltyappearbondspecial", async (Munientry.Poc.Api.Data.NotGuiltyAppearBondSpecialDto dto, Munientry.Poc.Api.Services.NotGuiltyAppearBondSpecialService service) =>
{
    if (dto == null) return Results.BadRequest();
    await service.SaveToDatabaseAsync(dto);
    return Results.Ok(new { status = "saved", dto });
});

// Fine Only Plea POST endpoint
app.MapPost("/api/fineonlyplea", async (MuniEntry.Api.Data.FineOnlyPleaDto dto, MuniEntry.Api.Services.FineOnlyPleaService service) =>
{
    if (dto == null) return Results.BadRequest();
    service.CreateFineOnlyPleaEntry(dto);
    return Results.Ok(new { status = "saved", dto });
});

// Not Guilty Plea POST endpoint
app.MapPost("/api/notguiltyplea", async (Munientry.Poc.Api.Data.NotGuiltyPleaDto dto, Munientry.Poc.Api.Services.NotGuiltyPleaService service) =>
{
    if (dto == null) return Results.BadRequest();
    await service.SaveToDatabaseAsync(dto);
    return Results.Ok(new { status = "saved", dto });
});

// Arraignment Continuance POST endpoint
app.MapPost("/api/arraignmentcontinuance", async (Munientry.Poc.Api.Data.ArraignmentContinuanceDto dto, Munientry.Poc.Api.Services.ArraignmentContinuanceService service) =>
{
    if (dto == null) return Results.BadRequest();
    var result = await service.CreateArraignmentContinuanceAsync(dto);
    return Results.Ok(new { status = result ? "saved" : "error", dto });
});

// Scheduling Entry POST endpoint
app.MapPost("/api/schedulingentry", async (Munientry.Poc.Api.Data.SchedulingEntryDto dto, Munientry.Poc.Api.Services.SchedulingEntryService service) =>
{
    if (dto == null) return Results.BadRequest();
    var result = await service.CreateSchedulingEntryAsync(dto);
    return Results.Ok(new { status = result ? "saved" : "error", dto });
});

// Diversion Plea POST endpoint
app.MapPost("/api/diversionplea", async (Munientry.Poc.Api.Data.DiversionPleaDto dto, Munientry.Poc.Api.Services.DiversionPleaService service) =>
{
    if (dto == null) return Results.BadRequest();
    await service.AddDiversionPleaAsync(dto);
    return Results.Ok(new { status = "saved", dto });
});

app.Run();

// Shared DOCX generation helper — no third-party dependencies, uses BCL ZipArchive via Open XML SDK
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

