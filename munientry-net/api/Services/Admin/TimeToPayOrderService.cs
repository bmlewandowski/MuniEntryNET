using Munientry.Shared.Dtos;

namespace Munientry.Api.Services;

public class TimeToPayOrderService : DocxServiceBase<TimeToPayOrderDto>, ITimeToPayOrderService
{
    protected override string TemplatePath { get; } =
        TemplateResolver.Get("Time_To_Pay_Template.docx");

    protected override Dictionary<string, object> BuildTokens(TimeToPayOrderDto dto) => new()
    {
        ["case_number"]                   = dto.CaseNumber ?? "",
        ["entry_date"]                    = dto.EntryDate.ToString("MMMM dd, yyyy"),
        ["defendant.first_name"]          = dto.DefendantFirstName ?? "",
        ["defendant.last_name"]           = dto.DefendantLastName ?? "",
        ["appearance_date"]               = dto.AppearanceDate.ToString("MMMM dd, yyyy"),
        ["judicial_officer.first_name"]   = dto.JudicialOfficerFirstName ?? "",
        ["judicial_officer.last_name"]    = dto.JudicialOfficerLastName ?? "",
        ["judicial_officer.officer_type"] = dto.JudicialOfficerType ?? "",
    };
}
