using Munientry.Shared.Dtos;

namespace Munientry.Api.Services
{
    public interface INotGuiltyReportService
    {
        Task<List<NotGuiltyReportResultDto>> GetNotGuiltyReportAsync(DateTime eventDate);
    }
}
