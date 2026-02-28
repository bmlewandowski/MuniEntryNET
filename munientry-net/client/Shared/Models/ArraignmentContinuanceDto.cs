using System;
using System.ComponentModel.DataAnnotations;

namespace munientry_poc.client.Shared.Models
{
    public class ArraignmentContinuanceDto
    {
        [Required]
        public string? DefendantFirstName { get; set; }
        [Required]
        public string? DefendantLastName { get; set; }
        [Required]
        public string? CaseNumber { get; set; }
        public DateTime? PleaTrialDate { get; set; }
        public string? DefenseCounselName { get; set; }
        public string? DefenseCounselType { get; set; }
        public bool CounselWaived { get; set; }
        public string? AppearanceReason { get; set; }
        public DateTime? CurrentArraignmentDate { get; set; }
        public DateTime? NewArraignmentDate { get; set; }
        public string? ContinuanceLength { get; set; }
        public string? ContinuanceReason { get; set; }
    }
}
