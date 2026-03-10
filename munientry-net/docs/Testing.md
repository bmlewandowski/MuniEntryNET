# MuniEntry .NET — Test Suite Guide

> **Last updated:** March 10, 2026  
> **Scope:** `api.Tests/` — integration tests using `WebApplicationFactory`. Blazor component tests are not yet present.

---

## Overview

All tests live in `api.Tests/` and run as **integration tests** against a real in-memory hosting of the API (`Microsoft.AspNetCore.Mvc.Testing`). There are no mocked `HttpContext` objects — every test sends an actual HTTP request through the full middleware pipeline (exception handler, audit middleware, FluentValidation endpoint filter, DOCX generation service).

There are two test categories:

| Category | File pattern | Purpose |
|---|---|---|
| Happy-path DOCX generation | `test_create_*.cs` (one per form) | POST a fully valid DTO → assert `200 OK` + `Content-Type: application/vnd.openxmlformats-officedocument.wordprocessingml.document` |
| Validation failure paths | `ValidationFailureTests.cs` | POST an intentionally broken DTO → assert `422 Unprocessable Entity` + RFC 7807 `errors` key; POST a valid bypass case → assert `200 OK` |

---

## Running the Tests

```bash
# From the repo root
dotnet test api.Tests/api.Tests.csproj

# With verbose output
dotnet test api.Tests/api.Tests.csproj --logger "console;verbosity=detailed"
```

Expected output after the full suite:

```
Passed! - Failed: 0, Passed: 163, Skipped: 0, Total: 163
```

_(Count will grow as new forms and validators are added.)_

---

## Infrastructure: `MuniEntryWebApplicationFactory`

**File:** `api.Tests/Infrastructure/MuniEntryWebApplicationFactory.cs`

All tests share a single `WebApplicationFactory<Program>` subclass. It replaces the 7 SQL-backed read services with lightweight in-memory fakes so tests never need a database connection:

| Replaced service | Fake behavior |
|---|---|
| `ICaseSearchService` | Returns a hardcoded test `CaseModel` for case number `"99-TRC-99999"` |
| `IDailyListService` | Returns a fixed list of two dummy cases |
| `ICaseDocketService` | Returns an empty docket list |
| `IDrivingCaseService` | Returns a hardcoded driving case |
| `IEventReportService` | Returns an empty report list |
| `IFtaReportService` | Returns an empty FTA list |
| `INotGuiltyReportService` | Returns an empty not-guilty report list |

**DOCX services are NOT replaced.** The actual `DocxTemplateProcessor` runs against the real `.docx` templates in `api/Templates/`. This means a happy-path test exercises the full document generation path end-to-end.

> **Resilience pipeline and fakes:** The `"sql-transient"` `ResiliencePipeline` registered by `ServiceRegistration` is present in the test host but never triggered during tests. Because the fake services are direct `IXxxService` implementations (they do not inherit `SqlServiceBase`), no ADO.NET calls are made and the Polly pipeline is bypassed entirely. This is correct — tests validate business logic and HTTP contracts, not transient-fault retry behavior.

### Adding a new fake

If a new SQL-backed service is added to the API, add a corresponding fake to `MuniEntryWebApplicationFactory.ConfigureWebHost(...)`:

```csharp
services.Replace(new ServiceDescriptor(
    typeof(IMyNewSqlService),
    _ => new FakeMyNewSqlService(),
    ServiceLifetime.Scoped));
```

---

## Happy-Path Tests

File naming convention: `test_create_<form_name>.cs`

Each file contains one test class with (at minimum) a single `[Fact]` that:

1. Constructs a fully valid DTO for the form.
2. POSTs it to the corresponding endpoint (`/api/v1/<route>`).
3. Asserts `200 OK`.
4. Asserts the response `Content-Type` is `application/vnd.openxmlformats-officedocument.wordprocessingml.document`.

This proves that the template can be found, opened, and written to from the test environment.

---

## Validation Failure Tests

**File:** `api.Tests/ValidationFailureTests.cs`  
**Test count:** 40 tests across 8 validators (+ 2 judicial officer rule tests)

### Design principles

- Each test mutates **exactly one field** of an otherwise-valid DTO. This isolates which rule fired and prevents false positives from stacking errors.
- Valid DTOs are produced by `ValidXxxDto()` factory methods at the bottom of the class. Keep these factory methods updated if DTO properties are added or renamed.
- The constant `private const HttpStatusCode Unprocessable = HttpStatusCode.UnprocessableEntity;` is used for readability throughout.
- `ReadProblemAsync()` parses the RFC 7807 response body into a `JsonDocument` when the test needs to inspect `errors` keys.

### Covered validators and test regions

| Validator | Region | Tests | Key rules exercised |
|---|---|---|---|
| `NotGuiltyPleaValidator` | Defense counsel | 4 | `AddDefenseCounselRule` (null/empty name when not waived; waived bypass) |
| `NotGuiltyPleaValidator` | Bond rules | 7 | `AddBondRules` via `[Theory]` — `Cash`, `10%`, `OR`, `Property`, `CashOrSurety`, `No Bond` bypass; `UnknownBondType` failure |
| `NotGuiltyPleaValidator` | Judicial officer (representative) | 4 | `AddJudicialOfficerRule` — null/empty first name, empty last name, both null, valid pass-through |
| `JailCcPleaValidator` | Suspend > imposed | 3 | `SuspendedSentence` can't exceed `JailSentence` (days and months independently) |
| `DiversionPleaValidator` | Diversion dates | 3 | Diversion start must be in the past; diversion deadline must be in the future |
| `BondHearingValidator` | Bond hearing | 4 | `AddDefenseCounselRule`; `AddBondRules` on the `BondHearingDto` path |
| `LeapSentencingValidator` | LEAP dates | 4 | `LeapAdmissionDate` must be in the past; `SentenceDate` must be in the past |
| `SchedulingEntryValidator` | Scheduling dates | 5 | Multiple future-date fields (trial date, pretrial date, etc.) |
| `SchedulingEntryValidator` | Judicial officer (single-field) | 1 | `JudicialOfficer` (last name string) must not be empty |
| `FineOnlyPleaValidator` | Fine-only | 2 | `AddDefenseCounselRule`; `FineAmount` must be > 0 |
| _(cross-cutting)_ | RFC 7807 shape | 1 | Response body must have `errors` key (not just `detail`) |

