using Microsoft.AspNetCore.Routing;
using Munientry.Shared.Dtos;

using Munientry.Api.Services;

namespace Munientry.Api.Endpoints;

public static class BondEndpoints
{
    public static IEndpointRouteBuilder MapBondEndpoints(this IEndpointRouteBuilder routes)
    {

        routes.MapPost("/bondhearing", (BondHearingDto dto, IBondHearingService svc) =>
            DocxResult.File(svc.GenerateDocx(dto), "BondHearing", dto.CaseNumber));

        routes.MapPost("/bondmodificationrevocation", (BondModificationRevocationDto dto, IBondModificationRevocationService svc) =>
            DocxResult.File(svc.GenerateDocx(dto), "BondModificationRevocation", dto.CaseNumber));

        routes.MapPost("/probationviolationbond", (ProbationViolationBondDto dto, IProbationViolationBondService svc) =>
            DocxResult.File(svc.GenerateDocx(dto), "ProbationViolationBond", dto.CaseNumber));

        return routes;
    }
}
