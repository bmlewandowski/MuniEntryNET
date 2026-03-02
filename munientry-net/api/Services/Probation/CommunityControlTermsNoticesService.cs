using Munientry.Shared.Dtos;

namespace Munientry.Api.Services;

public class CommunityControlTermsNoticesService : DocxServiceBase<CommunityControlTermsNoticesDto>, ICommunityControlTermsNoticesService
{
    protected override string TemplatePath { get; } =
        TemplateResolver.Get("Notice_CC_Violation_Template.docx");

    protected override Dictionary<string, object> BuildTokens(CommunityControlTermsNoticesDto dto) => new()
    {
        ["case_number"]                   = dto.CaseNumber ?? "",
        ["defendant.first_name"]          = dto.DefendantFirstName ?? "",
        ["defendant.last_name"]           = dto.DefendantLastName ?? "",
        ["entry_date"]                    = dto.EntryDate.ToString("MMMM dd, yyyy"),
        ["slated_date"]                   = "",
        ["violation_hearing_date"]        = dto.HearingDate.ToString("MMMM dd, yyyy"),
        ["violation_hearing_time"]        = dto.HearingTime ?? "",
        ["judicial_officer.first_name"]   = dto.JudicialOfficerFirstName ?? "",
        ["judicial_officer.last_name"]    = dto.JudicialOfficerLastName ?? "",
        ["judicial_officer.officer_type"] = dto.JudicialOfficerType ?? "",
    };
}
