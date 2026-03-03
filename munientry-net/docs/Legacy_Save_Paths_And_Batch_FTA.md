# Legacy Python: Auto-Save Paths & Batch FTA Behaviour

This document captures how the legacy Python application saves generated DOCX files and how
the batch FTA process works, for reference during the Blazor migration.

> **See also:** [Blazor_DOCX_Migration.md](Blazor_DOCX_Migration.md) — form-to-SP-to-template mapping for the Blazor replacement | [StoredProcedureIntegration.md](StoredProcedureIntegration.md) — the SPs that replaced legacy inline SQL (e.g. `general_case_search_query` → `[reports].[DMCMuniEntryCaseSearch]`)

---

## Auto-Save Location

The Python app saves all generated DOCX files to a **mapped network drive (`M:\`)** immediately
after template fill, using `doc.save(path)` from `docxtpl`. The drive is mapped on each
workstation but points to the same shared network resource. No local temp copy is involved —
the file goes straight to the share.

Each entry type has its own subfolder. All paths come from `config.ini` under `[paths]` and are
loaded at startup by `munientry/settings/paths.py`.

### Save Path by Entry Type

| Entry Type | Config Key | Mapped Path |
|---|---|---|
| Default / fallback | `default_entries_save_path` | `M:\Entries\` |
| Criminal / Traffic | `crimtraffic_save_path` | `M:\Entries\CrimTraffic\` |
| Civil | `civil_save_path` | `M:\Entries\Civil\` |
| Driving Privileges | `drive_save_path` | `M:\Entries\DrivingPrivileges\` |
| Fiscal / Admin | `fiscal_save_path` | `M:\Entries\Fiscal\` |
| Scheduling | `scheduling_save_path` | `M:\Entries\Scheduling\` |
| Juror Payment | `jury_pay_save_path` | `M:\Entries\JuryPay\` |
| Probation | `probation_save_path` | `M:\Entries\Probation\` |
| **Batch FTA** | `batch_save_path` | `M:\Entries\Batch\` |

Digital workflow paths (judge review & approval queue) also live under `M:\Entries\DigitalWorkflow\{JudgeName}\`.

### Document Naming Convention

Files are named `{CaseNumber}_{TemplateName}.docx`, set by `BaseEntryCreator._set_document_name()`:

```
21CRB1234_Crim_Traffic Judgment Entry.docx
21TRC5678_FTA_Arraignment.docx
```

After saving, the app immediately opens the file in Word via `os.startfile()`.

---

## Batch FTA Process

The batch FTA button (`batch_menu.run_batch_fta_process()`) **generates fresh DOCX files** for
every FTA-eligible case — it does **not** collect or ZIP pre-existing files.

### Step-by-Step

1. User is prompted for the arraignment date (format `YYYY-MM-DD`).
2. `batch_fta_query(event_date, next_day)` runs against SQL Server to return all FTA-eligible
   case numbers. The query filters by:
   - Mandatory-appearance cases
   - Criminal cases (`CaseType = 3`)
   - Traffic cases with out-of-state non-compact license, CDL/commercial vehicle, or No OL offense
3. For each case number, `CrimCaseData(case_number)` fetches defendant info (name, etc.) via
   `general_case_search_query`.
4. `create_entry(case_data, event_date)` renders `Batch_Failure_To_Appear_Arraignment_Template.docx`
   and saves the result to `M:\Entries\Batch\{CaseNumber}_FTA_Arraignment.docx`.
5. After all entries are created, a dialog shows the count of entries created.
6. `open_entries_folder('batch_entries')` opens `M:\Entries\Batch\` in Windows Explorer so staff
   can print or distribute from there.

There is **no ZIP step** in the Python app. Staff print from the open folder.

### Warrant Rule Logic

The warrant rule stamped on each FTA entry is determined by the case number type code
(characters 2–4):

| Case Type Code | Warrant Rule |
|---|---|
| `CRB` | Criminal Rule 4 |
| All others (`TRC`, `TRD`, `TRE`, etc.) | Traffic Rule 7 |

---

## Blazor Migration Notes

The Blazor API (`ReportsEndpoints.cs`) re-implements this as **stateless, on-demand generation**
with no file system dependency:

| Concern | Legacy Python | Blazor API |
|---|---|---|
| Single FTA entry | Renders DOCX → saves to `M:\Entries\Batch\` → opens in Word | `GET /reports/fta/entry/{caseNumber}/{date}` → returns DOCX stream to browser |
| Batch FTA entries | Renders one DOCX per case → saves each to `M:\Entries\Batch\` → opens folder | `GET /reports/fta/batch/{date}` → renders all DOCXs in memory → returns single ZIP stream |
| No persistent store needed | Files remain on network share indefinitely | Files are ephemeral — browser download only; no server-side storage |
| Re-run same batch | Re-saves over existing files silently | Re-generates from DB, re-downloads |

The Blazor approach eliminates the mapped drive dependency, making the app suitable for
containerised/cloud deployment. If a persistent record of generated entries is required,
a storage backend (Azure Blob, SQL, SMB share mounted in the container) would need to be added.

---

_Last updated: March 3, 2026_
