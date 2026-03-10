using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Munientry.Api.Options;
using Polly;
using Polly.Registry;

namespace Munientry.Api.Services;

/// <summary>
/// Abstract base class for all services that execute SQL Server stored procedures.
///
/// Every subclass inherits two resilience-wrapped helpers:
///   <see cref="ExecuteSpListAsync{T}"/>   — maps all result rows to a list.
///   <see cref="ExecuteSpSingleAsync{T}"/> — maps the first result row, or returns null.
///
/// Both helpers delegate to the "sql-transient" <see cref="ResiliencePipeline"/> registered
/// in <see cref="ServiceRegistration.AddMuniEntryServices"/>.  The pipeline retries up to 3
/// times on transient <see cref="SqlException"/> or <see cref="TimeoutException"/>, using
/// exponential back-off with jitter, and enforces a per-attempt 30-second timeout.
///
/// The complete open → execute → read cycle is inside each <c>ExecuteAsync</c> call so that
/// a failed mid-query connection is torn down and a fresh <see cref="SqlConnection"/> is
/// opened on every retry attempt.
/// </summary>
public abstract class SqlServiceBase
{
    private readonly string _connectionString;
    private readonly ResiliencePipeline _pipeline;

    protected SqlServiceBase(
        IOptions<AuthorityCourtOptions> options,
        ResiliencePipelineProvider<string> pipelineProvider)
    {
        _connectionString = options.Value.ConnectionString;
        _pipeline = pipelineProvider.GetPipeline("sql-transient");
    }

    /// <summary>
    /// Executes <paramref name="procedureName"/> and maps every result row using
    /// <paramref name="mapRow"/>, returning the full list.
    /// </summary>
    /// <typeparam name="T">DTO type produced per row.</typeparam>
    /// <param name="procedureName">Fully-qualified stored procedure name, e.g. <c>[reports].[DMCMuniEntryCaseSearch]</c>.</param>
    /// <param name="parameterize">Action that adds input parameters to the <see cref="SqlCommand"/> before execution.</param>
    /// <param name="mapRow">Function that converts one <see cref="IDataReader"/> row to <typeparamref name="T"/>.</param>
    /// <param name="cancellationToken">Optional cancellation token propagated to all async ADO.NET calls.</param>
    protected async Task<List<T>> ExecuteSpListAsync<T>(
        string procedureName,
        Action<SqlCommand> parameterize,
        Func<IDataReader, T> mapRow,
        CancellationToken cancellationToken = default)
    {
        return await _pipeline.ExecuteAsync(async ct =>
        {
            var results = new List<T>();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(procedureName, conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            parameterize(cmd);

            await conn.OpenAsync(ct);
            using var reader = await cmd.ExecuteReaderAsync(ct);
            while (await reader.ReadAsync(ct))
                results.Add(mapRow(reader));

            return results;
        }, cancellationToken);
    }

    /// <summary>
    /// Executes <paramref name="procedureName"/> and maps the first result row using
    /// <paramref name="mapRow"/>, or returns <c>null</c> if the stored procedure returns
    /// no rows.
    /// </summary>
    /// <typeparam name="T">DTO type produced from the single row.</typeparam>
    /// <param name="procedureName">Fully-qualified stored procedure name.</param>
    /// <param name="parameterize">Action that adds input parameters to the <see cref="SqlCommand"/> before execution.</param>
    /// <param name="mapRow">Function that converts one <see cref="IDataReader"/> row to <typeparamref name="T"/>.</param>
    /// <param name="cancellationToken">Optional cancellation token propagated to all async ADO.NET calls.</param>
    protected async Task<T?> ExecuteSpSingleAsync<T>(
        string procedureName,
        Action<SqlCommand> parameterize,
        Func<IDataReader, T> mapRow,
        CancellationToken cancellationToken = default) where T : class
    {
        return await _pipeline.ExecuteAsync(async ct =>
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(procedureName, conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            parameterize(cmd);

            await conn.OpenAsync(ct);
            using var reader = await cmd.ExecuteReaderAsync(ct);
            return await reader.ReadAsync(ct) ? mapRow(reader) : null;
        }, cancellationToken);
    }
}
