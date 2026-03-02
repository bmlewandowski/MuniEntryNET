using Microsoft.AspNetCore.Routing;
using Munientry.Shared.Dtos;

using Munientry.Api.Services;

namespace Munientry.Api.Endpoints;

public static class DrivingEndpoints
{
    public static IEndpointRouteBuilder MapDrivingEndpoints(this IEndpointRouteBuilder routes)
    {

        routes.MapPost("/drivingprivileges", (DrivingPrivilegesDto dto, IDrivingPrivilegesService svc) =>
            DocxResult.File(svc.GenerateDocx(dto), "DrivingPrivileges", dto.CaseNumber));

        routes.MapPost("/denyprivilegespermitretest", (DenyPrivilegesPermitRetestDto dto, IDenyPrivilegesPermitRetestService svc) =>
            DocxResult.File(svc.GenerateDocx(dto), "DenyPrivilegesPermitRetest", dto.CaseNumber));

        return routes;
    }
}
