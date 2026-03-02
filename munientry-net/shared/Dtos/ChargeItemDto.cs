namespace Munientry.Shared.Dtos
{
    /// <summary>
    /// Represents a single charge row in a multi-charge form submission.
    /// Used by endpoints that drive <c>{%tc for charge in charges_list %}</c>
    /// table-row loops in DOCX templates.
    /// </summary>
    public class ChargeItemDto
    {
        public string Offense { get; set; } = string.Empty;
        public string Statute { get; set; } = string.Empty;
        public string Degree  { get; set; } = string.Empty;
        public string Plea    { get; set; } = string.Empty;
    }
}
