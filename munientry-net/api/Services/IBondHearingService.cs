using System.Threading.Tasks;
using Munientry.Api.Data;

namespace Munientry.Api.Services;

public interface IBondHearingService
{
    Task SaveToDatabaseAsync(BondHearingDto dto);
}
