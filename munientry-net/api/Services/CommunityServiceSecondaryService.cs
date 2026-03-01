using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Munientry.Api.Data;

namespace Munientry.Api.Services
{
    public class CommunityServiceSecondaryService : ICommunityServiceSecondaryService
    {
        private readonly IConfiguration _config;
        public CommunityServiceSecondaryService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SaveToDatabaseAsync(CommunityServiceSecondaryDto dto)
        {
            var connStr = _config.GetConnectionString("AuthorityCourt");
            using var conn = new SqlConnection(connStr);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO community_service_secondary (community_service_ordered, community_service_hours, community_service_days_to_complete, community_service_due_date, license_suspension_ordered, license_suspension_length, fingerprinting_ordered, victim_notification_ordered, immobilize_impound_ordered, other_conditions)
VALUES (@CommunityServiceOrdered, @CommunityServiceHours, @CommunityServiceDaysToComplete, @CommunityServiceDueDate, @LicenseSuspensionOrdered, @LicenseSuspensionLength, @FingerprintingOrdered, @VictimNotificationOrdered, @ImmobilizeImpoundOrdered, @OtherConditions);";
            cmd.Parameters.AddWithValue("@CommunityServiceOrdered", dto.CommunityServiceOrdered);
            cmd.Parameters.AddWithValue("@CommunityServiceHours", dto.CommunityServiceHours ?? "");
            cmd.Parameters.AddWithValue("@CommunityServiceDaysToComplete", dto.CommunityServiceDaysToComplete ?? "");
            cmd.Parameters.AddWithValue("@CommunityServiceDueDate", dto.CommunityServiceDueDate?.ToString("yyyy-MM-dd") ?? (object)System.DBNull.Value);
            cmd.Parameters.AddWithValue("@LicenseSuspensionOrdered", dto.LicenseSuspensionOrdered);
            cmd.Parameters.AddWithValue("@LicenseSuspensionLength", dto.LicenseSuspensionLength ?? "");
            cmd.Parameters.AddWithValue("@FingerprintingOrdered", dto.FingerprintingOrdered);
            cmd.Parameters.AddWithValue("@VictimNotificationOrdered", dto.VictimNotificationOrdered);
            cmd.Parameters.AddWithValue("@ImmobilizeImpoundOrdered", dto.ImmobilizeImpoundOrdered);
            cmd.Parameters.AddWithValue("@OtherConditions", dto.OtherConditions ?? "");
            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
