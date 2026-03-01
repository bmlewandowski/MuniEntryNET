using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Munientry.Api.Data;

namespace Munientry.Api.Services
{
    public class JailCcPleaService : IJailCcPleaService
    {
        private readonly IConfiguration _config;
        public JailCcPleaService(IConfiguration config)
        {
            _config = config;
        }

        public async Task AddJailCcPleaAsync(JailCcPleaDto dto)
        {
            var connStr = _config.GetConnectionString("AuthorityCourt");
            using var conn = new SqlConnection(connStr);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO jail_cc_plea (
                defendant_first_name, defendant_last_name, case_number, date, defense_counsel_name, defense_counsel_type, defense_counsel_waived, appearance_reason, charges, offense_of_violence, victim_statements, victim_notification, impoundment, community_control, license_suspension, community_service, other_conditions, additional_conditions, jail_time_credit_days, jail_time_credit_apply, in_jail, companion_cases, companion_cases_sentence, jail_reporting_terms, add_jail_report_date, pay_today, monthly_pay, court_costs, time_to_pay, due_date, fra_in_file, fra_in_court, distracted_driving
            ) VALUES (
                @DefendantFirstName, @DefendantLastName, @CaseNumber, @Date, @DefenseCounselName, @DefenseCounselType, @DefenseCounselWaived, @AppearanceReason, @Charges, @OffenseOfViolence, @VictimStatements, @VictimNotification, @Impoundment, @CommunityControl, @LicenseSuspension, @CommunityService, @OtherConditions, @AdditionalConditions, @JailTimeCreditDays, @JailTimeCreditApply, @InJail, @CompanionCases, @CompanionCasesSentence, @JailReportingTerms, @AddJailReportDate, @PayToday, @MonthlyPay, @CourtCosts, @TimeToPay, @DueDate, @FraInFile, @FraInCourt, @DistractedDriving
            );";
            cmd.Parameters.AddWithValue("@DefendantFirstName", dto.DefendantFirstName ?? "");
            cmd.Parameters.AddWithValue("@DefendantLastName", dto.DefendantLastName ?? "");
            cmd.Parameters.AddWithValue("@CaseNumber", dto.CaseNumber ?? "");
            cmd.Parameters.AddWithValue("@Date", (object?)dto.Date ?? System.DBNull.Value);
            cmd.Parameters.AddWithValue("@DefenseCounselName", dto.DefenseCounselName ?? "");
            cmd.Parameters.AddWithValue("@DefenseCounselType", dto.DefenseCounselType ?? "");
            cmd.Parameters.AddWithValue("@DefenseCounselWaived", dto.DefenseCounselWaived);
            cmd.Parameters.AddWithValue("@AppearanceReason", dto.AppearanceReason ?? "");
            cmd.Parameters.AddWithValue("@Charges", dto.Charges ?? "");
            cmd.Parameters.AddWithValue("@OffenseOfViolence", dto.OffenseOfViolence);
            cmd.Parameters.AddWithValue("@VictimStatements", dto.VictimStatements);
            cmd.Parameters.AddWithValue("@VictimNotification", dto.VictimNotification);
            cmd.Parameters.AddWithValue("@Impoundment", dto.Impoundment);
            cmd.Parameters.AddWithValue("@CommunityControl", dto.CommunityControl);
            cmd.Parameters.AddWithValue("@LicenseSuspension", dto.LicenseSuspension);
            cmd.Parameters.AddWithValue("@CommunityService", dto.CommunityService);
            cmd.Parameters.AddWithValue("@OtherConditions", dto.OtherConditions);
            cmd.Parameters.AddWithValue("@AdditionalConditions", dto.AdditionalConditions);
            cmd.Parameters.AddWithValue("@JailTimeCreditDays", (object?)dto.JailTimeCreditDays ?? System.DBNull.Value);
            cmd.Parameters.AddWithValue("@JailTimeCreditApply", dto.JailTimeCreditApply ?? "");
            cmd.Parameters.AddWithValue("@InJail", dto.InJail ?? "");
            cmd.Parameters.AddWithValue("@CompanionCases", dto.CompanionCases ?? "");
            cmd.Parameters.AddWithValue("@CompanionCasesSentence", dto.CompanionCasesSentence ?? "");
            cmd.Parameters.AddWithValue("@JailReportingTerms", dto.JailReportingTerms);
            cmd.Parameters.AddWithValue("@AddJailReportDate", dto.AddJailReportDate);
            cmd.Parameters.AddWithValue("@PayToday", (object?)dto.PayToday ?? System.DBNull.Value);
            cmd.Parameters.AddWithValue("@MonthlyPay", (object?)dto.MonthlyPay ?? System.DBNull.Value);
            cmd.Parameters.AddWithValue("@CourtCosts", dto.CourtCosts ?? "");
            cmd.Parameters.AddWithValue("@TimeToPay", dto.TimeToPay ?? "");
            cmd.Parameters.AddWithValue("@DueDate", (object?)dto.DueDate ?? System.DBNull.Value);
            cmd.Parameters.AddWithValue("@FraInFile", dto.FraInFile ?? "");
            cmd.Parameters.AddWithValue("@FraInCourt", dto.FraInCourt ?? "");
            cmd.Parameters.AddWithValue("@DistractedDriving", dto.DistractedDriving);
            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
