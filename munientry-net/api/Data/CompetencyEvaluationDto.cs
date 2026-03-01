using System.ComponentModel.DataAnnotations;

namespace Munientry.Api.Data
{
    public class CompetencyEvaluationDto
    {
        [Required]
        public string? DefendantFirstName { get; set; }
        [Required]
        public string? DefendantLastName { get; set; }
        [Required]
        public string? CaseNumber { get; set; }
        public DateTime? EntryDate { get; set; }
        public string? DefenseCounselName { get; set; }
        public string? DefenseCounselType { get; set; }
        public bool CounselWaived { get; set; }
        public string? EvaluatorName { get; set; }
        public DateTime? EvaluationDate { get; set; }
        public DateTime? CompetencyHearingDate { get; set; }
        public string? CompetencyHearingType { get; set; }
    }
}
