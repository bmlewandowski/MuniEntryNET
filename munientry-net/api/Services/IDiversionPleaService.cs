using System.Threading.Tasks;
using Munientry.Api.Data;

namespace Munientry.Api.Services;

public interface IDiversionPleaService
{
    Task AddDiversionPleaAsync(DiversionPleaDto dto);
}
