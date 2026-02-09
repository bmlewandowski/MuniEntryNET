using System;
using System.ComponentModel.DataAnnotations;

namespace Munientry.Poc.Client.Shared.Models
{
	public class CommunityControlTermsNoticesDto
	{
		[Required]
		public string CaseNumber { get; set; } = null!;
		[Required]
		public DateTime EntryDate { get; set; }
		[Required]
		public string DefendantFirstName { get; set; } = null!;
		[Required]
		public string DefendantLastName { get; set; } = null!;
		[Required]
		public DateTime HearingDate { get; set; }
		[Required]
		public string HearingTime { get; set; } = null!;
		[Required]
		public string AssignedCourtroom { get; set; } = null!;
		public string? DefenseCounselName { get; set; }
		public string? ViolationType { get; set; }
		public bool InterpreterRequired { get; set; }
		public string? LanguageRequired { get; set; }
	}
}
