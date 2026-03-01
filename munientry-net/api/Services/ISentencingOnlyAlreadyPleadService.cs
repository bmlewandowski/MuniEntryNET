using Munientry.Api.Data;

namespace Munientry.Api.Services;

public interface ISentencingOnlyAlreadyPleadService
{
    void Save(SentencingOnlyAlreadyPleadDto dto);
}
