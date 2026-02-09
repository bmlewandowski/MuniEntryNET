using Microsoft.AspNetCore.Builder;
using Xceed.Words.NET;

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

var app = builder.Build();
app.MapPost("/api/drivingprivileges", async (Munientry.Poc.Api.Data.DrivingPrivilegesDto dto, Munientry.Api.Services.DrivingPrivilegesService service) =>
{
    service.InsertDrivingPrivileges(dto);
    return Results.Ok();
});
app.MapPost("/api/noticesfreeformcivil", async (Munientry.Poc.Api.Data.NoticesFreeformCivilDto dto, Munientry.Api.Services.NoticesFreeformCivilService service) =>
{
    service.InsertNoticesFreeformCivil(dto);
    return Results.Ok();
});
app.UseCors();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapPost("/api/communitycontroltermsnotices", async (Munientry.Poc.Api.Data.CommunityControlTermsNoticesDto dto, Munientry.Api.Services.CommunityControlTermsNoticesService service) =>
{
    service.InsertCommunityControlTermsNotices(dto);
app.MapPost("/api/denyprivilegespermitretest", async (Munientry.Poc.Api.Data.DenyPrivilegesPermitRetestDto dto, Munientry.Poc.Api.Services.DenyPrivilegesPermitRetestService service) =>
{
     if (dto == null) return Results.BadRequest();
     await service.SaveToDatabaseAsync(dto);
     return Results.Ok(new { status = "saved", dto });
});
    return Results.Ok();
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
    var outputPath = Path.Combine(Path.GetTempPath(), outputName);
    try
    {
        using var doc = Xceed.Words.NET.DocX.Load(templatePath);
        doc.ReplaceText("{CaseNumber}", dto.CaseNumber ?? "");
        doc.ReplaceText("{DefendantName}", dto.DefendantName ?? "");
        doc.ReplaceText("{Charge}", dto.Charge ?? "");
        doc.ReplaceText("{FineAmount}", dto.FineAmount?.ToString("F2") ?? "");
        doc.SaveAs(outputPath);
        var bytes = await File.ReadAllBytesAsync(outputPath);
        File.Delete(outputPath);
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
    var outputPath = Path.Combine(Path.GetTempPath(), outputName);
    try
    {
        using var doc = Xceed.Words.NET.DocX.Load(templatePath);
        // Use obsolete ReplaceText overload for compatibility
        doc.ReplaceText("{CaseNumber}", dto.CaseNumber ?? "");
        doc.ReplaceText("{DefendantFirstName}", dto.DefendantFirstName ?? "");
        doc.ReplaceText("{DefendantLastName}", dto.DefendantLastName ?? "");
        doc.ReplaceText("{EntryDate}", dto.EntryDate?.ToString("MMMM dd, yyyy") ?? "");
        doc.ReplaceText("{DefenseCounselName}", dto.DefenseCounselName ?? "");
        doc.ReplaceText("{TrialToCourtDate}", dto.TrialToCourtDate?.ToString("MMMM dd, yyyy") ?? "");
        doc.ReplaceText("{TrialToCourtTime}", dto.TrialToCourtTime ?? "");
        doc.ReplaceText("{AssignedCourtroom}", dto.AssignedCourtroom ?? "");
        doc.ReplaceText("{InterpreterRequired}", dto.InterpreterRequired ? "Yes" : "No");
        doc.ReplaceText("{LanguageRequired}", dto.LanguageRequired ?? "");
        doc.ReplaceText("{DateConfirmedWithCounsel}", dto.DateConfirmedWithCounsel ? "Yes" : "No");
        doc.SaveAs(outputPath);
        // --- DB Save Logic (commented out for isolated DOCX test) ---
        // await service.SaveToDatabaseAsync(dto);
        // --- End DB Save Logic ---
        var bytes = await File.ReadAllBytesAsync(outputPath);
        File.Delete(outputPath);
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
    await service.SaveToDatabaseAsync(dto);
    return Results.Ok(new { status = "saved", dto });
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

