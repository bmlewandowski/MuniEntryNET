using Munientry.Api.Data;

namespace Munientry.Api.Services;

public interface ICivilFreeformEntryService
{
    void InsertCivilFreeformEntry(CivilFreeformEntryDto dto);
}
