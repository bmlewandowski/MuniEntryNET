using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Munientry.Poc.Api.Data;

namespace Munientry.Poc.Api.Services
{
    public class NotGuiltyPleaService
    {
        private readonly IConfiguration _config;
        public NotGuiltyPleaService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SaveToDatabaseAsync(NotGuiltyPleaDto dto)
        {
            var connStr = _config.GetConnectionString("AuthorityCourt");
            using var conn = new SqlConnection(connStr);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO not_guilty_plea (
                defendant_first_name, defendant_last_name, case_number, defense_counsel_name, defense_counsel_type, defense_counsel_waived, appearance_reason, plea_date, charges, court_costs, ability_to_pay, balance_due_date, pay_today, monthly_pay, credit_for_jail, jail_time_credit_days, fra_in_file, fra_in_court, license_suspension, community_service, other_conditions, bond_type, bond_amount, no_contact, no_alcohol_drugs, alcohol_drugs_assessment, mental_health_assessment, fingerprint_in_court, specialized_docket, specialized_docket_type, monitoring, monitoring_type, comply_protection_order, public_safety_suspension
            ) VALUES (
                @DefendantFirstName, @DefendantLastName, @CaseNumber, @DefenseCounselName, @DefenseCounselType, @DefenseCounselWaived, @AppearanceReason, @PleaDate, @Charges, @CourtCosts, @AbilityToPay, @BalanceDueDate, @PayToday, @MonthlyPay, @CreditForJail, @JailTimeCreditDays, @FraInFile, @FraInCourt, @LicenseSuspension, @CommunityService, @OtherConditions, @BondType, @BondAmount, @NoContact, @NoAlcoholDrugs, @AlcoholDrugsAssessment, @MentalHealthAssessment, @FingerprintInCourt, @SpecializedDocket, @SpecializedDocketType, @Monitoring, @MonitoringType, @ComplyProtectionOrder, @PublicSafetySuspension
            );";
            cmd.Parameters.AddWithValue("@DefendantFirstName", dto.DefendantFirstName ?? "");
            cmd.Parameters.AddWithValue("@DefendantLastName", dto.DefendantLastName ?? "");
            cmd.Parameters.AddWithValue("@CaseNumber", dto.CaseNumber ?? "");
            cmd.Parameters.AddWithValue("@DefenseCounselName", dto.DefenseCounselName ?? "");
            cmd.Parameters.AddWithValue("@DefenseCounselType", dto.DefenseCounselType ?? "");
            cmd.Parameters.AddWithValue("@DefenseCounselWaived", dto.DefenseCounselWaived);
            cmd.Parameters.AddWithValue("@AppearanceReason", dto.AppearanceReason ?? "");
            cmd.Parameters.AddWithValue("@PleaDate", (object?)dto.PleaDate ?? System.DBNull.Value);
            cmd.Parameters.AddWithValue("@Charges", dto.Charges ?? "");
            cmd.Parameters.AddWithValue("@CourtCosts", dto.CourtCosts ?? "");
            cmd.Parameters.AddWithValue("@AbilityToPay", dto.AbilityToPay ?? "");
            cmd.Parameters.AddWithValue("@BalanceDueDate", (object?)dto.BalanceDueDate ?? System.DBNull.Value);
            cmd.Parameters.AddWithValue("@PayToday", dto.PayToday ?? "");
            cmd.Parameters.AddWithValue("@MonthlyPay", dto.MonthlyPay ?? "");
            cmd.Parameters.AddWithValue("@CreditForJail", dto.CreditForJail);
            cmd.Parameters.AddWithValue("@JailTimeCreditDays", dto.JailTimeCreditDays ?? "");
            cmd.Parameters.AddWithValue("@FraInFile", dto.FraInFile ?? "");
            cmd.Parameters.AddWithValue("@FraInCourt", dto.FraInCourt ?? "");
            cmd.Parameters.AddWithValue("@LicenseSuspension", dto.LicenseSuspension);
            cmd.Parameters.AddWithValue("@CommunityService", dto.CommunityService);
            cmd.Parameters.AddWithValue("@OtherConditions", dto.OtherConditions);
            cmd.Parameters.AddWithValue("@BondType", dto.BondType ?? "");
            cmd.Parameters.AddWithValue("@BondAmount", dto.BondAmount ?? "");
            cmd.Parameters.AddWithValue("@NoContact", dto.NoContact);
            cmd.Parameters.AddWithValue("@NoAlcoholDrugs", dto.NoAlcoholDrugs);
            cmd.Parameters.AddWithValue("@AlcoholDrugsAssessment", dto.AlcoholDrugsAssessment);
            cmd.Parameters.AddWithValue("@MentalHealthAssessment", dto.MentalHealthAssessment);
            cmd.Parameters.AddWithValue("@FingerprintInCourt", dto.FingerprintInCourt);
            cmd.Parameters.AddWithValue("@SpecializedDocket", dto.SpecializedDocket);
            cmd.Parameters.AddWithValue("@SpecializedDocketType", dto.SpecializedDocketType ?? "");
            cmd.Parameters.AddWithValue("@Monitoring", dto.Monitoring);
            cmd.Parameters.AddWithValue("@MonitoringType", dto.MonitoringType ?? "");
            cmd.Parameters.AddWithValue("@ComplyProtectionOrder", dto.ComplyProtectionOrder);
            cmd.Parameters.AddWithValue("@PublicSafetySuspension", dto.PublicSafetySuspension);
            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
