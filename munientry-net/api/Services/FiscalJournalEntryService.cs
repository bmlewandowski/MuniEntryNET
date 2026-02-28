using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Munientry.Poc.Api.Data;

namespace Munientry.Api.Services
{
    public interface IFiscalJournalEntryService
    {
        void InsertFiscalJournalEntry(FiscalJournalEntryDto dto);
    }

    public class FiscalJournalEntryService : IFiscalJournalEntryService
    {
        private readonly string _connectionString;

        public FiscalJournalEntryService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("AuthorityCourt")!;
        }

        public void InsertFiscalJournalEntry(FiscalJournalEntryDto dto)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            var command = new SqlCommand(@"INSERT INTO FiscalJournalEntries
                (AccountNumber, AccountName, SubaccountNumber, SubaccountName, DisbursementReason, DisbursementAmount, DisbursementVendor, InvoiceNumber, JudicialOfficer)
                VALUES (@AccountNumber, @AccountName, @SubaccountNumber, @SubaccountName, @DisbursementReason, @DisbursementAmount, @DisbursementVendor, @InvoiceNumber, @JudicialOfficer)", connection);
            command.Parameters.AddWithValue("@AccountNumber", dto.AccountNumber);
            command.Parameters.AddWithValue("@AccountName", (object?)dto.AccountName ?? DBNull.Value);
            command.Parameters.AddWithValue("@SubaccountNumber", (object?)dto.SubaccountNumber ?? DBNull.Value);
            command.Parameters.AddWithValue("@SubaccountName", (object?)dto.SubaccountName ?? DBNull.Value);
            command.Parameters.AddWithValue("@DisbursementReason", (object?)dto.DisbursementReason ?? DBNull.Value);
            command.Parameters.AddWithValue("@DisbursementAmount", (object?)dto.DisbursementAmount ?? DBNull.Value);
            command.Parameters.AddWithValue("@DisbursementVendor", (object?)dto.DisbursementVendor ?? DBNull.Value);
            command.Parameters.AddWithValue("@InvoiceNumber", (object?)dto.InvoiceNumber ?? DBNull.Value);
            command.Parameters.AddWithValue("@JudicialOfficer", (object?)dto.JudicialOfficer ?? DBNull.Value);
            command.ExecuteNonQuery();
        }
    }
}
