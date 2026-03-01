using System.ComponentModel.DataAnnotations;
namespace Munientry.Client.Shared.Models
{
    public class FineOnlyDto
    {
        [Required]
        public string? CaseNumber { get; set; }
        [Required]
        public string? DefendantName { get; set; }
        [Required]
        public string? Charge { get; set; }
        [Range(0, 10000)]
        public decimal? FineAmount { get; set; }
    }
}