using System.Threading.Tasks;
using Munientry.Api.Data;

namespace Munientry.Api.Services;

public interface IFinalJuryNoticeService
{
    Task SaveToDatabaseAsync(FinalJuryNoticeDto dto);
}
