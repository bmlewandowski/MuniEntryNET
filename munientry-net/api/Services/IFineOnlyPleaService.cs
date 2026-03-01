using Munientry.Api.Data;

namespace Munientry.Api.Services;

public interface IFineOnlyPleaService
{
    void CreateFineOnlyPleaEntry(FineOnlyPleaDto dto);
}
