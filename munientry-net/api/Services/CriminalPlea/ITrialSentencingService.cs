using Munientry.Shared.Dtos;

namespace Munientry.Api.Services;

// TrialSentencingService reuses FinalJuryNoticeDto (same courtroom-scheduling form shape).
public interface ITrialSentencingService : IDocxService<FinalJuryNoticeDto>;
