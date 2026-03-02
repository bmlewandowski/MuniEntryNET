using Munientry.Shared.Dtos;

namespace Munientry.Api.Services;

/// <summary>
/// Generates the Admin Fiscal Journal Entry DOCX from <see cref="FiscalJournalEntryDto"/>.
/// Mirrors the legacy <c>AdminFiscalEntryCreator</c> in <c>entrycreators/entry_creator.py</c>.
/// </summary>
public interface IFiscalJournalEntryService : IDocxService<FiscalJournalEntryDto>
{
}
