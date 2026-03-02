using Microsoft.AspNetCore.Routing;
using Munientry.Shared.Dtos;

using Munientry.Api.Services;

namespace Munientry.Api.Endpoints;

public static class ProbationEndpoints
{
    public static IEndpointRouteBuilder MapProbationEndpoints(this IEndpointRouteBuilder routes)
    {

        routes.MapPost("/communitycontroltermsnotices", (CommunityControlTermsNoticesDto dto, ICommunityControlTermsNoticesService svc) =>
            DocxResult.File(svc.GenerateDocx(dto), "CommunityControlTermsNotices", dto.CaseNumber));

        routes.MapPost("/communitycontrolterms", (CommunityControlTermsDto dto, ICommunityControlTermsService svc) =>
            DocxResult.File(svc.GenerateDocx(dto), "CommunityControlTerms", dto.CaseNumber));

        routes.MapPost("/communityservicesecondary", (CommunityServiceSecondaryDto dto, ICommunityServiceSecondaryService svc) =>
            DocxResult.File(svc.GenerateDocx(dto), "CommunityServiceSecondary"));

        return routes;
    }
}
