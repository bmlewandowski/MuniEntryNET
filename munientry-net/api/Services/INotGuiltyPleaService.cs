using System.Threading.Tasks;
using Munientry.Api.Data;

namespace Munientry.Api.Services;

public interface INotGuiltyPleaService
{
    Task SaveToDatabaseAsync(NotGuiltyPleaDto dto);
}
