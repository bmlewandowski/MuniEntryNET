# High-Impact / Low-Effort Improvement Plan
### *Crystal Ball Edition — Pre-Launch Hardening*

---

## Scoring Key
- **Impact:** How much pain it removes for users or reduces future support burden
- **Effort:** Engineering time (1 = hours, 2 = 1–2 days, 3 = 3–5 days)

> **See also:** [Pseudo_User_Analysis.md](Pseudo_User_Analysis.md) — the user analysis this plan responds to | [SECURITY_ARCHITECTURE_REVIEW.md](SECURITY_ARCHITECTURE_REVIEW.md) — security/architecture blockers and resolved items

---

## Tier 1 — Do These This Week

### 1. Consistent DOCX Filename Convention
**Impact: 🔥🔥🔥 | Effort: 1**

The API already knows the case number, form type, and date at the moment it returns the file stream. This is a one-liner.

```csharp
var sanitizedCaseNumber = caseNumber.Replace("/", "-").Replace("\\", "-");
var formType = nameof(FineOnlyPleaDto).Replace("Dto", "");
var fileName = $"{sanitizedCaseNumber}_{formType}_{DateTime.Today:yyyy-MM-dd}.docx";

Response.Headers["Content-Disposition"] = $"attachment; filename=\"{fileName}\"";
return File(docxBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
```

Zero user training required. Fixes the `download (3).docx` problem permanently.

---

### 2. Submission Confirmation Toast / Success Banner
**Impact: 🔥🔥🔥 | Effort: 1**

Users have no feedback that the DOCX was generated. Add a success toast after every successful POST that includes the case number and a timestamp.

```razor
@if (_show)
{
    <div class="alert alert-success alert-dismissible position-fixed bottom-0 end-0 m-3 shadow"
         style="z-index:1055; min-width:320px;" role="alert">
        <strong>✅ Entry Generated</strong><br />
        <small>@_caseNumber — @_formType</small><br />
        <small class="text-muted">@_timestamp.ToString("h:mm tt")</small>
        <button type="button" class="btn-close" @onclick="Dismiss"></button>
    </div>
}

@code {
    private bool _show;
    private string _caseNumber = "";
    private string _formType = "";
    private DateTime _timestamp;

    public void Show(string caseNumber, string formType)
    {
        _caseNumber = caseNumber;
        _formType = formType;
        _timestamp = DateTime.Now;
        _show = true;
        StateHasChanged();
    }

    private void Dismiss() => _show = false;
}
```

Wire it into every form's `OnSuccess` callback. One shared component, used everywhere.

---

### 3. Judge Context in Nav Bar via User Preference Service
**Impact: 🔥🔥🔥 | Effort: 2**

The single biggest UX complaint. A lightweight `UserPreferenceService` registered as `Scoped` stores the active judge for the session. The nav bar shows it prominently. Every form reads from it as a default.

```csharp
// Services/UserPreferenceService.cs
public class UserPreferenceService
{
    public string ActiveJudgeCode { get; private set; } = string.Empty;
    public string ActiveJudgeName { get; private set; } = string.Empty;
    public event Action? OnChange;

    public void SetJudge(string code, string name)
    {
        ActiveJudgeCode = code;
        ActiveJudgeName = name;
        OnChange?.Invoke();
    }
}
```

```razor
@* Shared/NavMenu.razor — judge selector at bottom of nav *@
@inject UserPreferenceService Prefs

<div class="nav-item px-3 mt-auto border-top pt-2">
    <span class="text-muted small">Active Judge:</span>
    <select class="form-select form-select-sm mt-1"
            @onchange="e => Prefs.SetJudge(e.Value!.ToString()!, judgeNames[e.Value!.ToString()!])">
        <option value="">— Select Judge —</option>
        @foreach (var j in judges)
        {
            <option value="@j.Code" selected="@(Prefs.ActiveJudgeCode == j.Code)">@j.Name</option>
        }
    </select>
</div>
```

Register in `Program.cs` as `builder.Services.AddScoped<UserPreferenceService>()`. Every form that has a judge field pre-populates from `Prefs.ActiveJudgeCode` on `OnInitialized`.

---

### 4. Global 500 / API Unreachable Error Handling
**Impact: 🔥🔥 | Effort: 1**

Right now a dead API shows a blank spinner or a raw exception. Add a global error boundary and an `HttpClient` interceptor.

```razor
@* Shared/GlobalErrorBoundary.razor *@
@inherits ErrorBoundaryBase

@if (CurrentException is HttpRequestException)
{
    <div class="alert alert-danger m-4">
        <h5>⚠️ Unable to reach the MuniEntry server.</h5>
        <p>Please check your network connection or contact IT support.</p>
        <button class="btn btn-outline-danger btn-sm" @onclick="Recover">Try Again</button>
    </div>
}
else if (CurrentException is not null)
{
    <div class="alert alert-warning m-4">
        <h5>Something went wrong.</h5>
        <p><code>@CurrentException.Message</code></p>
        <button class="btn btn-outline-warning btn-sm" @onclick="Recover">Dismiss</button>
    </div>
}
else
{
    @ChildContent
}
```

