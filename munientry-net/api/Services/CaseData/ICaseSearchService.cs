using System.Collections.Generic;
using System.Threading.Tasks;
using Munientry.Shared.Dtos;

namespace Munientry.Api.Services
{
    public interface ICaseSearchService
    {
        Task<List<CaseSearchResultDto>> SearchCaseAsync(string caseNumber);
    }
}
