using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Munientry.Api.Data;

namespace Munientry.Api.Services
{
    public class DrivingCaseService : IDrivingCaseService
    {
        private readonly string _connectionString;

        public DrivingCaseService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("AuthorityCourt")!;
        }

        public async Task<DrivingCaseInfoDto?> GetDrivingCaseInfoAsync(string caseNumber)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("reports.DMCMuniEntryDrivingCaseSearch", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@CaseNumber", caseNumber);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new DrivingCaseInfoDto
                {
                    CaseNumber = reader["CaseNumber"]?.ToString(),
                    DefLastName = reader["DefLastName"]?.ToString(),
                    DefFirstName = reader["DefFirstName"]?.ToString(),
                    DefMiddleName = reader["DefMiddleName"]?.ToString(),
                    DefSuffix = reader["DefSuffix"]?.ToString(),
                    DefBirthDate = reader["DefBirthDate"]?.ToString(),
                    DefCity = reader["DefCity"]?.ToString(),
                    DefState = reader["DefState"]?.ToString(),
                    DefZipcode = reader["DefZipcode"]?.ToString(),
                    DefAddress = reader["DefAddress"]?.ToString(),
                    DefLicenseNumber = reader["DefLicenseNumber"]?.ToString(),
                    CaseAddress = reader["CaseAddress"]?.ToString(),
                    CaseCity = reader["CaseCity"]?.ToString(),
                    CaseState = reader["CaseState"]?.ToString(),
                    CaseZipcode = reader["CaseZipcode"]?.ToString(),
                };
            }
            return null;
        }
    }
}
