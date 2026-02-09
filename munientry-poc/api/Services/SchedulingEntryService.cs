using Munientry.Poc.Api.Data;

namespace Munientry.Poc.Api.Services
{
    public class SchedulingEntryService
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
