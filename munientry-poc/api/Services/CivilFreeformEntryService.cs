using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Munientry.Poc.Api.Data;

namespace Munientry.Api.Services
{
    public class CivilFreeformEntryService
    {
        private readonly string _connectionString;

        public CivilFreeformEntryService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("AuthorityCourt");
        }

        public void InsertCivilFreeformEntry(CivilFreeformEntryDto dto)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            var command = new SqlCommand(@"INSERT INTO CivilFreeformEntries
                (EntryDate, Plaintiff, Defendant, CaseNumber, AppearanceReason, EntryContent)
                VALUES (@EntryDate, @Plaintiff, @Defendant, @CaseNumber, @AppearanceReason, @EntryContent)", connection);
            command.Parameters.AddWithValue("@EntryDate", dto.EntryDate);
            command.Parameters.AddWithValue("@Plaintiff", dto.Plaintiff);
            command.Parameters.AddWithValue("@Defendant", dto.Defendant);
            command.Parameters.AddWithValue("@CaseNumber", dto.CaseNumber);
            command.Parameters.AddWithValue("@AppearanceReason", (object?)dto.AppearanceReason ?? DBNull.Value);
            command.Parameters.AddWithValue("@EntryContent", dto.EntryContent);
            command.ExecuteNonQuery();
        }
    }
}
