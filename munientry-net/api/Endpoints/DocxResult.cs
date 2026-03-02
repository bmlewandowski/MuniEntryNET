namespace Munientry.Api.Endpoints;

/// <summary>
/// Factory for the standard DOCX <see cref="IResult"/> returned by all entry endpoints.
/// Centralises the MIME type constant and timestamped filename format, eliminating the
/// three-line <c>Results.File(...)</c> boilerplate that was repeated across every endpoint.
/// </summary>
internal static class DocxResult
{
    internal const string MimeType =
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

    /// <summary>
    /// Returns a DOCX file response with a timestamp-suffixed filename.
    /// </summary>
    /// <param name="bytes">Filled DOCX bytes from a service's <c>GenerateDocx</c> call.</param>
    /// <param name="baseName">Entry type prefix used in the filename, e.g. <c>"NotGuiltyPlea"</c>.</param>
    /// <param name="identifier">
    /// Optional identifier appended after the base name — typically the case number.
    /// When <see langword="null"/> or empty, the filename is <c>{baseName}_{timestamp}.docx</c>.
    /// When provided, the filename is <c>{baseName}_{identifier}_{timestamp}.docx</c>.
    /// </param>
    internal static IResult File(byte[] bytes, string baseName, string? identifier = null)
    {
        var id = string.IsNullOrWhiteSpace(identifier) ? "" : $"_{identifier}";
        return Results.File(bytes, MimeType, $"{baseName}{id}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx");
    }
}
