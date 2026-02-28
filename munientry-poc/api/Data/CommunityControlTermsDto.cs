using System;
using System.ComponentModel.DataAnnotations;

namespace Munientry.Poc.Api.Data
{
    public class CommunityControlTermsDto
    {
        [Required]
        public string? DefendantFirstName { get; set; }
        [Required]
        public string? DefendantLastName { get; set; }
        public string? CaseNumber { get; set; }
        public DateTime? EntryDate { get; set; }
        public string? TermOfControl { get; set; }
        public string? ReportFrequency { get; set; }
        public bool ReportToJail { get; set; }
        public DateTime? JailReportDate { get; set; }
        public string? JailReportTime { get; set; }
        public string? JailDaysToServe { get; set; }
        public bool InterlockVehicles { get; set; }
        public bool NoAlcoholOrdered { get; set; }
        public bool GpsExclusion { get; set; }
        public bool ScramOrdered { get; set; }
        public string? ScramDays { get; set; }
        public bool NoContact { get; set; }
        public string? NoContactWith { get; set; }
        public bool Antitheft { get; set; }
        public bool AlcoholTreatment { get; set; }
        public bool DomesticViolenceProgram { get; set; }
        public bool AngerManagement { get; set; }
        public bool MentalHealthTreatment { get; set; }
        public bool DriverInterventionProgram { get; set; }
        public bool CommunityService { get; set; }
        public string? CommunityServiceHours { get; set; }
        public bool PayRestitution { get; set; }
        public bool SpecializedDocket { get; set; }
        public string? SpecializedDocketType { get; set; }
    }
}
