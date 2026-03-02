using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Munientry.Shared.Dtos;

namespace Munientry.Api.Services
{
    public interface IDailyListService
    {
        Task<List<DailyListResultDto>> GetDailyListAsync(string listType, DateTime reportDate);
    }
}
