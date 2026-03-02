using System.ComponentModel.DataAnnotations;

namespace Munientry.Shared.Dtos
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
        public string? JudicialOfficerFirstName { get; set; }
        public string? JudicialOfficerLastName { get; set; }
        public string? JudicialOfficerType { get; set; }
    }
}
