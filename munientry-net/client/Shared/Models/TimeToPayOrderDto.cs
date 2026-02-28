using System.ComponentModel.DataAnnotations;

namespace Munientry.Poc.Client.Shared.Models
{
	public class TimeToPayOrderDto
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
		public DateTime AppearanceDate { get; set; }
	}
}
