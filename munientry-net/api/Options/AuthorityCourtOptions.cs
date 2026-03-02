namespace Munientry.Api.Options;

/// <summary>
/// Strongly-typed options for the AuthorityCourt SQL Server connection.
/// Bound once in <see cref="ServiceRegistration.AddMuniEntryServices"/> so
/// the key name <c>"AuthorityCourt"</c> and the null-guard live in exactly one
/// place — not repeated across every SQL-querying service constructor.
/// </summary>
public sealed class AuthorityCourtOptions
{
    /// <summary>
    /// The ADO.NET connection string for the AuthorityCourt SQL Server database.
    /// Sourced from the <c>ConnectionStrings:AuthorityCourt</c> entry in
    /// appsettings.json / environment variables / K8s Secret volume mount.
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;
}
