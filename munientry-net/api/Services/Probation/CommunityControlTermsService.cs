using Munientry.Shared.Dtos;

namespace Munientry.Api.Services;

public class CommunityControlTermsService : DocxServiceBase<CommunityControlTermsDto>, ICommunityControlTermsService
{
    protected override string TemplatePath { get; } =
        TemplateResolver.Get("Terms_Of_Community_Control_Template.docx");

    protected override Dictionary<string, object> BuildTokens(CommunityControlTermsDto dto) => new()
    {
        ["case_number"]                   = dto.CaseNumber ?? "",
        ["defendant.first_name"]          = dto.DefendantFirstName ?? "",
        ["defendant.last_name"]           = dto.DefendantLastName ?? "",
        ["entry_date"]                    = dto.EntryDate?.ToString("MMMM dd, yyyy") ?? "",
        ["term_of_control"]               = dto.TermOfControl ?? "",
        ["report_frequency"]              = dto.ReportFrequency ?? "",
        ["jail_days"]                     = dto.JailDaysToServe?.ToString() ?? "",
        ["jail_report_date"]              = dto.JailReportDate?.ToString("MMMM dd, yyyy") ?? "",
        ["jail_report_time"]              = dto.JailReportTime ?? "",
        ["no_contact_with_person"]        = dto.NoContactWith ?? "",
        ["community_service_hours"]       = dto.CommunityServiceHours?.ToString() ?? "",
        ["scram_days"]                    = dto.ScramDays?.ToString() ?? "",
        ["specialized_docket_type"]       = dto.SpecializedDocketType ?? "",
        ["judicial_officer.first_name"]   = dto.JudicialOfficerFirstName ?? "",
        ["judicial_officer.last_name"]    = dto.JudicialOfficerLastName ?? "",
        ["judicial_officer.officer_type"] = dto.JudicialOfficerType ?? "",
    };
}
