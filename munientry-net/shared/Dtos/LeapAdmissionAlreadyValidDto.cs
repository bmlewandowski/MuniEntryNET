using System;
using System.ComponentModel.DataAnnotations;
using Munientry.Shared.Validation;

namespace Munientry.Shared.Dtos
{
    public class LeapAdmissionAlreadyValidDto
    {
        [Required]
        public string DefendantFirstName { get; set; } = string.Empty;
        [Required]
        public string DefendantLastName { get; set; } = string.Empty;
        [Required]
        public string CaseNumber { get; set; } = string.Empty;
        public string? DefenseCounselName { get; set; }
        public string? DefenseCounselType { get; set; }
        public bool DefenseCounselWaived { get; set; }
        public string? AppearanceReason { get; set; }
        [Required]
        [PastDate]
        public DateTime? AdmissionDate { get; set; }
        public string? Charges { get; set; }
        public string? ChargeStatute { get; set; }
        public string? ChargeDegree { get; set; }
        public string? ChargePlea { get; set; }
        public string? CourtCosts { get; set; }
        public string? AbilityToPay { get; set; }
        public DateTime? BalanceDueDate { get; set; }
        public string? PayToday { get; set; }
        public string? MonthlyPay { get; set; }
        public bool CreditForJail { get; set; }
        public string? JailTimeCreditDays { get; set; }
        public string? FraInFile { get; set; }
        public string? FraInCourt { get; set; }
        public bool LicenseSuspension { get; set; }
        public bool CommunityService { get; set; }
        public bool OtherConditions { get; set; }

        /// <summary>
        /// All charges for the entry. Drives the <c>{%tc for charge in charges_list %}</c>
        /// table-row loop in the DOCX template. Each item maps to one row.
        /// </summary>
        public List<ChargeItemDto> ChargeItems { get; set; } = new();
        public string? JudicialOfficerFirstName { get; set; }
        public string? JudicialOfficerLastName { get; set; }
        public string? JudicialOfficerType { get; set; }
    }
}