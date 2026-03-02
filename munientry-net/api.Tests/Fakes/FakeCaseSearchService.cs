using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Munientry.Shared.Dtos;
using Munientry.Api.Services;

namespace Munientry.Api.Tests.Fakes
{
    /// <summary>
    /// In-memory stand-in for <see cref="ICaseSearchService"/> — no SQL Server required.
    /// Returns populated data for well-known case numbers used across the test suite;
    /// returns an empty list (→ 404) for everything else.
    /// </summary>
    internal class FakeCaseSearchService : ICaseSearchService
    {
        /// <summary>Case recognised by CaseSearchApiTests (happy-path assertions).</summary>
        public const string KnownCaseNumber = "2026-CR-001";

        /// <summary>Case that yields no results — triggers the 404 path.</summary>
        public const string UnknownCaseNumber = "9999-XX-999";

        /// <summary>CRB case returned by FakeFtaReportService; used by the FTA entry/batch endpoints.</summary>
        public const string FtaCrbCase = "26CRB01234";

        /// <summary>TRC case returned by FakeFtaReportService; used by the FTA entry/batch endpoints.</summary>
        public const string FtaTrcCase = "26TRC05678";

        private static readonly Dictionary<string, CaseSearchResultDto> KnownCases =
            new(StringComparer.OrdinalIgnoreCase)
            {
                [KnownCaseNumber] = new CaseSearchResultDto
                {
                    CaseNumber        = KnownCaseNumber,
                    SubCaseNumber     = "2026-CR-001-A",
                    SubCaseId         = 1,
                    Charge            = "OVI",
                    ViolationId       = 10,
                    ViolationDetailId = 20,
                    Statute           = "4511.19",
                    DegreeCode        = "M1",
                    DefFirstName      = "Jane",
                    DefLastName       = "Doe",
                    MovingBool        = true,
                    ViolationDate     = new DateTime(2026, 1, 15),
                    FraInFile         = "N/A",
                    DefenseCounsel    = "Smith, J.",
                    PubDef            = false,
                },
                [FtaCrbCase] = new CaseSearchResultDto
                {
                    CaseNumber   = FtaCrbCase,
                    DefFirstName = "John",
                    DefLastName  = "Doe",
                },
                [FtaTrcCase] = new CaseSearchResultDto
                {
                    CaseNumber   = FtaTrcCase,
                    DefFirstName = "Jane",
                    DefLastName  = "Smith",
                },
            };

        public Task<List<CaseSearchResultDto>> SearchCaseAsync(string caseNumber)
        {
            if (KnownCases.TryGetValue(caseNumber, out var dto))
                return Task.FromResult(new List<CaseSearchResultDto> { dto });

            return Task.FromResult(new List<CaseSearchResultDto>());
        }
    }
}
