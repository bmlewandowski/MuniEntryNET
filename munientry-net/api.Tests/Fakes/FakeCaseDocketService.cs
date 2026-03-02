using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Munientry.Shared.Dtos;
using Munientry.Api.Services;

namespace Munientry.Api.Tests.Fakes
{
    /// <summary>
    /// In-memory stand-in for <see cref="ICaseDocketService"/> — no SQL Server required.
    /// </summary>
    internal class FakeCaseDocketService : ICaseDocketService
    {
        public const string KnownCaseNumber   = "22CRB01234";
        public const string UnknownCaseNumber = "99TRC99999";

        public Task<List<CaseDocketEntryDto>> GetCaseDocketAsync(string caseNumber)
        {
            if (caseNumber.Equals(KnownCaseNumber, StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(new List<CaseDocketEntryDto>
                {
                    new() { Date = new DateTime(2026, 1, 10), Remark = "Arraignment - Not Guilty Plea entered." },
                    new() { Date = new DateTime(2026, 1, 25), Remark = "Case continued to 02/15/2026." },
                    new() { Date = new DateTime(2026, 2, 15), Remark = "Final Pretrial held." },
                });
            }

            return Task.FromResult(new List<CaseDocketEntryDto>());
        }
    }
}
