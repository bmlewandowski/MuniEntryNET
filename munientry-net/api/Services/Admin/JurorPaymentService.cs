using Munientry.Shared.Dtos;

namespace Munientry.Api.Services;

public class JurorPaymentService : DocxServiceBase<JurorPaymentDto>, IJurorPaymentService
{
    protected override string TemplatePath { get; } =
        TemplateResolver.Get("Jury_Payment_Template.docx");

    protected override Dictionary<string, object> BuildTokens(JurorPaymentDto dto) => new()
    {
        ["case_number"]                   = dto.CaseNumber ?? "",
        ["entry_date"]                    = dto.EntryDate.ToString("MMMM dd, yyyy"),
        ["defendant.first_name"]          = dto.DefendantFirstName ?? "",
        ["defendant.last_name"]           = dto.DefendantLastName ?? "",
        ["trial_date"]                    = dto.TrialDate.ToString("MMMM dd, yyyy"),
        ["jurors_reported"]               = dto.JurorsReported.ToString(),
        ["jurors_seated"]                 = dto.JurorsSeated.ToString(),
        ["jurors_not_seated"]             = dto.JurorsNotSeated.ToString(),
        ["jurors_pay_not_seated"]         = dto.JurorsPayNotSeated.ToString(),
        ["jurors_pay_seated"]             = dto.JurorsPaySeated.ToString(),
        ["jury_panel_total_pay"]          = dto.JuryPanelTotalPay.ToString(),
        ["judicial_officer.first_name"]   = dto.JudicialOfficerFirstName ?? "",
        ["judicial_officer.last_name"]    = dto.JudicialOfficerLastName ?? "",
        ["judicial_officer.officer_type"] = dto.JudicialOfficerType ?? "",
    };
}
