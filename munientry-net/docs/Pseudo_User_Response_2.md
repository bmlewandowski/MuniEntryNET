# Engineering Response to Pseudo_User_Analysis_2
### *Pre-Launch Hardening — Crystal Ball Edition, Round 2*

> **See also:** [Pseudo_User_Analysis_2.md](Pseudo_User_Analysis_2.md) — the user analysis this responds to | [Pseudo_User_Response.md](Pseudo_User_Response.md) — original engineering response | [SECURITY_ARCHITECTURE_REVIEW.md](SECURITY_ARCHITECTURE_REVIEW.md) — security/architecture blockers | [Testing.md](Testing.md) — test suite guide

---

## Problems Identified & Recommended Solutions

---

### Problem 1 — Judge/Docket Context Lost at Session Start

**User Pain:** Defaulting to wrong judge on day one. Re-selecting judge on every form. Wrong-tab submissions.

**Root Cause:** No persistent session-scoped concept of "who am I working for today."

**Status: ✅ Implemented (March 10, 2026)**

Three complementary fixes were applied:

| Layer | File | What changed |
|---|---|---|
| Client guard | `client/Shared/FormPageBase.cs` | `HandleValidSubmit` returns early with a clear error if `OfficerSession.IsInitialized` is false. No form can POST without a resolved officer. |
| UI error state | `client/Shared/AppInitializer.razor` | Added `_initFailed` state. When `GetCurrentOfficerAsync()` returns null, a blocking "Account Not Recognised" modal is shown instead of an infinite `Loading…` screen. |
| Server-side defence | `shared/Validation/FormValidators.cs` | `Rules.AddJudicialOfficerRule` added — enforces `NotEmpty` on `JudicialOfficerFirstName` / `JudicialOfficerLastName` across all 10 split-field validators. `SchedulingEntryValidator` gains `NotEmpty` for its single-field `JudicialOfficer`. API returns 422 even if the client guard is bypassed (e.g. direct API call). |
| Tests | `api.Tests/ValidationFailureTests.cs` | 5 new tests. All happy-path DTO factories updated with JO fields. Suite: **163 tests, 0 failures**. |

The `JudicialOfficerSession` + `AppInitializer` + nav-bar officer badge were already in place before this round. This change adds the submission-time guard and the unresolvable-user error state that complete the feature.

---

### Problem 2 — DOCX File Naming and Save Location

**User Pain:** Files land in Downloads as `download (1).docx`. Manual rename, drag to network share, verify — 4 extra steps × 60 times/day.

**Status: 🟠 Planned**

**Solution A: Force Content-Disposition Header on API Response**

```csharp
// api/Endpoints/<form>Endpoint.cs
var fileName = $"{entry.CaseNumber}_{entry.EntryType}_{DateTime.Now:yyyyMMdd}.docx";
Response.Headers["Content-Disposition"] = $"attachment; filename=\"{fileName}\"";
```

This alone gets users `24CRB1234_FineOnlyPlea_20260310.docx` in Downloads immediately.

**Solution B: Server-Side Save to Network Share (Preferred)**

```csharp
[HttpPost("generate")]
public async Task<IActionResult> GenerateEntry([FromBody] EntryRequest request)
{
    var docxBytes = await _docxService.GenerateAsync(request);
    var fileName = BuildFileName(request);
    var savePath = _pathResolver.ResolveNetworkPath(request.EntryType, request.Judge);

    // Save to network share server-side
    await _fileService.SaveToNetworkShareAsync(savePath, fileName, docxBytes);

    // Log the generation event
    await _auditService.LogEntryGeneratedAsync(request, fileName, savePath);

    // Still stream back to browser for immediate preview/print
    return File(docxBytes,
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        fileName);
}
```

> **Key insight:** The server is already on the domain. It can write to `M:\Entries\` directly. The browser download becomes a convenience copy, not the only copy.

---

### Problem 3 — No Audit Trail / Generation Log

**User Pain:** Supervisor asks "show me all entries generated Tuesday" — answer is no.

**Status: 🟠 Planned**

**Solution: EntryGenerationLog Table + Supervisor View**

```csharp
public class EntryGenerationLog
{
    public int Id { get; set; }
    public string CaseNumber { get; set; } = default!;
    public string EntryType { get; set; } = default!;
    public string JudgeName { get; set; } = default!;
    public string GeneratedByUpn { get; set; } = default!;  // from EntraID claim
    public string SavedFilePath { get; set; } = default!;
    public DateTime GeneratedAtUtc { get; set; }
    public bool WasRegenerated { get; set; }
}
```

Expose a filterable `/admin/entry-log` page gated behind a `Supervisor` role claim. Filter by date, judge, staff member, or entry type; export to CSV.

This also answers "did you already do the FTA for 24CRB1234?" — clerks can self-serve the lookup.

---

### Problem 4 — Form State Lost on Browser Refresh / VPN Hiccup

**User Pain:** Partial form entries wiped on any interruption. Staff invented a Notepad shadow process.

**Status: 🟠 Planned**

**Solution: Auto-Save Form Draft to `localStorage`**

```csharp
public class FormDraftService
{
    private readonly ILocalStorageService _localStorage;

