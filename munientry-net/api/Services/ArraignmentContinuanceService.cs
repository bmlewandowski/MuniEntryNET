using Munientry.Api.Data;

namespace Munientry.Api.Services
{
    public class ArraignmentContinuanceService : IArraignmentContinuanceService
    {
        public ArraignmentContinuanceService() { }

        public Task<bool> CreateArraignmentContinuanceAsync(ArraignmentContinuanceDto dto)
        {
            // TODO: Implement business logic and persistence
            return Task.FromResult(true);
        }
    }
}
