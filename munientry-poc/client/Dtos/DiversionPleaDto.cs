using System;
using System.ComponentModel.DataAnnotations;

namespace Munientry.Poc.Client.Dtos
{
    public class DiversionPleaDto
    {
        [Required]
        public string DefendantFirstName { get; set; }
        [Required]
        public string DefendantLastName { get; set; }
        [Required]
        public string CaseNumber { get; set; }
        [Required]
        public DateTime? Date { get; set; }
        public string DefenseCounselName { get; set; }
        public string DefenseCounselType { get; set; }
        public bool DefenseCounselWaived { get; set; }
        public string AppearanceReason { get; set; }
        public string Charges { get; set; }
        public string DiversionType { get; set; }
        public DateTime? DiversionCompletionDate { get; set; }
        public DateTime? DiversionFinePayDate { get; set; }
        public bool Probation { get; set; }
        public bool PayRestitution { get; set; }
        public string PayRestitutionTo { get; set; }
        public decimal? PayRestitutionAmount { get; set; }
        public bool OtherConditions { get; set; }
        public string OtherConditionsText { get; set; }
    }
}
