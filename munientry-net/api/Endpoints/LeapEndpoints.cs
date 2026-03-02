using Microsoft.AspNetCore.Routing;
using Munientry.Shared.Dtos;

using Munientry.Api.Services;

namespace Munientry.Api.Endpoints;

public static class LeapEndpoints
{
    public static IEndpointRouteBuilder MapLeapEndpoints(this IEndpointRouteBuilder routes)
    {

        routes.MapPost("/leapadmissionplea", (LeapAdmissionPleaDto dto, ILeapAdmissionPleaService svc) =>
            DocxResult.File(svc.GenerateDocx(dto), "LeapAdmissionPlea", dto.CaseNumber));

        routes.MapPost("/leapadmissionalreadyvalid", (LeapAdmissionAlreadyValidDto dto, ILeapAdmissionAlreadyValidService svc) =>
            DocxResult.File(svc.GenerateDocx(dto), "LeapAdmissionAlreadyValid", dto.CaseNumber));

        routes.MapPost("/leapvalidsentencing", (LeapValidSentencingDto dto, ILeapValidSentencingService svc) =>
            DocxResult.File(svc.GenerateDocx(dto), "LeapValidSentencing", dto.CaseNumber));

        routes.MapPost("/leapsentencing", (LeapSentencingDto dto, ILeapSentencingService svc) =>
            DocxResult.File(svc.GenerateDocx(dto), "LeapSentencing", dto.CaseNumber));

        return routes;
    }
}