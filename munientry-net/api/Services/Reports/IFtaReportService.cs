using Munientry.Shared.Dtos;

namespace Munientry.Api.Services
{
    public interface IFtaReportService
    {
        Task<List<FtaReportResultDto>> GetFtaReportAsync(DateTime eventDate);
    }
}
