using System.Threading.Tasks;
using Munientry.Shared.Dtos;
using Munientry.Api.Services;

namespace Munientry.Api.Tests.Fakes
{
    /// <summary>
    /// In-memory stand-in for <see cref="IDrivingCaseService"/> — no SQL Server required.
    /// </summary>
    internal class FakeDrivingCaseService : IDrivingCaseService
    {
        public const string KnownCaseNumber   = "2026-TRC-050";
        public const string UnknownCaseNumber = "9999-TRC-000";

        public Task<DrivingCaseInfoDto?> GetDrivingCaseInfoAsync(string caseNumber)
        {
            if (caseNumber == KnownCaseNumber)
            {
                return Task.FromResult<DrivingCaseInfoDto?>(new DrivingCaseInfoDto
                {
                    CaseNumber       = KnownCaseNumber,
                    DefLastName      = "Johnson",
                    DefFirstName     = "Robert",
                    DefMiddleName    = "Lee",
                    DefSuffix        = null,
                    DefBirthDate     = "1985-06-15",
                    DefCity          = "Columbus",
                    DefState         = "OH",
                    DefZipcode       = "43215",
                    DefAddress       = "123 Main St",
                    DefLicenseNumber = "OH12345678",
                    CaseAddress      = "I-270 NB MM 35",
                    CaseCity         = "Columbus",
                    CaseState        = "OH",
                    CaseZipcode      = "43215",
                });
            }

            return Task.FromResult<DrivingCaseInfoDto?>(null);
        }
    }
}
