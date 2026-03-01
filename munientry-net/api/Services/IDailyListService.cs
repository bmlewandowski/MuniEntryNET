using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Munientry.Api.Data;

namespace Munientry.Api.Services
{
    public interface IDailyListService
    {
        Task<List<DailyListResultDto>> GetDailyListAsync(string listType, DateTime reportDate);
    }
}
