using System.Threading.Tasks;
using Munientry.Shared.Dtos;

namespace Munientry.Api.Services
{
    public interface IDrivingCaseService
    {
        Task<DrivingCaseInfoDto?> GetDrivingCaseInfoAsync(string caseNumber);
    }
}