    public async Task SaveDraftAsync<T>(string formKey, T model)
    {
        var draft = new FormDraft<T> { Model = model, SavedAtUtc = DateTime.UtcNow };
        await _localStorage.SetItemAsync(formKey, draft);
    }

    public async Task<FormDraft<T>?> LoadDraftAsync<T>(string formKey)
        => await _localStorage.GetItemAsync<FormDraft<T>>(formKey);

    public async Task ClearDraftAsync(string formKey)
        => await _localStorage.RemoveItemAsync(formKey);
}
```

- Auto-save every 30 seconds via a debounced timer in `OnAfterRenderAsync`.
- On init, check for a draft and offer: **"You have an unsaved draft from 10:42 AM — restore it?"**
- Clear draft only on successful generation.

---

### Problem 5 — No Confirmation Before Generation

**User Pain:** Fat-finger submit fires immediately with half-complete data. No recovery.

**Status: 🟠 Planned**

**Solution: Review Modal Before Final POST**

```razor
<MudDialog>
    <TitleContent><MudText Typo="Typo.h6">Review Before Generating</MudText></TitleContent>
    <DialogContent>
        <MudText><strong>Case:</strong> @Model.CaseNumber</MudText>
        <MudText><strong>Entry Type:</strong> @Model.EntryType</MudText>
        <MudText><strong>Judge:</strong> @Model.JudgeName</MudText>
        <MudText><strong>Defendant:</strong> @Model.DefendantName</MudText>
        @if (Model.IsRegeneration)
        {
            <MudAlert Severity="Severity.Warning">
                A document for this case was already generated today.
                This will create a second version.
            </MudAlert>
        }
    </DialogContent>
    <DialogActions>
        <MudButton OnClick="Cancel">Go Back</MudButton>
        <MudButton Color="Color.Primary" OnClick="Confirm">Generate Document</MudButton>
    </DialogActions>
</MudDialog>
```

The modal also checks the audit log — if a DOCX was already generated for this case number today, the warning fires automatically.

---

### Problem 6 — Scheduling Forms Re-Selecting Judge Every Time

**User Pain:** 30+ arraignments in a morning, picking judge on each form submission.

**Status: ✅ Resolved by Problem 1 fix**

The `JudicialOfficerSession` is already scoped per browser session and pre-fills the judge field on all forms. The scheduling form reads from the session and shows the active judge as a read-only display with a small change link:

```razor
<MudField Label="Judge" Variant="Variant.Outlined" ReadOnly="true">
    @OfficerSession.JudicialOfficer?.FullName
    <MudIconButton Icon="@Icons.Material.Filled.Edit" Size="Size.Small" OnClick="ChangeJudge" />
</MudField>
```

---

### Problem 7 — Batch FTA ZIP vs. Open Folder

**User Pain:** Expecting the folder to open automatically (Python behaviour). Got a ZIP instead.

**Status: 🟠 Planned (pending Problem 2 — server-side save)**

Once server-side saving (Problem 2) is implemented, batch FTA saves all files directly to `M:\Entries\Batch\{date}\` and the success card reads:

> ✅ **12 FTA entries generated**
> Saved to `M:\Entries\Batch\2026-03-10\`
> [View Generation Log](#)

No ZIP. No extraction. Matches the existing mental model.

**Short-term workaround (until server-side save lands):** Add explicit UI copy:
> *"Your batch FTA documents will download as a ZIP file. Extract and print from the extracted folder."*

---

## Summary Priority Matrix

| Priority | Problem | Solution | Status |
|----------|---------|----------|--------|
| 🔴 P0 | Wrong judge on submission | `FormPageBase` guard + `AppInitializer` error state + `AddJudicialOfficerRule` | ✅ Done |
| 🔴 P0 | Form state lost on refresh | `FormDraftService` with `localStorage` | 🟠 Planned |
| 🟠 P1 | DOCX naming & save path | `Content-Disposition` header + server-side save | 🟠 Planned |
| 🟠 P1 | No confirmation before generate | Review modal with duplicate detection | 🟠 Planned |
| 🟠 P1 | No audit trail | `EntryGenerationLog` + filterable admin view | 🟠 Planned |
| 🟡 P2 | Judge re-select on every form | Resolved by P0 session fix | ✅ Done |
| 🟡 P2 | Batch FTA ZIP confusion | Server-side save to `M:\Entries\Batch\` | 🟠 Planned (needs P1 save first) |

---

> The common thread across almost every complaint is the same: **the Python app held state and took responsibility for side effects**. The Blazor app correctly moved to a stateless model, but didn't yet provide the services that replace what stateful behaviour was doing for free. These solutions restore that contract without sacrificing the architectural improvements the new system brings.

---

_Last updated: March 10, 2026_