### The RFC 7807 `errors` key test

`PostNotGuiltyPlea_ValidationFailure_Returns422WithErrorsKey` verifies that the `errors` key is present in the problem response body:

```csharp
[Fact]
public async Task PostNotGuiltyPlea_ValidationFailure_Returns422WithErrorsKey()
{
    var dto = ValidNotGuiltyDto();
    dto.DefenseCounselName = null;
    dto.DefenseCounselWaived = false;

    var response = await _client.PostAsJsonAsync("/api/v1/notguiltyplea", dto);
    var problem = await ReadProblemAsync(response);

    Assert.Equal(Unprocessable, response.StatusCode);
    Assert.True(problem.RootElement.TryGetProperty("errors", out _),
        "Problem Details response must contain 'errors' key with field-level messages.");
}
```

This test acts as a contract check: if `FluentValidationFilter` is changed in a way that drops the `errors` key (e.g., by switching to `Results.Problem()` instead of `Results.ValidationProblem()`), this test fails immediately.

---

## Bug Fixed: `FluentValidationFilter` returned HTTP 400 instead of 422

**File:** `api/Validation/FluentValidationFilter.cs`  
**Discovered:** March 10, 2026 (surfaced by `ValidationFailureTests.cs`)  
**Fixed:** Same session

### Root cause

`Results.ValidationProblem()` in ASP.NET Core **defaults to HTTP 400 Bad Request**, not 422 Unprocessable Entity, despite its name implying domain-level validation failure.

**Before (bug):**
```csharp
return Results.ValidationProblem(result.ToDictionary());
```

**After (fix):**
```csharp
return Results.ValidationProblem(
    result.ToDictionary(),
    statusCode: StatusCodes.Status422UnprocessableEntity);
```

### Secondary impact on the Blazor client

`FormPageBase<TDto>.ReadErrorMessageAsync` (in `client/Shared/FormPageBase.razor.cs`) reads the response body and surfaces error messages to the user. At the time of the bug:

- The method only read the `detail` key from the RFC 7807 body.
- `Results.ValidationProblem()` puts field errors under the `errors` key, not `detail`.
- Additionally, the method checked for status 422, but the filter was returning 400.

**Combined effect:** Validation errors were swallowed. The user saw `"An error occurred. Please try again."` instead of the field-specific messages (e.g., `"Defense counsel name is required."`).

Both issues are now resolved:

- The 422 status code fix is applied in `FluentValidationFilter`.
- `FormPageBase.ReadErrorMessageAsync` has been updated to read from the `errors` key on 422 responses and join all field messages into a single display string. The `detail` key path is retained for server error (5xx) responses. See architecture review item P16.

---

## Writing a New Validation Test

1. **Find or add the validator** in `shared/Validation/FormValidators.cs`.
2. **Add (or extend) a `ValidXxxDto()` factory** at the bottom of `ValidationFailureTests.cs` that returns a DTO where every required field satisfies all rules.
3. **Add a failure test** — mutate exactly one field, POST, assert `422`.
4. **Add a bypass test** where applicable — test the condition that makes the rule optional (e.g., `WaivedCounsel = true`, `BondType = "OR"`), assert `200`.

Template:

```csharp
[Fact]
public async Task PostXxx_FieldInvalid_Returns422()
{
    var dto = ValidXxxDto();
    dto.SomeField = /* invalid value */;

    var response = await _client.PostAsJsonAsync("/api/v1/xxxroute", dto);

    Assert.Equal(Unprocessable, response.StatusCode);
}

[Fact]
public async Task PostXxx_BypassCondition_Returns200()
{
    var dto = ValidXxxDto();
    dto.BypassFlag = true;
    dto.SomeField = null; // would normally fail

    var response = await _client.PostAsJsonAsync("/api/v1/xxxroute", dto);

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
}
```

---

## Adding Tests for a New Form

When a new form is added to the API:

1. **Happy-path test** — create `api.Tests/test_create_<form>.cs`, follow the existing pattern.
2. **Validation tests** — if the new form's DTO has a validator in `FormValidators.cs`, add a region to `ValidationFailureTests.cs` covering each rule.
3. **Fake** — if the new endpoint calls a new SQL service, add a fake to `MuniEntryWebApplicationFactory`.
4. **Update this doc** — add the new validator to the "Covered validators" table above.

> **Judicial officer rule convention:** All happy-path DTO factories must populate `JudicialOfficerFirstName`, `JudicialOfficerLastName`, and `JudicialOfficerType` (split-field pattern) or `JudicialOfficer` (single-field pattern). `AddJudicialOfficerRule` is applied to every validator � omitting these fields will produce a 422 and cause the happy-path test to fail.

---

_Last updated: March 10, 2026_
