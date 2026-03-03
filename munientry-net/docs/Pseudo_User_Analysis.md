# MuniEntry Blazor — User Perspective Analysis
### *As a Court Administrative Staff Member Transitioning from the Legacy Python App*

> **See also:** [Legacy_Save_Paths_And_Batch_FTA.md](Legacy_Save_Paths_And_Batch_FTA.md) — exact legacy save-path behavior and batch FTA mechanics | [Pseudo_User_Response.md](Pseudo_User_Response.md) — the engineering improvement plan responding to this analysis

---

## First Impression — Day 1 to Week 1

### What Immediately Stands Out (Good)
- **No install, no Python environment, no `pip install` nightmares.** I open a browser, sign in with my Microsoft 365 account, and I'm in. That alone is a win compared to the old app which required IT to touch every machine whenever a dependency broke.
- **EntraID login feels natural.** I already use my 365 account for Outlook and Teams all day. Having it work here too is seamless — no separate password to remember.
- **The forms look cleaner.** Fonts are consistent, spacing is better, it doesn't feel like a 2008 Tkinter dialog anymore.

### What Immediately Causes Friction
- **"Where's my judge selector?"** The old app asked me which judge I was working for the moment it opened. That selection influenced *everything* — which daily list loaded, which folder the DOCX saved to, which template header was used. In the Blazor version it's not obvious where I set that context, or if it persists between sessions.
- **The DOCX downloads to my Downloads folder.** In the old app the file was just *there* on the `M:` drive where the clerk expected it. Now I get a browser download prompt and the file ends up in `C:\Users\me\Downloads`. I have to manually move it. This is going to cause missed filings.
- **The daily list is there but I can't click a case and open a form from it.** In the Python app I could double-click a case in the arraignment list and it would pre-populate the form. That workflow is gone — I have to manually type the case number into each form.
- **Where is the batch FTA button?** Several staff members spent 20 minutes looking for it on day one.

### Emotional Reaction
> *"It looks nicer but it feels slower to actually do my job."*

---

## A Few Weeks In — Week 2 to Week 6

### What I've Gotten Used To
- EntraID auth is completely invisible now. I don't even think about it.
- I've bookmarked the forms I use every day. The URL structure is consistent enough that I can navigate without hunting.
- The form validation is noticeably better — the old app let me submit with blank required fields and the DOCX would just have empty gaps that I wouldn't catch until printing.

### What Is Still Causing Daily Pain

