using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Munientry.Poc.Api.Data;

namespace Munientry.Api.Services
{
    public class DrivingPrivilegesService
    {
        private readonly string _connectionString;

        public DrivingPrivilegesService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("AuthorityCourt");
        }

        public void InsertDrivingPrivileges(DrivingPrivilegesDto dto)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            var command = new SqlCommand(@"INSERT INTO DrivingPrivileges
                (DefendantFirstName, DefendantLastName, CaseNumber, PleaTrialDate, AppearanceReason, DefenseCounselName, DefenseCounselType, DefenseCounselWaived, Offense, Statute, Degree, Plea, Finding, Fines, FinesSuspended, CourtCosts, AbilityToPay, BalanceDueDate, PayToday, MonthlyPay, CreditForJail, JailTimeCredit, LicenseSuspension, CommunityService, OtherConditions)
                VALUES (@DefendantFirstName, @DefendantLastName, @CaseNumber, @PleaTrialDate, @AppearanceReason, @DefenseCounselName, @DefenseCounselType, @DefenseCounselWaived, @Offense, @Statute, @Degree, @Plea, @Finding, @Fines, @FinesSuspended, @CourtCosts, @AbilityToPay, @BalanceDueDate, @PayToday, @MonthlyPay, @CreditForJail, @JailTimeCredit, @LicenseSuspension, @CommunityService, @OtherConditions)", connection);
            command.Parameters.AddWithValue("@DefendantFirstName", dto.DefendantFirstName);
            command.Parameters.AddWithValue("@DefendantLastName", dto.DefendantLastName);
            command.Parameters.AddWithValue("@CaseNumber", dto.CaseNumber);
            command.Parameters.AddWithValue("@PleaTrialDate", (object?)dto.PleaTrialDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@AppearanceReason", (object?)dto.AppearanceReason ?? DBNull.Value);
            command.Parameters.AddWithValue("@DefenseCounselName", (object?)dto.DefenseCounselName ?? DBNull.Value);
            command.Parameters.AddWithValue("@DefenseCounselType", (object?)dto.DefenseCounselType ?? DBNull.Value);
            command.Parameters.AddWithValue("@DefenseCounselWaived", dto.DefenseCounselWaived);
            command.Parameters.AddWithValue("@Offense", (object?)dto.Offense ?? DBNull.Value);
            command.Parameters.AddWithValue("@Statute", (object?)dto.Statute ?? DBNull.Value);
            command.Parameters.AddWithValue("@Degree", (object?)dto.Degree ?? DBNull.Value);
            command.Parameters.AddWithValue("@Plea", (object?)dto.Plea ?? DBNull.Value);
            command.Parameters.AddWithValue("@Finding", (object?)dto.Finding ?? DBNull.Value);
            command.Parameters.AddWithValue("@Fines", (object?)dto.Fines ?? DBNull.Value);
            command.Parameters.AddWithValue("@FinesSuspended", (object?)dto.FinesSuspended ?? DBNull.Value);
            command.Parameters.AddWithValue("@CourtCosts", (object?)dto.CourtCosts ?? DBNull.Value);
            command.Parameters.AddWithValue("@AbilityToPay", (object?)dto.AbilityToPay ?? DBNull.Value);
            command.Parameters.AddWithValue("@BalanceDueDate", (object?)dto.BalanceDueDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@PayToday", (object?)dto.PayToday ?? DBNull.Value);
            command.Parameters.AddWithValue("@MonthlyPay", (object?)dto.MonthlyPay ?? DBNull.Value);
            command.Parameters.AddWithValue("@CreditForJail", dto.CreditForJail);
            command.Parameters.AddWithValue("@JailTimeCredit", (object?)dto.JailTimeCredit ?? DBNull.Value);
            command.Parameters.AddWithValue("@LicenseSuspension", dto.LicenseSuspension);
            command.Parameters.AddWithValue("@CommunityService", dto.CommunityService);
            command.Parameters.AddWithValue("@OtherConditions", dto.OtherConditions);
            command.ExecuteNonQuery();
        }
    }
}
