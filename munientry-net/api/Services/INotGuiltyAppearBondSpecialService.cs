using System.Threading.Tasks;
using Munientry.Api.Data;

namespace Munientry.Api.Services;

public interface INotGuiltyAppearBondSpecialService
{
    Task SaveToDatabaseAsync(NotGuiltyAppearBondSpecialDto dto);
}
