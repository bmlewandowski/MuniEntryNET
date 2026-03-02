using Microsoft.AspNetCore.Routing;
using Munientry.Shared.Dtos;

using Munientry.Api.Services;

namespace Munientry.Api.Endpoints;

public static class NoticesAndSchedulingEndpoints
{
    public static IEndpointRouteBuilder MapNoticesAndSchedulingEndpoints(this IEndpointRouteBuilder routes)
    {

        routes.MapPost("/trialtocourt", (TrialToCourtNoticeDto dto, ITrialToCourtNoticeService svc) =>
            DocxResult.File(svc.GenerateDocx(dto), "TrialToCourt", dto.CaseNumber));

        routes.MapPost("/finaljury", (FinalJuryNoticeDto dto, IFinalJuryNoticeService svc) =>
            DocxResult.File(svc.GenerateDocx(dto), "FinalJury", dto.CaseNumber));

        routes.MapPost("/generalnoticeofhearing", (GeneralNoticeOfHearingDto dto, IGeneralNoticeOfHearingService svc) =>
            DocxResult.File(svc.GenerateDocx(dto), "GeneralNoticeOfHearing", dto.CaseNumber));

        routes.MapPost("/schedulingentry", (SchedulingEntryDto dto, ISchedulingEntryService svc) =>
            DocxResult.File(svc.GenerateDocx(dto), "SchedulingEntry", dto.CaseNumber));

        routes.MapPost("/jurorpayment", (JurorPaymentDto dto, IJurorPaymentService svc) =>
            DocxResult.File(svc.GenerateDocx(dto), "JurorPayment", dto.CaseNumber));

        return routes;
    }
}
