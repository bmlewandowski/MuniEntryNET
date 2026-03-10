using Munientry.Shared.Dtos;

namespace Munientry.Api.Services
{
    public interface IDailyListService
    {
        Task<List<DailyListResultDto>> GetDailyListAsync(string listType);
    }
}
