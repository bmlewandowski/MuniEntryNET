namespace Munientry.Client.Shared.Models
{
    /// <summary>
    /// Client-side DTO for the result of GET /api/case/search/{caseNumber}.
    /// Maps the [reports].[DMCMuniEntryCaseSearch] stored procedure output.
    /// Each instance represents one charge (SubCase) on the case.
    /// Mirrors Munientry.Api.Data.CaseSearchResultDto on the API side.
    /// </summary>
    public class CaseSearchResultDto
    {
        public string? SubCaseNumber { get; set; }
        public string? CaseNumber { get; set; }
        public int SubCaseId { get; set; }
        public string? Charge { get; set; }
        public int ViolationId { get; set; }
        public int ViolationDetailId { get; set; }
        public string? Statute { get; set; }
        public string? DegreeCode { get; set; }
        public string? DefFirstName { get; set; }
        public string? DefLastName { get; set; }
        public bool MovingBool { get; set; }
        public DateTime? ViolationDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? FraInFile { get; set; }
        public string? DefenseCounsel { get; set; }
        public bool PubDef { get; set; }
    }
}
