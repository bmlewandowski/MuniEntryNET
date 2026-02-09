// DTO created to match API for Community Control Terms
// TODO: Copy fields from api/Data/CommunityControlTermsDto.cs

public class CommunityControlTermsDto
{
    public string? DefendantFirstName { get; set; }
    public string? DefendantLastName { get; set; }
    public string? CaseNumber { get; set; }
    public string? TermOfControl { get; set; }
    public string? ReportFrequency { get; set; }
    public string? JailReportTime { get; set; }
    public string? NoContactWith { get; set; }
    public string? SpecializedDocketType { get; set; }
}
