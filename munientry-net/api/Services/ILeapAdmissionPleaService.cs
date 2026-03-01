using System.Threading.Tasks;
using Munientry.Api.Data;

namespace Munientry.Api.Services;

public interface ILeapAdmissionPleaService
{
    Task<int> CreateLeapAdmissionPleaEntryAsync(LeapAdmissionPleaDto dto);
}
