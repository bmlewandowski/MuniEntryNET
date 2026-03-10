using Munientry.Shared.Dtos;

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
        // date: yyyy-MM-dd — validated here for a clean 400 response; the SPs filter by
        //   GETDATE() internally and do not accept a date parameter.
        // Note: string→DateTime conversion is done manually rather than via typed route
        //   binding because .NET 10 minimal API throws on DateTime parse failure (hitting
        //   GlobalExceptionHandler → 500) instead of returning 400 gracefully.
        app.MapGet("/dailylist/{listType}/{date}", async (string listType, string date, IDailyListService service) =>
        {
            if (!DateTime.TryParse(date, out _))
                return Results.BadRequest("Invalid date format. Use yyyy-MM-dd.");
            try
            {
                var result = await service.GetDailyListAsync(listType);
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

        // Case docket — chronological docket entries (date + remark) for a case.
        // Calls [reports].[DMCMuniEntryCaseDocket].
        // Replaces the legacy get_case_docket_query used by CrimCaseDocket.get_docket().
        app.MapGet("/case/docket/{caseNumber}", async (string caseNumber, ICaseDocketService service) =>
        {
            var result = await service.GetCaseDocketAsync(caseNumber);
            return result.Count > 0 ? Results.Ok(result) : Results.NotFound();
        });

        return app;
    }
}
