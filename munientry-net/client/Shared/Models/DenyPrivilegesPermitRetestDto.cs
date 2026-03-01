using System;
using System.ComponentModel.DataAnnotations;

namespace Munientry.Client.Shared.Models
{
    public class DenyPrivilegesPermitRetestDto
    {
        [Required]
        public string? DefendantFirstName { get; set; }
        [Required]
        public string? DefendantLastName { get; set; }
        [Required]
        public string? CaseNumber { get; set; }
        [Required]
        public DateTime? EntryDate { get; set; }
        [Required]
        public EntryType EntryType { get; set; }
        public bool HardTimeNotPassed { get; set; }
        public bool PermanentIdCard { get; set; }
        public bool OutOfStateLicense { get; set; }
        public bool PetitionIncomplete { get; set; }
        public bool NoInsurance { get; set; }
        public bool NoEmployerInfo { get; set; }
        public bool NoJurisdiction { get; set; }
        public bool NoPayPlan { get; set; }
        public bool ProhibitedActivities { get; set; }
        public DateTime? LicenseExpirationDate { get; set; }
        public DateTime? PrivilegesGrantDate { get; set; }
        public DateTime? NufcDate { get; set; }
    }
}
