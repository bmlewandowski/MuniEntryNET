using Microsoft.AspNetCore.Routing;
using Munientry.Shared.Dtos;

using Munientry.Api.Services;

namespace Munientry.Api.Endpoints;

public static class CriminalPleaEndpoints
{
    public static IEndpointRouteBuilder MapCriminalPleaEndpoints(this IEndpointRouteBuilder routes)
    {

        routes.MapPost("/jailccplea", (JailCcPleaDto dto, IJailCcPleaService svc) =>
            DocxResult.File(svc.GenerateDocx(dto), "JailCcPlea", dto.CaseNumber));

        routes.MapPost("/notguiltyappearbondspecial", (NotGuiltyAppearBondSpecialDto dto, INotGuiltyAppearBondSpecialService svc) =>
            DocxResult.File(svc.GenerateDocx(dto), "NotGuiltyAppearBondSpecial", dto.CaseNumber));

        routes.MapPost("/arraignmentcontinuance", (ArraignmentContinuanceDto dto, IArraignmentContinuanceService svc) =>
            DocxResult.File(svc.GenerateDocx(dto), "ArraignmentContinuance", dto.CaseNumber));

        routes.MapPost("/fineonlyplea", (FineOnlyPleaDto dto, IFineOnlyPleaService svc) =>
            DocxResult.File(svc.GenerateDocx(dto), "FineOnlyPlea", dto.CaseNumber));

        routes.MapPost("/notguiltyplea", (NotGuiltyPleaDto dto, INotGuiltyPleaService svc) =>
            DocxResult.File(svc.GenerateDocx(dto), "NotGuiltyPlea", dto.CaseNumber));

        routes.MapPost("/sentencingonlyalreadyplead", (SentencingOnlyAlreadyPleadDto dto, ISentencingOnlyAlreadyPleadService svc) =>
            DocxResult.File(svc.GenerateDocx(dto), "SentencingOnlyAlreadyPlead", dto.CaseNumber));

        routes.MapPost("/pleaonlyfuturesentencing", (PleaOnlyFutureSentencingDto dto, IPleaOnlyFutureSentencingService svc) =>
            DocxResult.File(svc.GenerateDocx(dto), "PleaOnlyFutureSentencing", dto.CaseNumber));

        routes.MapPost("/appearonwarrantnoplea", (AppearOnWarrantNoPleaDto dto, IAppearOnWarrantNoPleaService svc) =>
            DocxResult.File(svc.GenerateDocx(dto), "AppearOnWarrantNoPlea", dto.CaseNumber));

        routes.MapPost("/trialsentencing", (FinalJuryNoticeDto dto, ITrialSentencingService svc) =>
            DocxResult.File(svc.GenerateDocx(dto), "TrialSentencing", dto.CaseNumber));

        routes.MapPost("/failuretoappear", (FailureToAppearDto dto, IFailureToAppearService svc) =>
            DocxResult.File(svc.GenerateDocx(dto), "FailureToAppear", dto.CaseNumber));

        routes.MapPost("/freeformentry", (CriminalFreeformEntryDto dto, ICriminalFreeformEntryService svc) =>
            DocxResult.File(svc.GenerateDocx(dto), "FreeformEntry", dto.CaseNumber));

        return routes;
    }
}