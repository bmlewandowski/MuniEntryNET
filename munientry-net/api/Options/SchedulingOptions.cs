namespace Munientry.Api.Options;

/// <summary>
/// Maps judicial officer last names to scheduling entry DOCX template filenames.
/// Add, rename, or remove entries in appsettings.json — no code change required.
/// Bound from the "Scheduling" section in appsettings.json.
/// </summary>
public sealed class SchedulingOptions
{
    /// <summary>
    /// Dictionary keyed by judicial officer last name.
    /// Lookups are case-insensitive (OrdinalIgnoreCase).
    /// Value is the template filename under the Templates/source directory.
    /// </summary>
    public Dictionary<string, string> JudgeTemplates { get; set; }
        = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Template filename used when the judicial officer name is not found
    /// in <see cref="JudgeTemplates"/>.
    /// </summary>
    public string DefaultTemplate { get; set; } = "Scheduling_Entry_Template_Rohrer.docx";
}
