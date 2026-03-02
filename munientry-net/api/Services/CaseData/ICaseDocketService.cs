using Munientry.Shared.Dtos;

namespace Munientry.Api.Services
{
    public interface ICaseDocketService
    {
        Task<List<CaseDocketEntryDto>> GetCaseDocketAsync(string caseNumber);
    }
}
