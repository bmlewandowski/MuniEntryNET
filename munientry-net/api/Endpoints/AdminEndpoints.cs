using Microsoft.AspNetCore.Routing;
using Munientry.Shared.Dtos;

using Munientry.Api.Services;

namespace Munientry.Api.Endpoints;

public static class AdminEndpoints
{
    public static IEndpointRouteBuilder MapAdminEndpoints(this IEndpointRouteBuilder routes)
    {

        routes.MapPost("/fiscaljournalentry", (FiscalJournalEntryDto dto, IFiscalJournalEntryService svc) =>
            DocxResult.File(svc.GenerateDocx(dto), "FiscalJournalEntry", dto.AccountNumber));

        routes.MapPost("/timetopayorder", (TimeToPayOrderDto dto, ITimeToPayOrderService svc) =>
            DocxResult.File(svc.GenerateDocx(dto), "TimeToPay", dto.CaseNumber));

        routes.MapPost("/criminalsealing", (CriminalSealingEntryDto dto, ICriminalSealingEntryService svc) =>
            DocxResult.File(svc.GenerateDocx(dto), "CriminalSealingEntry", dto.CaseNumber));

        routes.MapPost("/competencyevaluation", (CompetencyEvaluationDto dto, ICompetencyEvaluationService svc) =>
            DocxResult.File(svc.GenerateDocx(dto), "CompetencyEvaluation", dto.CaseNumber));

        return routes;
    }
}
