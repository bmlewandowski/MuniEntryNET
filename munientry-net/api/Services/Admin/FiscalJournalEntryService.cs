using Munientry.Shared.Dtos;

namespace Munientry.Api.Services;

/// <summary>
/// Fills <c>Admin_Fiscal_Template.docx</c> with fiscal journal entry data and returns DOCX bytes.
/// Replaces the legacy <c>AdminFiscalEntryInformation</c> + <c>AdminFiscalEntryCreator</c> workflow
/// in <c>munientry/builders/administrative/admin_fiscal_dialog.py</c>.
/// The system is read-only — no database writes occur.
/// </summary>
public class FiscalJournalEntryService : DocxServiceBase<FiscalJournalEntryDto>, IFiscalJournalEntryService
{
    protected override string TemplatePath { get; } =
        TemplateResolver.Get("Admin_Fiscal_Template.docx");

    protected override Dictionary<string, object> BuildTokens(FiscalJournalEntryDto dto) => new()
    {
        ["account_number"]      = dto.AccountNumber ?? "",
        ["account_name"]        = dto.AccountName ?? "",
        ["subaccount_number"]   = dto.SubaccountNumber ?? "",
        ["subaccount_name"]     = dto.SubaccountName ?? "",
        ["disbursement_reason"] = dto.DisbursementReason ?? "",
        // Format as a plain decimal string to match the legacy string field in AdminFiscalEntryInformation.
        ["disbursement_amount"] = dto.DisbursementAmount?.ToString("F2") ?? "",
        ["disbursement_vendor"] = dto.DisbursementVendor ?? "",
        ["invoice_number"]      = dto.InvoiceNumber ?? "",
        ["judicial_officer"]    = dto.JudicialOfficer ?? "",
    };
}
