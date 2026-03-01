using Munientry.Api.Data;

namespace Munientry.Api.Services;

public interface ILeapAdmissionAlreadyValidService
{
    void Save(LeapAdmissionAlreadyValidDto dto);
}
