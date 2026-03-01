namespace Munientry.Client.Shared.Models
{
    public class SentencingChargeItemDto
    {
        public string Offense { get; set; } = string.Empty;
        public string Statute { get; set; } = string.Empty;
        public string Degree { get; set; } = string.Empty;
        public string Plea { get; set; } = string.Empty;
        public string Finding { get; set; } = string.Empty;
        public string FinesAmount { get; set; } = string.Empty;
        public string FinesSuspended { get; set; } = string.Empty;
        public string JailDays { get; set; } = string.Empty;
        public string JailDaysSuspended { get; set; } = string.Empty;
    }
}
