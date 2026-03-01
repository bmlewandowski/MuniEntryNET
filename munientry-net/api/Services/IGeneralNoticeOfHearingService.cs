using System.Threading.Tasks;
using Munientry.Api.Data;

namespace Munientry.Api.Services;

public interface IGeneralNoticeOfHearingService
{
    Task SaveToDatabaseAsync(GeneralNoticeOfHearingDto dto);
}
