using System.Threading.Tasks;
using Munientry.Api.Data;

namespace Munientry.Api.Services;

public interface IJurorPaymentService
{
    Task SaveToDatabaseAsync(JurorPaymentDto dto);
}
