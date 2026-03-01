using System.Threading.Tasks;
using Munientry.Api.Data;

namespace Munientry.Api.Services;

public interface ITrialToCourtNoticeService
{
    Task SaveToDatabaseAsync(TrialToCourtNoticeDto dto);
}
