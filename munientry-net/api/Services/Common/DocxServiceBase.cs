namespace Munientry.Api.Services;

/// <summary>
/// Abstract base for all DOCX entry services.
/// <para>
/// Subclasses supply:
/// <list type="bullet">
///   <item><see cref="TemplatePath"/> — the resolved path to the source <c>.docx</c> file.</item>
///   <item><see cref="BuildTokens"/> — the token map passed to <see cref="DocxTemplateProcessor"/>.</item>
/// </list>
/// This class calls <see cref="DocxTemplateProcessor.FillTemplate"/> and returns the filled bytes,
/// eliminating the identical boilerplate that previously appeared in every DOCX service class.
/// </para>
/// <para>
/// Override <see cref="GenerateDocx"/> for the rare case where the template path depends on
/// runtime data (e.g. <c>SchedulingEntryService</c> selects the file based on the judicial officer).
/// </para>
/// </summary>
public abstract class DocxServiceBase<TDto> : IDocxService<TDto>
{
    /// <summary>Full path to the source DOCX template, resolved via <see cref="TemplateResolver"/>.</summary>
    protected abstract string TemplatePath { get; }

    /// <summary>Builds the token dictionary used to fill the template.</summary>
    protected abstract Dictionary<string, object> BuildTokens(TDto dto);

    /// <inheritdoc/>
    public virtual byte[] GenerateDocx(TDto dto) =>
        DocxTemplateProcessor.FillTemplate(TemplatePath, BuildTokens(dto));
}
