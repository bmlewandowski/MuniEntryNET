using Munientry.Shared.Dtos;

namespace Munientry.Api.Services;

public class DrivingPrivilegesService : DocxServiceBase<DrivingPrivilegesDto>, IDrivingPrivilegesService
{
    protected override string TemplatePath { get; } =
        TemplateResolver.Get("Driving_Privileges_Template.docx");

    protected override Dictionary<string, object> BuildTokens(DrivingPrivilegesDto dto) => new()
    {
        ["case_number"]                     = dto.CaseNumber ?? "",
        ["defendant.first_name"]            = dto.DefendantFirstName ?? "",
        ["defendant.last_name"]             = dto.DefendantLastName ?? "",
        ["defendant.license_number"]        = "",
        ["defendant.birth_date"]            = "",
        ["defendant.address"]               = "",
        ["defendant.city"]                  = "",
        ["defendant.state"]                 = "",
        ["defendant.zipcode"]               = "",
        ["suspension_type"]                 = "",
        ["suspension_start_date"]           = "",
        ["suspension_end_date"]             = "",
        ["bmv_cases"]                       = "",
        ["related_traffic_case_number"]     = "",
        ["employer.privileges_type"]        = "",
        ["employer.name"]                   = "",
        ["employer.address"]                = "",
        ["employer.city"]                   = "",
        ["employer.state"]                  = "",
        ["employer.zipcode"]                = "",
        ["employer.driving_days"]           = "",
        ["employer.driving_hours"]          = "",
        ["employer.other_conditions"]       = "",
        ["additional_information_text"]     = "",
        ["judicial_officer.first_name"]     = dto.JudicialOfficerFirstName ?? "",
        ["judicial_officer.last_name"]      = dto.JudicialOfficerLastName ?? "",
        ["judicial_officer.officer_type"]   = dto.JudicialOfficerType ?? "",
    };
}
