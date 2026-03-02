using Munientry.Shared.Dtos;

namespace Munientry.Api.Services;

public class DenyPrivilegesPermitRetestService : DocxServiceBase<DenyPrivilegesPermitRetestDto>, IDenyPrivilegesPermitRetestService
{
    protected override string TemplatePath { get; } =
        TemplateResolver.Get("Deny_Privileges_Template.docx");

    protected override Dictionary<string, object> BuildTokens(DenyPrivilegesPermitRetestDto dto) => new()
    {
        ["case_number"]                   = dto.CaseNumber ?? "",
        ["defendant.first_name"]          = dto.DefendantFirstName ?? "",
        ["defendant.last_name"]           = dto.DefendantLastName ?? "",
        ["license_expiration_date"]       = dto.LicenseExpirationDate?.ToString("MMMM dd, yyyy") ?? "",
        ["privileges_grant_date"]         = dto.PrivilegesGrantDate?.ToString("MMMM dd, yyyy") ?? "",
        ["nufc_date"]                     = dto.NufcDate?.ToString("MMMM dd, yyyy") ?? "",
        ["judicial_officer.first_name"]   = dto.JudicialOfficerFirstName ?? "",
        ["judicial_officer.last_name"]    = dto.JudicialOfficerLastName ?? "",
        ["judicial_officer.officer_type"] = dto.JudicialOfficerType ?? "",
    };
}
