using System.Threading.Tasks;
using Munientry.Api.Data;

namespace Munientry.Api.Services;

public interface IDenyPrivilegesPermitRetestService
{
    Task SaveToDatabaseAsync(DenyPrivilegesPermitRetestDto dto);
}
