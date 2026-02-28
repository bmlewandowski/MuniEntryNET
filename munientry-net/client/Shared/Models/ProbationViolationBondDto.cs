using System.ComponentModel.DataAnnotations;

namespace Munientry.Poc.Client.Shared.Models
{
	public class ProbationViolationBondDto
	{
		[Required]
		public string? DefendantFirstName { get; set; }
		[Required]
		public string? DefendantLastName { get; set; }
		[Required]
		public string? CaseNumber { get; set; }
		public string? AppearanceReason { get; set; }
		public string? DefenseCounselName { get; set; }
		public string? DefenseCounselType { get; set; }
		public bool DefenseCounselWaived { get; set; }
		public string? ProbableCauseFinding { get; set; }
		public string? BondType { get; set; }
		public string? BondAmount { get; set; }
		public bool NoAlcoholDrugs { get; set; }
		public bool Monitoring { get; set; }
		public string? MonitoringType { get; set; }
		public bool ComplyProtectionOrder { get; set; }
		public bool OtherConditions { get; set; }
	}
}
