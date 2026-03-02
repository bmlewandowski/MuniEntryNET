using Munientry.Shared.Dtos;

namespace Munientry.Api.Services;

public class LeapValidSentencingService : DocxServiceBase<LeapValidSentencingDto>, ILeapValidSentencingService
{
    protected override string TemplatePath { get; } =
        TemplateResolver.Get("Leap_Admission_Plea_Valid_Template.docx");

    protected override Dictionary<string, object> BuildTokens(LeapValidSentencingDto dto) => new()
    {
        ["case_number"]                              = dto.CaseNumber ?? "",
        ["defendant.first_name"]                     = dto.DefendantFirstName ?? "",
        ["defendant.last_name"]                      = dto.DefendantLastName ?? "",
        ["defense_counsel"]                          = dto.DefenseCounselName ?? "",
        ["defense_counsel_type"]                     = dto.DefenseCounselType ?? "",
        ["plea_trial_date"]                          = dto.Date?.ToString("MMMM dd, yyyy") ?? "",
        ["amend_offense_details.motion_disposition"] = "",
        ["judicial_officer.first_name"]              = dto.JudicialOfficerFirstName ?? "",
        ["judicial_officer.last_name"]               = dto.JudicialOfficerLastName ?? "",
        ["judicial_officer.officer_type"]            = dto.JudicialOfficerType ?? "",
        ["charges_list"]                             = ChargeListBuilder.Build(
            dto.ChargeItems, dto.Charges, dto.ChargeStatute, dto.ChargeDegree, dto.ChargePlea),
    };
}
