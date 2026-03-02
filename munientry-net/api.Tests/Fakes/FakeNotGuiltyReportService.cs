using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Munientry.Shared.Dtos;
using Munientry.Api.Services;

namespace Munientry.Api.Tests.Fakes
{
    /// <summary>
    /// In-memory stand-in for <see cref="INotGuiltyReportService"/> — no SQL Server required.
    /// </summary>
    internal class FakeNotGuiltyReportService : INotGuiltyReportService
    {
        public static readonly DateTime DateWithData   = new(2026, 2, 28);
        public static readonly DateTime DateWithNoData = new(2026, 1, 1);

        public Task<List<NotGuiltyReportResultDto>> GetNotGuiltyReportAsync(DateTime eventDate)
        {
            if (eventDate.Date == DateWithData.Date)
            {
                return Task.FromResult(new List<NotGuiltyReportResultDto>
                {
                    new()
                    {
                        CaseNumber  = "2026-CR-010",
                        DefFullName = "Smith, John",
                        Remark      = "NG Plea — Continuance to 03/15/2026",
                    },
                    new()
                    {
                        CaseNumber  = "2026-CR-011",
                        DefFullName = "Brown, Alice",
                        Remark      = "Not Guilty plea entered",
                    },
                });
            }

            return Task.FromResult(new List<NotGuiltyReportResultDto>());
        }
    }
}