Wrap `<Routes />` in `App.razor` with this. Done in 30 minutes.

---

### 5. Form-Level Required Field Validation Before POST
**Impact: 🔥🔥🔥 | Effort: 1**

Every form should use `<EditForm>` with `DataAnnotationsValidator` and `<ValidationSummary>`. If the DTOs don't already have `[Required]` attributes, add them now. This costs almost nothing and prevents the blank-field-in-DOCX problem permanently.

```csharp
public class FineOnlyPleaDto
{
    [Required(ErrorMessage = "Case number is required.")]
    public string CaseNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Defendant name is required.")]
    public string DefendantName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Judge must be selected.")]
    public string JudgeCode { get; set; } = string.Empty;
    // ...
}
```

---

## Tier 2 — Do These This Sprint

### 6. LocalStorage Form State Persistence
**Impact: 🔥🔥🔥 | Effort: 2**

Add `Blazored.LocalStorage` (standard NuGet for Blazor). Auto-save form state on every field change, restore on component init, clear on successful submission.

```csharp
// Example: Pages/FineOnlyPlea.razor.cs
@inject ILocalStorageService LocalStorage

private const string DraftKey = "draft_fineonlyplea";

protected override async Task OnInitializedAsync()
{
    var saved = await LocalStorage.GetItemAsync<FineOnlyPleaDto>(DraftKey);
    if (saved is not null)
    {
        _model = saved;
        _showResumeBanner = true;
    }
    if (string.IsNullOrEmpty(_model.JudgeCode))
        _model.JudgeCode = Prefs.ActiveJudgeCode;
}

private async Task OnFieldChanged()
    => await LocalStorage.SetItemAsync(DraftKey, _model);

private async Task OnSubmitSuccess()
    => await LocalStorage.RemoveItemAsync(DraftKey);
```

Add a dismissible "Resume your last entry?" banner component that shows when a draft is detected. Prevents the Notepad-as-backup behavior immediately.

---

### 7. Entry Log Table + "My Recent Entries" Panel
**Impact: 🔥🔥🔥 | Effort: 3**

This is the audit trail that eliminates the compliance risk. The schema is tiny and the API already has all the data it needs at the moment of DOCX generation.

> **Note:** `AuditMiddleware` ([SECURITY_ARCHITECTURE_REVIEW.md](SECURITY_ARCHITECTURE_REVIEW.md) item 20) already writes per-request structured logs (`TraceId`, `UserId`, `Method`, `Path`, `Status`, `ElapsedMs`) to stdout for K8s log aggregation — this is **not** the same thing. This item creates a persistent `[app].[EntryLog]` table written to the database and surfaced in the Blazor UI as a "My Recent Entries" panel. Both are needed; only the middleware piece is done.

```sql
-- db/migrations/001_CreateEntryLog.sql
CREATE TABLE [app].[EntryLog] (
    [Id]              INT IDENTITY(1,1) PRIMARY KEY,
    [UserId]          NVARCHAR(100) NOT NULL,        -- EntraID OID
    [UserDisplayName] NVARCHAR(200) NOT NULL,
    [CaseNumber]      NVARCHAR(50)  NOT NULL,
    [FormType]        NVARCHAR(100) NOT NULL,
    [JudgeCode]       NVARCHAR(20)  NOT NULL,
    [FileName]        NVARCHAR(260) NOT NULL,
    [TemplateVersion] NVARCHAR(20)  NOT NULL,
    [GeneratedAt]     DATETIME2     NOT NULL DEFAULT GETUTCDATE()
);

CREATE INDEX IX_EntryLog_UserId_GeneratedAt
    ON [app].[EntryLog] ([UserId], [GeneratedAt] DESC);

CREATE INDEX IX_EntryLog_CaseNumber
    ON [app].[EntryLog] ([CaseNumber]);
```

A lightweight `EntryLogService` writes one row per successful DOCX generation using the EntraID claims already available on `HttpContext.User`. The "My Recent Entries" panel in the nav sidebar pulls the last 10 rows for the current user. A supervisor-scoped view can query by date range or case number. No PII beyond what's already in Entra.

---

### 8. "Load from Daily List" Case Picker on Every Form
**Impact: 🔥🔥🔥 | Effort: 3**

The daily list data is already being fetched for `MainWindow`. Extract it into a shared `DailyListService` and add a reusable `<CasePicker>` component. Every form gets a **"Load Case"** button that opens a modal pre-filtered to today's list for the active judge.

