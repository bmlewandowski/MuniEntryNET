using Munientry.Api.Data;

namespace Munientry.Api.Services
{
    public class SchedulingEntryService : ISchedulingEntryService
    {
        public SchedulingEntryService()
        {
        }

        public Task<bool> CreateSchedulingEntryAsync(SchedulingEntryDto dto)
        {
            // TODO: Implement business logic and persistence
            return Task.FromResult(true);
        }
    }
}
