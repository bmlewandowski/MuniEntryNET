using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Munientry.Shared.Dtos;
using Munientry.Api.Services;

namespace Munientry.Api.Tests.Fakes
{
    /// <summary>
    /// In-memory stand-in for <see cref="IDailyListService"/> — no SQL Server required.
    /// </summary>
    internal class FakeDailyListService : IDailyListService
    {
        public const string ListTypeWithData  = "arraignments";
        public const string ListTypeWithData2 = "pleas";

        public static readonly string[] AllValidTypes =
            { "arraignments", "slated", "pleas", "pcvh_fcvh", "final_pretrial", "trials_to_court" };

        public Task<List<DailyListResultDto>> GetDailyListAsync(string listType)
        {
            // Mirror the real service: throw on unknown list type.
            var procName = DailyListStoredProcs.GetProcName(listType);
            if (procName is null)
                throw new ArgumentException(
                    $"Unknown list type '{listType}'. Valid values: {string.Join(", ", DailyListStoredProcs.ValidTypes)}");

            if (listType.Equals(ListTypeWithData, StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(new List<DailyListResultDto>
                {
                    new()
                    {
                        Time           = "09:00 AM",
                        CaseNumber     = "2026-CR-010",
                        DefFullName    = "Smith, John",
                        SubCaseNumber  = "2026-CR-010-A",
                        Charge         = "OVI",
                        EventId        = "EVT-001",
                        JudgeId        = "JUDGE-01",
                        DefenseCounsel = "Jones, Mary",
                    },
                    new()
                    {
                        Time           = "09:30 AM",
                        CaseNumber     = "2026-CR-011",
                        DefFullName    = "Brown, Alice",
                        SubCaseNumber  = "2026-CR-011-A",
                        Charge         = "Reckless Driving",
                        EventId        = "EVT-002",
                        JudgeId        = "JUDGE-01",
                        DefenseCounsel = null,
                    },
                });
            }

            // All other valid list types → no cases scheduled.
            return Task.FromResult(new List<DailyListResultDto>());
        }
    }
}
