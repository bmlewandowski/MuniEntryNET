# MuniEntry .NET — Documentation Index

This folder documents the Blazor + ASP.NET Core replacement for the legacy PyQt6 desktop application.
Start here to find the right doc for your question.

---

## Doc Map

| Document | What It Answers |
|---|---|
| [Blazor_DOCX_Migration.md](Blazor_DOCX_Migration.md) | *"What Blazor form maps to which SP, which DOCX template, and which legacy Python form?"* — master form/SP/template/legacy mapping table |
| [DTO_Reference.md](DTO_Reference.md) | *"What DTOs exist and how are they structured?"* — shared DTO list, base class `FormPageBase<TDto>`, PR guidelines |
| [StoredProcedureIntegration.md](StoredProcedureIntegration.md) | *"How are stored procedures called? What are the connection strings? What do I need to know for local vs Docker vs K8s?"* |
| [EntraID_Setup.md](EntraID_Setup.md) | *"How do I turn on authentication?"* — step-by-step Entra ID enablement for client **and** API |
| [SECURITY_ARCHITECTURE_REVIEW.md](SECURITY_ARCHITECTURE_REVIEW.md) | *"What are the current security gaps and resolved items?"* — all 24 security/architecture items with status |
| [Legacy_Save_Paths_And_Batch_FTA.md](Legacy_Save_Paths_And_Batch_FTA.md) | *"How did the Python app save files and run batch FTA?"* — migration reference for save paths and batch behavior |
| [Pseudo_User_Analysis.md](Pseudo_User_Analysis.md) | *"What will a court staff user think of the Blazor app on day 1, week 2, and month 2?"* — simulated transition UX analysis |
| [Pseudo_User_Response.md](Pseudo_User_Response.md) | *"What should we build next, in what order?"* — tiered engineering improvement plan with code examples |
| [Pseudo_User_Analysis_2.md](Pseudo_User_Analysis_2.md) | *"Same user, second voice — first impressions through week 6, and things I wish I'd known."* — ground-level staff account used to drive the Round 2 response |
| [Pseudo_User_Response_2.md](Pseudo_User_Response_2.md) | *"What does the engineer do about it?"* — Round 2 improvement plan; Problem 1 (wrong judge on submission) is implemented; Problems 2–7 are planned with code references |
| [Testing.md](Testing.md) | *"How is the test suite structured? How do I run tests? How do I add validation tests?"* — test infrastructure, `MuniEntryWebApplicationFactory` fakes, happy-path and validation-failure test patterns, 422 bug history |

---

## Quick Decision Tree

**Deploying / enabling auth?** → [EntraID_Setup.md](EntraID_Setup.md), then [SECURITY_ARCHITECTURE_REVIEW.md](SECURITY_ARCHITECTURE_REVIEW.md) for remaining blockers

**Adding a new form?** → [DTO_Reference.md](DTO_Reference.md) for DTO/base class pattern → [Blazor_DOCX_Migration.md](Blazor_DOCX_Migration.md) to update the mapping table → [StoredProcedureIntegration.md](StoredProcedureIntegration.md) if a new SP is needed

**Understanding the legacy system?** → [Legacy_Save_Paths_And_Batch_FTA.md](Legacy_Save_Paths_And_Batch_FTA.md) for file save behavior, [Blazor_DOCX_Migration.md](Blazor_DOCX_Migration.md) for form equivalents

**Prioritizing pre-launch work?** → [Pseudo_User_Response_2.md](Pseudo_User_Response_2.md) (Round 2 improvement plan, Problem 1 implemented) cross-referenced with [SECURITY_ARCHITECTURE_REVIEW.md](SECURITY_ARCHITECTURE_REVIEW.md) (security blockers)

**Writing or running tests?** → [Testing.md](Testing.md) for test structure, infrastructure fakes, validation-failure test patterns, and the `FluentValidationFilter` 422 fix history

---

## Current Blocker Summary (as of March 3, 2026)

| Blocker | Status | Doc |
|---|---|---|
| Authentication disabled (client + API) | 🔴 Open | [EntraID_Setup.md](EntraID_Setup.md) |
| CORS fully open (`AllowAnyOrigin`) | 🔴 Open | [SECURITY_ARCHITECTURE_REVIEW.md](SECURITY_ARCHITECTURE_REVIEW.md) item 3 |
| Role / claim model not yet designed | 🔴 Open | [SECURITY_ARCHITECTURE_REVIEW.md](SECURITY_ARCHITECTURE_REVIEW.md) item 19 |
| Civil form inline SQL (no SP) | 🟡 Known gap | [StoredProcedureIntegration.md](StoredProcedureIntegration.md), [Blazor_DOCX_Migration.md](Blazor_DOCX_Migration.md) |
| `EntryLog` persistent audit table (UI-queryable) | 🟡 Open | [Pseudo_User_Response.md](Pseudo_User_Response.md) item 7 |
| K8s TLS / Secrets management | 🟡 Open | [SECURITY_ARCHITECTURE_REVIEW.md](SECURITY_ARCHITECTURE_REVIEW.md) items 8–9 |

_Last updated: March 10, 2026_
