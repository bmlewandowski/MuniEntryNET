using System.ComponentModel.DataAnnotations;

namespace Munientry.Poc.Client.Shared.Models
{
	public class JurorPaymentDto
	{
		[Required]
		public string? CaseNumber { get; set; }
		[Required]
		public DateTime EntryDate { get; set; }
		[Required]
		public string? DefendantFirstName { get; set; }
		[Required]
		public string? DefendantLastName { get; set; }
		[Required]
		public DateTime TrialDate { get; set; }
		[Required]
		public string? TrialLength { get; set; }
		[Required]
		public int JurorsReported { get; set; }
		[Required]
		public int JurorsSeated { get; set; }
		[Required]
		public int JurorsNotSeated { get; set; }
		[Required]
		public int JurorsPayNotSeated { get; set; }
		[Required]
		public int JurorsPaySeated { get; set; }
		[Required]
		public int JuryPanelTotalPay { get; set; }
	}
}
