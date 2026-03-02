using Munientry.Shared.Dtos;

namespace Munientry.Api.Services;

/// <summary>
/// Shared helpers for building the <c>charges_list</c> token used by list-based sentencing
/// templates that use <c>{%tc for item in charges_list %}</c> table-row loops.
/// <para>
/// Extracted from the identical private methods that previously existed in
/// <c>JailCcPleaService</c>, <c>LeapSentencingService</c>,
/// <c>LeapAdmissionAlreadyValidService</c>, and <c>LeapValidSentencingService</c>.
/// </para>
/// </summary>
internal static class ChargeListBuilder
{
    /// <summary>
    /// Builds a simple charges list (offense / statute / degree / plea) from either the
    /// <paramref name="items"/> collection or the four individual fallback property values.
    /// Used by LEAP admission forms that display a charge table without fine or jail fields.
    /// </summary>
    internal static List<Dictionary<string, string>> Build(
        List<ChargeItemDto> items,
        string? offense, string? statute, string? degree, string? plea)
    {
        if (items.Count > 0)
            return items.Select(c => new Dictionary<string, string>
            {
                ["offense"] = c.Offense,
                ["statute"] = c.Statute,
                ["degree"]  = c.Degree,
                ["plea"]    = c.Plea,
            }).ToList();

        return [new() { ["offense"] = offense ?? "", ["statute"] = statute ?? "",
                        ["degree"]  = degree  ?? "", ["plea"]    = plea    ?? "" }];
    }

    /// <summary>
    /// Builds a full sentencing charges list (offense / statute / degree / plea / finding /
    /// fines / jail days) from either the <paramref name="items"/> collection or the
    /// individual fallback string parameters.
    /// Used by Jail/CC Plea and LEAP sentencing forms.
    /// <paramref name="jailDays"/> and <paramref name="jailDaysSuspended"/> default to
    /// <c>""</c> for the fallback row when the form does not capture jail data (LEAP sentencing).
    /// </summary>
    internal static List<Dictionary<string, string>> BuildSentencing(
        List<SentencingChargeItemDto> items,
        string? offense, string? statute, string? degree, string? plea,
        string? finding, string? finesAmount, string? finesSuspended,
        string? jailDays = null, string? jailDaysSuspended = null)
    {
        if (items.Count > 0)
            return items.Select(c => new Dictionary<string, string>
            {
                ["offense"]             = c.Offense,
                ["statute"]             = c.Statute,
                ["degree"]              = c.Degree,
                ["plea"]                = c.Plea,
                ["finding"]             = c.Finding,
                ["fines_amount"]        = c.FinesAmount,
                ["fines_suspended"]     = c.FinesSuspended,
                ["jail_days"]           = c.JailDays,
                ["jail_days_suspended"] = c.JailDaysSuspended,
            }).ToList();

        return [new()
        {
            ["offense"]             = offense           ?? "",
            ["statute"]             = statute           ?? "",
            ["degree"]              = degree            ?? "",
            ["plea"]                = plea              ?? "",
            ["finding"]             = finding           ?? "",
            ["fines_amount"]        = finesAmount       ?? "",
            ["fines_suspended"]     = finesSuspended    ?? "",
            ["jail_days"]           = jailDays          ?? "",
            ["jail_days_suspended"] = jailDaysSuspended ?? "",
        }];
    }
}
