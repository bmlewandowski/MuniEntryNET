using System.Threading.Tasks;
using Munientry.Api.Data;

namespace Munientry.Api.Services;

public interface ILeapValidSentencingService
{
    Task AddLeapValidSentencingAsync(LeapValidSentencingDto dto);
}
