using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Munientry.Poc.Api.Data;

namespace Munientry.Api.Services
{
    public class CommunityControlTermsNoticesService
    {
        private readonly string? _connectionString;

        public CommunityControlTermsNoticesService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("AuthorityCourt");
            if (string.IsNullOrEmpty(_connectionString))
                throw new InvalidOperationException("Connection string 'AuthorityCourt' is missing or null.");
        }

        public void InsertCommunityControlTermsNotices(CommunityControlTermsNoticesDto dto)
        {
            using var connection = new SqlConnection(_connectionString!);
            connection.Open();
            var command = new SqlCommand(@"INSERT INTO CommunityControlTermsNotices
                (CaseNumber, EntryDate, DefendantFirstName, DefendantLastName, HearingDate, HearingTime, AssignedCourtroom, DefenseCounselName, ViolationType, InterpreterRequired, LanguageRequired)
                VALUES (@CaseNumber, @EntryDate, @DefendantFirstName, @DefendantLastName, @HearingDate, @HearingTime, @AssignedCourtroom, @DefenseCounselName, @ViolationType, @InterpreterRequired, @LanguageRequired)", connection);
            command.Parameters.AddWithValue("@CaseNumber", dto.CaseNumber);
            command.Parameters.AddWithValue("@EntryDate", dto.EntryDate);
            command.Parameters.AddWithValue("@DefendantFirstName", dto.DefendantFirstName);
            command.Parameters.AddWithValue("@DefendantLastName", dto.DefendantLastName);
            command.Parameters.AddWithValue("@HearingDate", dto.HearingDate);
            command.Parameters.AddWithValue("@HearingTime", dto.HearingTime);
            command.Parameters.AddWithValue("@AssignedCourtroom", dto.AssignedCourtroom);
            command.Parameters.AddWithValue("@DefenseCounselName", dto.DefenseCounselName ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@ViolationType", dto.ViolationType ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@InterpreterRequired", dto.InterpreterRequired);
            command.Parameters.AddWithValue("@LanguageRequired", dto.LanguageRequired ?? (object)DBNull.Value);
            command.ExecuteNonQuery();
        }
    }
}
