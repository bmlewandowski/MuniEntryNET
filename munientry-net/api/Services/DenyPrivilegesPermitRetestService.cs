using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Munientry.Api.Data;
using Microsoft.Data.SqlClient;

namespace Munientry.Api.Services
{
    public class DenyPrivilegesPermitRetestService : IDenyPrivilegesPermitRetestService
    {
        private readonly IConfiguration _config;
        public DenyPrivilegesPermitRetestService(IConfiguration config)
        {
            _config = config;
        }
        public async Task SaveToDatabaseAsync(DenyPrivilegesPermitRetestDto dto)
        {
            var connStr = _config.GetConnectionString("AuthorityCourt");
            using var conn = new SqlConnection(connStr);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO deny_privileges_entries (
                case_number, entry_date, def_first_name, def_last_name, entry_type, hard_time_not_passed, permanent_id_card, out_of_state_license, petition_incomplete, no_insurance, no_employer_info, no_jurisdiction, no_pay_plan, prohibited_activities, license_expiration_date, privileges_grant_date, nufc_date
            ) VALUES (
                @CaseNumber, @EntryDate, @DefFirstName, @DefLastName, @EntryType, @HardTimeNotPassed, @PermanentIdCard, @OutOfStateLicense, @PetitionIncomplete, @NoInsurance, @NoEmployerInfo, @NoJurisdiction, @NoPayPlan, @ProhibitedActivities, @LicenseExpirationDate, @PrivilegesGrantDate, @NufcDate
            );";
            cmd.Parameters.AddWithValue("@CaseNumber", dto.CaseNumber ?? "");
            cmd.Parameters.AddWithValue("@EntryDate", dto.EntryDate ?? (object)System.DBNull.Value);
            cmd.Parameters.AddWithValue("@DefFirstName", dto.DefendantFirstName ?? "");
            cmd.Parameters.AddWithValue("@DefLastName", dto.DefendantLastName ?? "");
            cmd.Parameters.AddWithValue("@EntryType", dto.EntryType ?? "");
            cmd.Parameters.AddWithValue("@HardTimeNotPassed", dto.HardTimeNotPassed);
            cmd.Parameters.AddWithValue("@PermanentIdCard", dto.PermanentIdCard);
            cmd.Parameters.AddWithValue("@OutOfStateLicense", dto.OutOfStateLicense);
            cmd.Parameters.AddWithValue("@PetitionIncomplete", dto.PetitionIncomplete);
            cmd.Parameters.AddWithValue("@NoInsurance", dto.NoInsurance);
            cmd.Parameters.AddWithValue("@NoEmployerInfo", dto.NoEmployerInfo);
            cmd.Parameters.AddWithValue("@NoJurisdiction", dto.NoJurisdiction);
            cmd.Parameters.AddWithValue("@NoPayPlan", dto.NoPayPlan);
            cmd.Parameters.AddWithValue("@ProhibitedActivities", dto.ProhibitedActivities);
            cmd.Parameters.AddWithValue("@LicenseExpirationDate", dto.LicenseExpirationDate ?? (object)System.DBNull.Value);
            cmd.Parameters.AddWithValue("@PrivilegesGrantDate", dto.PrivilegesGrantDate ?? (object)System.DBNull.Value);
            cmd.Parameters.AddWithValue("@NufcDate", dto.NufcDate ?? (object)System.DBNull.Value);
            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