```razor
@* Shared/CasePicker.razor *@
<button class="btn btn-outline-secondary btn-sm mb-2" @onclick="Open">
    📋 Load from Daily List
</button>

@if (_open)
{
    <div class="modal show d-block" tabindex="-1">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Select Case — @_judgeDisplay (@DateTime.Today.ToString("MM/dd/yyyy"))</h5>
                    <button class="btn-close" @onclick="Close"></button>
                </div>
                <div class="modal-body">
                    <input class="form-control mb-2" placeholder="Filter by name or case #..."
                           @bind="_filter" @bind:event="oninput" />
                    <table class="table table-hover table-sm">
                        <thead><tr><th>Case #</th><th>Defendant</th><th>Charges</th></tr></thead>
                        <tbody>
                        @foreach (var c in FilteredCases)
                        {
                            <tr style="cursor:pointer" @onclick="() => Select(c)">
                                <td>@c.CaseNumber</td>
                                <td>@c.DefendantName</td>
                                <td>@c.ChargeDescription</td>
                            </tr>
                        }
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    </div>
    <div class="modal-backdrop show"></div>
}
```

When a row is clicked it fires `OnCaseSelected(CaseSummaryDto)` and the parent form calls the existing case-load SP with that case number. This directly replicates the Python double-click behavior.

---

### ~~9. Standardize All API Error Responses with `ProblemDetails`~~ — ✅ RESOLVED

`GlobalExceptionHandler` (`api/Middleware/GlobalExceptionHandler.cs`) is already implemented and registered. It logs the full exception server-side and returns a sanitized RFC 7807 Problem Details response with only a fixed message and trace ID — no internal detail reaches the client. See [SECURITY_ARCHITECTURE_REVIEW.md](SECURITY_ARCHITECTURE_REVIEW.md) item 5.

---

### ~~10. Health Check Endpoint~~ — ✅ RESOLVED

`builder.Services.AddHealthChecks()` and `app.MapHealthChecks("/healthz")` are already registered in `api/Program.cs`. The K8s `readinessProbe` and `livenessProbe` in `k8s/api/deployment.yaml` already point to `/healthz`. See [SECURITY_ARCHITECTURE_REVIEW.md](SECURITY_ARCHITECTURE_REVIEW.md) item 17.

> **Remaining:** A SQL Server ping check (`AddSqlServer(...)`) should be added to the health check registration once the connection string is stable in the cluster.

---

## Tier 3 — Next Milestone

### 11. Template Version Tagging
**Impact: 🔥🔥 | Effort: 1**

Embed the template filename and a hash of its contents into the DOCX custom properties at generation time. This costs one extra method call and means every generated document is self-describing — critical for compliance re-prints months later.

```csharp
// api/DocxTemplateProcessor.cs
using (var doc = DocX.Load(stream))
{
    // ...existing fill logic...

    doc.AddCustomProperty(new CustomProperty("TemplateSource", templateFileName));
    doc.AddCustomProperty(new CustomProperty("GeneratedAt", DateTime.UtcNow.ToString("o")));
    doc.AddCustomProperty(new CustomProperty("GeneratedBy", userDisplayName));
    doc.Save();
}
```

---

### 12. Civil Stored Procedure
**Impact: 🔥🔥 | Effort: 2**

The inline SQL for civil forms is the last piece of technical debt that prevents the API from being fully SP-driven. Creating `[reports].[DMCMuniEntryCivilCaseSearch]` mirrors the exact pattern of `DMCMuniEntryCaseSearch` and closes the gap.

---

## Consolidated Priority Matrix

| # | Change | Impact | Effort | Tier |
|---|---|---|---|---|
| 1 | Consistent DOCX filename | 🔥🔥🔥 | 1 hr | 1 |
| 2 | Success toast after generation | 🔥🔥🔥 | 1 hr | 1 |
| 3 | Judge context in nav / `UserPreferenceService` | 🔥🔥🔥 | 1 day | 1 |
| 4 | Global error boundary + offline message | 🔥🔥 | 1 hr | 1 |
| 5 | `[Required]` on all DTOs + form validation | 🔥🔥🔥 | 2 hrs | 1 |
| 6 | LocalStorage form state draft persistence | 🔥🔥🔥 | 1 day | 2 |
| 7 | `EntryLog` table + Recent Entries panel | 🔥🔥🔥 | 2 days | 2 |
| 8 | `CasePicker` load-from-daily-list modal | 🔥🔥🔥 | 2 days | 2 |
| ~~9~~ | ~~`ProblemDetails` global API error handler~~ | — | — | ✅ RESOLVED — [SECURITY item 5](SECURITY_ARCHITECTURE_REVIEW.md) |
| ~~10~~ | ~~`/health` endpoint with DB checks~~ | — | — | ✅ RESOLVED — [SECURITY item 17](SECURITY_ARCHITECTURE_REVIEW.md) |
| 11 | Template version tagging in DOCX properties | 🔥🔥 | 1 hr | 3 |
| 12 | Civil SP `DMCMuniEntryCivilCaseSearch` | 🔥🔥 | 1 day | 3 |

> **Tier 1 alone is roughly 1.5 days of engineering work and eliminates the top 5 complaints from the user analysis.** Tier 2 brings it to a genuinely production-ready state. Tier 3 closes all known technical debt.

_Last updated: March 3, 2026_
