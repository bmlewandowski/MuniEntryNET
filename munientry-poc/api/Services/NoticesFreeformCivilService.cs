using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Munientry.Poc.Api.Data;

namespace Munientry.Api.Services
{
    public class NoticesFreeformCivilService
    {
        private readonly string? _connectionString;

        public NoticesFreeformCivilService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("AuthorityCourt");
            if (string.IsNullOrEmpty(_connectionString))
                throw new InvalidOperationException("Connection string 'AuthorityCourt' is missing or null.");
        }

        public void InsertNoticesFreeformCivil(NoticesFreeformCivilDto dto)
        {
            using var connection = new SqlConnection(_connectionString!);
            connection.Open();
            var command = new SqlCommand(@"INSERT INTO NoticesFreeformCivil
                (CaseNumber, EntryDate, DefendantFirstName, DefendantLastName, NoticeText)
                VALUES (@CaseNumber, @EntryDate, @DefendantFirstName, @DefendantLastName, @NoticeText)", connection);
            command.Parameters.AddWithValue("@CaseNumber", dto.CaseNumber);
            command.Parameters.AddWithValue("@EntryDate", dto.EntryDate);
            command.Parameters.AddWithValue("@DefendantFirstName", dto.DefendantFirstName);
            command.Parameters.AddWithValue("@DefendantLastName", dto.DefendantLastName);
            command.Parameters.AddWithValue("@NoticeText", dto.NoticeText);
            command.ExecuteNonQuery();
        }
    }
}
