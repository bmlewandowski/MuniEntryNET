// DTO created to match API for Bond Hearing
// TODO: Copy fields from api/Data/BondHearingDto.cs

public class BondHearingDto
{
    public string? DefendantFirstName { get; set; }
    public string? DefendantLastName { get; set; }
    public string? CaseNumber { get; set; }
    public string? BondType { get; set; }
    public string? BondAmount { get; set; }
}
