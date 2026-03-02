using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Munientry.Shared.Dtos;
using Munientry.Api.Services;

namespace Munientry.Api.Tests.Fakes
{
    /// <summary>
    /// In-memory stand-in for <see cref="IEventReportService"/> — no SQL Server required.
    /// </summary>
    internal class FakeEventReportService : IEventReportService
    {
        public const string KnownEventCode  = "Arraignments";
        public const string EmptyEventCode  = "Jury Trials";
        public static readonly DateTime ValidDate = new(2026, 2, 28);

        public Task<List<EventReportResultDto>> GetEventReportAsync(string eventCode, DateTime eventDate)
        {
            if (eventCode.Equals(KnownEventCode, StringComparison.OrdinalIgnoreCase) &&
                eventDate.Date == ValidDate.Date)
            {
                return Task.FromResult(new List<EventReportResultDto>
                {
                    new()
                    {
                        Time           = "09:00 AM",
                        CaseNumber     = "2026-CR-010",
                        DefFullName    = "Smith, John",
                        SubCaseNumber  = "2026-CR-010-A",
                        Charge         = "OVI",
                        EventId        = "27",
                        JudgeId        = "JUDGE-A",
                        DefenseCounsel = "Jones, Mary",
                    },
                    new()
                    {
                        Time           = "09:30 AM",
                        CaseNumber     = "2026-CR-015",
                        DefFullName    = "Brown, Alice",
                        SubCaseNumber  = "2026-CR-015-A",
                        Charge         = "Reckless Driving",
                        EventId        = "28",
                        JudgeId        = "JUDGE-A",
                        DefenseCounsel = null,
                    },
                });
            }

            return Task.FromResult(new List<EventReportResultDto>());
        }
    }
}