#### 1. The Download Folder Problem Is Worse Than I Thought
Every generated DOCX lands in `Downloads`. Our clerks then have to:
1. Find the file in Downloads
2. Rename it to the case number convention
3. Move it to the correct folder on `M:\`
4. Open it to verify it looks right
5. Print

The old app did steps 2–4 automatically. **This is adding 2–3 minutes per entry** and we do 40–60 entries a day. That's up to 3 hours of extra clerical work daily that nobody budgeted for.

#### 2. No Session Context / Judge Awareness
If I'm working Hemmeter's docket and I accidentally POST a fine-only plea, the template header says the wrong judge. There's no warning. The DOCX goes out wrong. I've already had to reprint twice.

#### 3. Form State Is Lost on Refresh
If the browser refreshes (or my VPN hiccups) mid-form, everything I typed is gone. The Python app at least kept the window open. People are now copy-pasting case numbers into Notepad as a backup before filling long forms.

#### 4. The Scheduling Entry Form Confusion
The old `SchedulingEntryDialog` knew which judge you were working for and auto-filled the courtroom. The Blazor version makes me pick the judge *per form submission*. On a heavy arraignment morning with 30+ cases this is death by a thousand cuts.

#### 5. I Can't Tell If My DOCX Actually Saved
Green checkmark? Confirmation toast? Nothing. The file just downloads. I have no audit trail that I generated this entry. If a clerk asks "did you do the FTA for 24CRB1234?" I have no way to check in the app.

### The "I Wish I Would've Known" Moments Starting to Surface
> *"I wish someone had told me to set my browser's download folder to point directly to `M:\Entries\CrimTraffic\` before I started using this."*

> *"I wish I had known the judge context doesn't carry between tabs. I had two tabs open — one for Fowler and one for Hemmeter — and submitted the wrong one."*

> *"I wish there was a way to reopen the last-submitted form to make a correction. In the old app the dialog was still open. Now it's just gone."*

---

## A Few Months In — Month 2 to Month 4

### What Has Become Accepted (Good)
- **EntraID + role-based access is genuinely better.** When a new clerk joined, IT provisioned them in 5 minutes via Entra groups. No Python install, no config file editing, no IT ticket for the legacy app's shared-drive permissions.
- **Form validation catching errors before printing** has measurably reduced reprints for missing fields. Staff have noticed and acknowledged this.
- **The civil forms and criminal forms being in one app** instead of two separate Python windows is a quality-of-life improvement that took a few months to fully appreciate.
- **The FTA batch ZIP** returning immediately as a download is actually faster than the old process of watching it generate files one-by-one into a folder.

### What Has Become a Genuine Problem

#### 1. No Audit Trail Is a Compliance Risk
After a few months of use, supervisors are asking: *"Can you pull a report of all entries generated this week?"* The answer is no. The app generates and streams a DOCX and forgets it ever happened. The old app had file timestamps on `M:\` that served as a de-facto log. We've lost that entirely and nobody noticed until a filing dispute came up.

#### 2. The Download Workflow Has Created Shadow Processes
Staff have developed their own workarounds — some use browser settings to auto-download to `M:\`, some use a desktop shortcut to their personal subfolder, some just print directly from Downloads and never file to `M:\` at all. **We now have inconsistent filing practices across the office because the app doesn't enforce a save location.**

#### 3. Multi-Charge Forms Are Still Confusing
The LEAP and JailCC forms with the charge table loop work correctly but the UI for *adding* charges in the Blazor form doesn't feel as intuitive as the old "Add Charge" dialog. Staff still occasionally submit with only one charge populated when there should be three.

#### 4. No "Did You Mean To Submit?" Guard on Destructive Actions
The forms have no confirmation step before generating the DOCX. In the old Python app a modal dialog asked "Generate and save this entry?" The Blazor forms just POST immediately on button click. Accidental submissions happen.

---

## Suggested Improvements — Prioritized by Impact

### Critical (Fix Before Full Rollout)

| # | Issue | Suggested Fix |
|---|---|---|
| 1 | DOCX lands in browser Downloads with no enforcement | Add a **user-profile setting** to configure a default save path (UNC or mapped drive). On DOCX return, prompt "Save to your entry folder?" with the configured path pre-filled, or use the File System Access API to write directly. |
| 2 | No judge context session | Store **judge selection in EntraID session claims or a user preference record**. Show the active judge prominently in the nav bar. Warn if a form's judge field doesn't match. |
| 3 | No submission confirmation dialog | Add a **"Review & Submit" step** — show a read-only summary of what will be in the DOCX before the POST fires. |
| 4 | Form state lost on refresh/navigation | Implement **LocalStorage-backed form state** (`Blazored.LocalStorage`) so a partial form survives a browser refresh. Show a "Resume your last entry?" banner on return. |

### High Priority (First Month Post-Rollout)

| # | Issue | Suggested Fix |
|---|---|---|
| 5 | No audit trail | **Log every DOCX generation** to a lightweight `EntryLog` table: `UserId`, `CaseNumber`, `FormType`, `JudgeCode`, `GeneratedAt`, `TemplateVersion`. Surface this as a read-only "My Recent Entries" panel. |
| 6 | Case number must be typed manually | Add a **"Load from Daily List"** button on each form — clicking it opens a case-picker pre-filtered to today's list for the active judge, exactly replicating the double-click workflow. |
| 7 | Multi-charge add/remove UX | Replace the static charge table with a **dynamic add/remove row component** with inline validation (statute format check, degree dropdown). |
| 8 | No reprint / reopen last entry | Surface the last 5 generated entries in a **"Recent" sidebar** (pulled from `EntryLog`) with a one-click "Regenerate this DOCX" action. |

### Medium Priority (Quarter 1 Post-Rollout)

| # | Issue | Suggested Fix |
|---|---|---|
| 9 | Civil SP gap | Create `[reports].[DMCMuniEntryCivilCaseSearch]` to bring civil forms in line with the criminal pattern and remove inline SQL from the API. |
| 10 | No keyboard shortcuts | Power users knew the Python app's keyboard shortcuts cold. Add **`Alt+S` to submit, `Alt+C` to clear, `Alt+L` to load case** across all forms. |
| 11 | Scheduling entry judge re-selection per form | If the judge context is stored in session (fix #2), **pre-populate the scheduling template selection** automatically and let the user override rather than re-select every time. |
| 12 | DOCX filename convention | The downloaded file should be named `{CaseNumber}_{FormType}_{Date}.docx` automatically — never `download (3).docx`. |

### Nice to Have (Quarter 2+)

| # | Suggested Improvement |
|---|---|
| 13 | **Dark mode / high-contrast mode** for staff who work under fluorescent lights all day on a long docket. |
| 14 | **Print directly from the browser** after DOCX generation without requiring Word to open — consider returning an HTML preview or PDF alongside the DOCX. |
| 15 | **Mobile/tablet view** for judges who want to review entry details on a tablet in the courtroom. |
| 16 | **Offline graceful degradation** — if the API is unreachable, show a clear "Server unavailable" message rather than a raw 500 error or a spinner that never resolves. |
| 17 | **Entry template versioning** — tag each generated DOCX with the template version used so that if a template changes mid-year, older entries can still be regenerated correctly. |

---

## Summary Scorecard After 3 Months

| Category | Legacy Python | Blazor (Current) | Blazor (With Fixes) |
|---|---|---|---|
| Authentication | ❌ Shared credentials / IT-managed | ✅ EntraID SSO | ✅ |
| Auto-save to correct network location | ✅ Automatic | ❌ Manual Downloads | ✅ With fix #1 |
| Judge context awareness | ✅ Set at startup | ❌ Per-form only | ✅ With fix #2 |
| Form validation | ⚠️ Minimal | ✅ Better | ✅ |
| Audit trail | ⚠️ File timestamps only | ❌ None | ✅ With fix #5 |
| Load case from daily list | ✅ Double-click | ❌ Manual type | ✅ With fix #6 |
| Multi-device / no install | ❌ Windows only | ✅ Any browser | ✅ |
| Form state persistence | ⚠️ Window stays open | ❌ Lost on refresh | ✅ With fix #4 |
| Submission confirmation | ✅ Modal dialog | ❌ Immediate POST | ✅ With fix #3 |
| Civil/criminal in one app | ❌ Two apps | ✅ Unified | ✅ |

> **Bottom line:** The foundation is right. EntraID, the unified app, and better validation are genuine wins. But the **file save workflow and the loss of judge session context are the two things that will cause the most daily frustration and the most compliance risk** if left unaddressed. Fix those two things before the Python app is decommissioned and the transition will be remembered positively.

_Last updated: March 3, 2026_
