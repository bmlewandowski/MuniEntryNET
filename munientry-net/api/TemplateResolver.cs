namespace Munientry.Api;

/// <summary>
/// Resolves DOCX template file paths relative to the published output directory.
/// All templates are published to <c>Templates/source/</c> via the
/// <c>CopyToOutputDirectory="PreserveNewest"</c> directive in <c>Munientry.Api.csproj</c>.
/// Using this helper ensures the root path is defined in exactly one place;
/// changing the template directory (e.g. for a K8s volume mount) requires a single edit here.
/// </summary>
internal static class TemplateResolver
{
    private static readonly string TemplateRoot =
        Path.Combine("Templates", "source");

    /// <summary>Returns the full relative path for the given template file name.</summary>
    internal static string Get(string fileName) => Path.Combine(TemplateRoot, fileName);
}
