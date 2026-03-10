using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Munientry.Api.Options;
using Munientry.Shared.Dtos;
using Polly;
using Polly.Registry;

namespace Munientry.Api.Services
{
    public class DrivingCaseService : SqlServiceBase, IDrivingCaseService
    {
        public DrivingCaseService(
            IOptions<AuthorityCourtOptions> options,
            ResiliencePipelineProvider<string> pipelineProvider)
            : base(options, pipelineProvider) { }

        public Task<DrivingCaseInfoDto?> GetDrivingCaseInfoAsync(string caseNumber) =>
            ExecuteSpSingleAsync(
                "reports.DMCMuniEntryDrivingCaseSearch",
                cmd => cmd.Parameters.Add("@CaseNumber", SqlDbType.NVarChar, 20).Value = caseNumber,
                reader => new DrivingCaseInfoDto
                {
                    CaseNumber       = reader["CaseNumber"]?.ToString(),
                    DefLastName      = CourtDataCleaner.CleanLastName(reader["DefLastName"]?.ToString()),
                    DefFirstName     = reader["DefFirstName"]?.ToString(),
                    DefMiddleName    = reader["DefMiddleName"]?.ToString(),
                    DefSuffix        = reader["DefSuffix"]?.ToString(),
                    DefBirthDate     = reader["DefBirthDate"]?.ToString(),
                    DefCity          = reader["DefCity"]?.ToString(),
                    DefState         = reader["DefState"]?.ToString(),
                    DefZipcode       = reader["DefZipcode"]?.ToString(),
                    DefAddress       = reader["DefAddress"]?.ToString(),
                    DefLicenseNumber = reader["DefLicenseNumber"]?.ToString(),
                    CaseAddress      = reader["CaseAddress"]?.ToString(),
                    CaseCity         = reader["CaseCity"]?.ToString(),
                    CaseState        = reader["CaseState"]?.ToString(),
                    CaseZipcode      = reader["CaseZipcode"]?.ToString(),
                });
    }
}
