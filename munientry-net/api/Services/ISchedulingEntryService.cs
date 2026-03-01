using System.Threading.Tasks;
using Munientry.Api.Data;

namespace Munientry.Api.Services;

public interface ISchedulingEntryService
{
    Task<bool> CreateSchedulingEntryAsync(SchedulingEntryDto dto);
}
