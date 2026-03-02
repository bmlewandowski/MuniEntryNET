namespace Munientry.Api.Options;

/// <summary>
/// Strongly-typed options for the daily case list feature.
/// Bound from the "DailyList" section in appsettings.json.
/// </summary>
public sealed class DailyListOptions
{
    /// <summary>
    /// When <c>true</c>, the @ReportDate parameter is forwarded to the 6 daily-list stored
    /// procedures so any date can be queried.  Requires the DBA to have updated the SPs to
    /// accept <c>@ReportDate DATE = NULL</c>.
    /// <para>
    /// Set to <c>false</c> (default) in production — the SPs use <c>GETDATE()</c> internally.
    /// Set to <c>true</c> in appsettings.Development.json for dev/test once the SPs are updated.
    /// </para>
    /// </summary>
    public bool PassDateParameter { get; set; }
}
