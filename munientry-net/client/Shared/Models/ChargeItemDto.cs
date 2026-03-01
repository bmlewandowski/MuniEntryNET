namespace Munientry.Client.Shared.Models
{
    /// <summary>
    /// Represents a single charge row in a multi-charge form submission.
    /// Mirrors <c>Munientry.Api.Data.ChargeItemDto</c>.
    /// </summary>
    public class ChargeItemDto
    {
        public string Offense { get; set; } = string.Empty;
        public string Statute { get; set; } = string.Empty;
        public string Degree  { get; set; } = string.Empty;
        public string Plea    { get; set; } = string.Empty;
    }
}
