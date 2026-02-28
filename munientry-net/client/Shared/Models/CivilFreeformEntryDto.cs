using System;
using System.ComponentModel.DataAnnotations;

namespace Munientry.Poc.Client.Shared.Models
{
	public class CivilFreeformEntryDto
	{
		[Required]
		public DateTime EntryDate { get; set; }
		[Required]
		public string? Plaintiff { get; set; }
		[Required]
		public string? Defendant { get; set; }
		[Required]
		public string? CaseNumber { get; set; }
		public string? AppearanceReason { get; set; }
		[Required]
		public string? EntryContent { get; set; }
	}
}
