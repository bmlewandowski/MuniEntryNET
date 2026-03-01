using System.Threading.Tasks;
using Munientry.Api.Data;

namespace Munientry.Api.Services;

public interface ICommunityServiceSecondaryService
{
    Task SaveToDatabaseAsync(CommunityServiceSecondaryDto dto);
}
