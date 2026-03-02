using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Munientry.Shared.Dtos;
using Munientry.Api.Services;

namespace Munientry.Api.Tests.Fakes
{
    /// <summary>
    /// In-memory stand-in for <see cref="IFtaReportService"/> — no SQL Server required.
    /// </summary>
    internal class FakeFtaReportService : IFtaReportService
    {
        public static readonly DateTime DateWithData   = new(2026, 2, 28);
        public static readonly DateTime DateWithNoData = new(2026, 1, 1);

        /// <summary>CRB-type case: DeriveWarrantRule → "Criminal Rule 4".</summary>
        public const string CrbCase = "26CRB01234";

        /// <summary>TRC-type case: DeriveWarrantRule → "Traffic Rule 7".</summary>
        public const string TrcCase = "26TRC05678";

        public Task<List<FtaReportResultDto>> GetFtaReportAsync(DateTime eventDate)
        {
            if (eventDate.Date == DateWithData.Date)
            {
                return Task.FromResult(new List<FtaReportResultDto>
                {
                    new() { CaseNumber = CrbCase },
                    new() { CaseNumber = TrcCase },
                });
            }

            return Task.FromResult(new List<FtaReportResultDto>());
        }
    }
}
