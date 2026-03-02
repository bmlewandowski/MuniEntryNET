using Microsoft.AspNetCore.Routing;
using Munientry.Shared.Dtos;

using Munientry.Api.Services;

namespace Munientry.Api.Endpoints;

public static class DiversionEndpoints
{
    public static IEndpointRouteBuilder MapDiversionEndpoints(this IEndpointRouteBuilder routes)
    {

        routes.MapPost("/diversiondialog", (DiversionDialogDto dto, IDiversionDialogService svc) =>
            DocxResult.File(svc.GenerateDocx(dto), "DiversionDialog", dto.CaseNumber));

        routes.MapPost("/diversionplea", (DiversionPleaDto dto, IDiversionPleaService svc) =>
            DocxResult.File(svc.GenerateDocx(dto), "DiversionPlea", dto.CaseNumber));

        return routes;
    }
}