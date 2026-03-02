using Munientry.Shared.Dtos;

namespace Munientry.Api.Services
{
    public interface IEventReportService
    {
        Task<List<EventReportResultDto>> GetEventReportAsync(string eventCode, DateTime eventDate);
    }
}
