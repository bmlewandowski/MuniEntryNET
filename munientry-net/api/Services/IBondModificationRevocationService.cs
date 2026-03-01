using System.Threading.Tasks;
using Munientry.Api.Data;

namespace Munientry.Api.Services;

public interface IBondModificationRevocationService
{
    Task SaveToDatabaseAsync(BondModificationRevocationDto dto);
}
