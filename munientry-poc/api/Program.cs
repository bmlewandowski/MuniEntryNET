using Microsoft.AspNetCore.Builder;
using Xceed.Words.NET;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
builder.Services.AddScoped<Munientry.Poc.Api.Services.DrivingCaseService>();
builder.Services.AddScoped<Munientry.Poc.Api.Services.TrialToCourtNoticeService>();

var app = builder.Build();
app.UseCors();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/api/health", () => Results.Ok(new { status = "ok" }));

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
    var dto = await req.ReadFromJsonAsync<FineOnlyDto>();
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
        // --- DB Save Logic (commented out for now) ---
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

app.Run();

public class FineOnlyDto
{
    public string? CaseNumber { get; set; }
    public string? DefendantName { get; set; }
    public string? Charge { get; set; }
    public decimal? FineAmount { get; set; }
}
