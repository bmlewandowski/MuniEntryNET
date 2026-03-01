using Munientry.Api.Services;

namespace Munientry.Api.Endpoints;

/// <summary>
/// GET endpoints for loading/querying data into the Blazor forms.
/// These are read-only lookups against the SQL Server database via stored procedures.
/// </summary>
internal static class CaseDataEndpoints
{
    internal static IEndpointRouteBuilder MapCaseDataEndpoints(this IEndpointRouteBuilder app)
    {
        // Case search — calls [reports].[DMCMuniEntryCaseSearch]
        // Returns all charges + defendant info for pre-populating criminal dialogs.
        app.MapGet("/case/search/{caseNumber}", async (string caseNumber, ICaseSearchService service) =>
        {
            var result = await service.SearchCaseAsync(caseNumber);
            return result.Count > 0 ? Results.Ok(result) : Results.NotFound();
        });

        // Daily case list — calls one of the 6 scheduled-hearing stored procedures.
        // listType: arraignments, slated, pleas, pcvh_fcvh, final_pretrial, trials_to_court
        // date format: yyyy-MM-dd
        app.MapGet("/dailylist/{listType}/{date}", async (string listType, string date, IDailyListService service) =>
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

        // Driving case lookup — pre-populates the DrivingPrivileges form.
        app.MapGet("/drivingcase/{caseNumber}", async (string caseNumber, IDrivingCaseService service) =>
        {
            var result = await service.GetDrivingCaseInfoAsync(caseNumber);
            return result is not null ? Results.Ok(result) : Results.NotFound();
        });

        // ── Stub GET endpoints (return mock data; replace with DB fetch) ─────

        app.MapGet("/case/{id}", (string id) => Results.Ok(new
        {
            caseNumber = id,
            defendant = "John Doe",
            charges = new[] { new { charge = "Speeding", statute = "S-101" } }
        }));

        app.MapGet("/diversion/{id}", (string id) => Results.Ok(new
        {
            caseNumber = id,
            defendantFirstName = "Jane",
            defendantLastName = "Roe",
            diversionCompletionDate = DateTime.UtcNow.AddDays(90).ToString("yyyy-MM-dd"),
            diversionFinePayDate = DateTime.UtcNow.AddDays(120).ToString("yyyy-MM-dd"),
            charges = new[] {
                new { charge = "Speeding", statute = "S-101" },
                new { charge = "No Insurance", statute = "I-202" }
            }
        }));

        app.MapGet("/fineonly/{caseNumber}", (string caseNumber) => Results.Ok(new
        {
            CaseNumber = caseNumber,
            DefendantName = "Jane Doe",
            Charge = "Speeding",
            FineAmount = 150.00m
        }));

        app.MapGet("/civilfreeformentry/{caseNumber}", (string caseNumber) =>
            Results.Ok(new Data.CivilFreeformEntryDto
            {
                EntryDate = DateTime.Today,
                Plaintiff = "Sample Plaintiff",
                Defendant = "Sample Defendant",
                CaseNumber = caseNumber,
                AppearanceReason = "Sample Reason",
                EntryContent = "Sample entry content."
            }));

        return app;
    }
}
