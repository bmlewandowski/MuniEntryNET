namespace Munientry.Api.Services;

/// <summary>
/// Common contract for all DOCX-generation services.
/// Each domain-specific <c>IXxxService</c> extends this interface, making the shared
/// pattern explicit and allowing concrete classes to inherit a common base.
/// </summary>
public interface IDocxService<TDto>
{
    byte[] GenerateDocx(TDto dto);
}
