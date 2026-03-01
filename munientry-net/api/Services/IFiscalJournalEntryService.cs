using Munientry.Api.Data;

namespace Munientry.Api.Services;

public interface IFiscalJournalEntryService
{
    void InsertFiscalJournalEntry(FiscalJournalEntryDto dto);
}
