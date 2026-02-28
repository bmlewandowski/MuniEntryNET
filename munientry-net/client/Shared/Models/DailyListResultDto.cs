namespace Munientry.Poc.Client.Shared.Models
{
    /// <summary>
    /// Client-side mirror of the API's DailyListResultDto.
    /// Returned by GET /api/dailylist/{listType}/{date} for each of the
    /// 6 daily case list stored procedures.
    /// </summary>
    public class DailyListResultDto
    {
        public string? Time { get; set; }
        public string? CaseNumber { get; set; }
        public string? DefFullName { get; set; }
        public string? SubCaseNumber { get; set; }
        public string? Charge { get; set; }
        public string? EventId { get; set; }
        public string? JudgeId { get; set; }
        public string? DefenseCounsel { get; set; }
    }
}
