using Microsoft.AspNetCore.Routing;
using Munientry.Shared.Dtos;

using Munientry.Api.Services;

namespace Munientry.Api.Endpoints;

public static class CivilEndpoints
{
    public static IEndpointRouteBuilder MapCivilEndpoints(this IEndpointRouteBuilder routes)
    {

        routes.MapPost("/noticesfreeformcivil", (NoticesFreeformCivilDto dto, INoticesFreeformCivilService svc) =>
            DocxResult.File(svc.GenerateDocx(dto), "NoticesFreeformCivil", dto.CaseNumber));

        routes.MapPost("/civilfreeformentry", (CivilFreeformEntryDto dto, ICivilFreeformEntryService svc) =>
            DocxResult.File(svc.GenerateDocx(dto), "CivilFreeformEntry", dto.CaseNumber));

        return routes;
    }
}
