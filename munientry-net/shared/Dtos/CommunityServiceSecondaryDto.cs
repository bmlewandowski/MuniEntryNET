using System;
using System.ComponentModel.DataAnnotations;

namespace Munientry.Shared.Dtos
{
    public class CommunityServiceSecondaryDto
    {
        [Required]
        public bool CommunityServiceOrdered { get; set; }

        public string? CommunityServiceHours { get; set; }
        public string? CommunityServiceDaysToComplete { get; set; }
        public DateTime? CommunityServiceDueDate { get; set; }
        public bool LicenseSuspensionOrdered { get; set; }
        public string? LicenseSuspensionLength { get; set; }
        public bool FingerprintingOrdered { get; set; }
        public bool VictimNotificationOrdered { get; set; }
        public bool ImmobilizeImpoundOrdered { get; set; }
        public string? OtherConditions { get; set; }
        public string? JudicialOfficerFirstName { get; set; }
        public string? JudicialOfficerLastName { get; set; }
        public string? JudicialOfficerType { get; set; }
    }
}
