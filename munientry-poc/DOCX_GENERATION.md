# DOCX Generation

Court entry forms that produce a downloadable `.docx` file follow a consistent pattern across the API and Blazor client. This document describes how it works and how to add generation to a new form.

---

## How It Works

1. The Blazor form POSTs its model to an API endpoint.
2. The API opens a `.docx` template from `api/Templates/`, replaces `{Placeholder}` tokens with values from the DTO, and streams the resulting file back as a download.
3. The browser receives the byte stream and saves it via a JavaScript interop call.

No intermediate files are written to disk. The template is processed entirely in memory.

---

## Repository Layout

```
api/
  Program.cs                  ← All endpoints + GenerateDocxBytes() helper
  Templates/
    *.docx                    ← Processed templates (shipped in Docker image)
    source/                   ← Original Jinja2 .docx files (not shipped)
    prepare_templates.py      ← Script: converts source/ → processed *.docx
client/
  Pages/
    .../*.razor               ← Blazor forms
  wwwroot/
    index.html                ← Registers downloadFile() JS interop helper
```

---

## Template Format

Processed templates use `{CamelCase}` placeholder tokens directly in the document text, e.g.:

```
Case No: {CaseNumber}
Defendant: {DefendantFirstName} {DefendantLastName}
Appearance Date: {AppearanceDate}
```

The source templates in `api/Templates/source/` use Jinja2-style `{{ snake_case }}` tokens (the original Python app format). The `prepare_templates.py` script converts them.

---

## Adding DOCX Generation to a New Form

### Step 1 — Map the template tokens

Open the source template from `api/Templates/source/` and identify every `{{ variable }}` token. Add a token map to `prepare_templates.py`:

```python
MY_FORM_TOKENS = {
    "case_number":          "CaseNumber",
    "defendant.first_name": "DefendantFirstName",
    "defendant.last_name":  "DefendantLastName",
    "entry_date":           "EntryDate",
    # ... add all tokens in the template
}
```

Add the template to the `TEMPLATES` list at the bottom of the maps section:

```python
TEMPLATES = [
    ...
    ("My_Form_Template.docx", MY_FORM_TOKENS),
]
```

Run the script from the repo root to produce the processed template:

```powershell
python api/Templates/prepare_templates.py
```

Verify the output file in `api/Templates/` contains `{CamelCase}` tokens (not `{{ }}`).

---

### Step 2 — Update the API endpoint

In `api/Program.cs`, replace the existing `Results.Ok()` return (or add a new endpoint) with a `GenerateDocxBytes` call:

```csharp
app.MapPost("/api/myform", async (MyFormDto dto, MyFormService service) =>
{
    if (dto == null) return Results.BadRequest();
    var templatePath = Path.Combine("Templates", "My_Form_Template.docx");
    var outputName = $"MyForm_{dto.CaseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
    try
    {
        var bytes = GenerateDocxBytes(templatePath, new Dictionary<string, string>
        {
            ["{CaseNumber}"]         = dto.CaseNumber ?? "",
            ["{DefendantFirstName}"] = dto.DefendantFirstName ?? "",
            ["{DefendantLastName}"]  = dto.DefendantLastName ?? "",
            ["{EntryDate}"]          = dto.EntryDate.ToString("MMMM dd, yyyy"),
        });
        return Results.File(bytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", outputName);
    }
    catch (Exception ex)
    {
        return Results.Problem($"DOCX generation failed: {ex.Message}");
    }
});
```

The `GenerateDocxBytes` helper is defined at the bottom of `Program.cs` and requires no changes.

---

### Step 3 — Update the Blazor form

Replace the `HandleValidSubmit` method with the download pattern and ensure `ApiHelper` and `IJSRuntime` are injected:

```razor
private async Task HandleValidSubmit()
{
    isSubmitting = true;
    errorMessage = null;
    try
    {
        var http = new HttpClient { BaseAddress = new Uri(ApiHelper.GetApiBaseUrl()) };
        var response = await http.PostAsJsonAsync("myform", formModel);
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsByteArrayAsync();
            var fileName = response.Content.Headers.ContentDisposition?.FileName?.Trim('"') ?? "MyForm.docx";
            await JS.InvokeVoidAsync("downloadFile", fileName,
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document", content);
        }
        else
        {
            errorMessage = $"Error: {response.ReasonPhrase}";
        }
    }
    catch (Exception ex)
    {
        errorMessage = ex.Message;
    }
    finally
    {
        isSubmitting = false;
    }
}

[Inject] NavigationManager NavigationManager { get; set; } = default!;
[Inject] IJSRuntime JS { get; set; } = default!;
[Inject] Munientry.Poc.Client.Shared.ApiHelper ApiHelper { get; set; } = default!;
```

The submit button should be:

```razor
<button type="submit" class="btn btn-success" disabled="@isSubmitting">Generate DOCX</button>
```

---

### Step 4 — Rebuild

```powershell
docker compose build
docker compose up -d
```

---

## How `GenerateDocxBytes` Works

```csharp
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
```

- Uses `DocumentFormat.OpenXml` (MIT licensed, no commercial restrictions).
- Iterates every `<w:t>` text element in the document body.
- Replacements are applied to the in-memory stream — no temp files.
- The processed `.docx` templates guarantee each `{Placeholder}` token lands in a single `<w:t>` element (Word splits raw text across runs; `prepare_templates.py` merges them).

---

## Why Templates Must Be Pre-Processed

Microsoft Word internally splits text across multiple XML `<w:r>` runs at arbitrary boundaries. A token like `{{ case_number }}` may be stored as three separate runs: `{{`, ` case_`, `number }}`. Simple string replacement on the raw XML would never find the complete token.

`prepare_templates.py` merges all runs within a paragraph into a single text node before replacing tokens, then writes the corrected XML back into the `.docx` ZIP. The resulting file has each `{Placeholder}` in one run, which `GenerateDocxBytes` can replace reliably at runtime.

---

## Currently Wired Forms

| Form | Route | Template | Endpoint |
|------|-------|----------|----------|
| Time to Pay Order | `/admin/time-to-pay-order` | `Time_To_Pay_Template.docx` | `POST /api/timetopayorder` |
| Trial to Court Notice | `/scheduling/trial-to-court-notice-of-hearing` | `Trial_To_Court_Hearing_Notice_Template.docx` | `POST /api/trialtocourt` |
| Driving Privileges | `/driving/driving-privileges` | `Driving_Privileges_Template.docx` | `POST /api/drivingprivileges` |
