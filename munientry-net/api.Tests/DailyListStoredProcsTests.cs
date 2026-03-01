using System.Collections.Generic;
using System.Linq;
using Munientry.Api.Data;
using Xunit;

namespace Munientry.Api.Tests
{
    /// <summary>
    /// Pure unit tests for DailyListStoredProcs — no HTTP, no database.
    /// Verifies the list-type → stored procedure name mapping used by DailyListService.
    /// </summary>
    public class DailyListStoredProcsTests
    {
        // -----------------------------------------------------------------------
        // GetProcName — valid list types
        // -----------------------------------------------------------------------

        [Theory]
        [InlineData("arraignments",    "[reports].[DMCMuniEntryArraignment]")]
        [InlineData("slated",          "[reports].[DMCMuniEntrySlated]")]
        [InlineData("pleas",           "[reports].[DMCMuniEntryPleas]")]
        [InlineData("pcvh_fcvh",       "[reports].[DMCMuniEntryPrelimCommContViolHearings]")]
        [InlineData("final_pretrial",  "[reports].[DMCMuniEntryFinalPreTrials]")]
        [InlineData("trials_to_court", "[reports].[DMCMuniEntryBenchTrials]")]
        public void GetProcName_ValidListType_ReturnsCorrectProcedureName(string listType, string expectedProc)
        {
            var result = DailyListStoredProcs.GetProcName(listType);

            Assert.Equal(expectedProc, result);
        }

        // -----------------------------------------------------------------------
        // GetProcName — case-insensitivity
        // -----------------------------------------------------------------------

        [Theory]
        [InlineData("ARRAIGNMENTS")]
        [InlineData("Arraignments")]
        [InlineData("ArRaIgnMeNtS")]
        [InlineData("PLEAS")]
        [InlineData("TRIALS_TO_COURT")]
        public void GetProcName_AnyCase_ReturnsProcedureName(string listType)
        {
            var result = DailyListStoredProcs.GetProcName(listType);

            Assert.NotNull(result);
        }

        // -----------------------------------------------------------------------
        // GetProcName — invalid / unknown list types
        // -----------------------------------------------------------------------

        [Theory]
        [InlineData("bogus")]
        [InlineData("")]
        [InlineData("trial")]           // partial match should not resolve
        [InlineData("arraignment")]     // singular — not a valid key
        [InlineData("pcvh")]            // prefix only
        [InlineData(" arraignments")]   // leading space
        public void GetProcName_InvalidListType_ReturnsNull(string listType)
        {
            var result = DailyListStoredProcs.GetProcName(listType);

            Assert.Null(result);
        }

        // -----------------------------------------------------------------------
        // ValidTypes — completeness
        // -----------------------------------------------------------------------

        [Fact]
        public void ValidTypes_ContainsExactlySixEntries()
        {
            var types = DailyListStoredProcs.ValidTypes.ToList();

            Assert.Equal(6, types.Count);
        }

        [Theory]
        [InlineData("arraignments")]
        [InlineData("slated")]
        [InlineData("pleas")]
        [InlineData("pcvh_fcvh")]
        [InlineData("final_pretrial")]
        [InlineData("trials_to_court")]
        public void ValidTypes_ContainsAllExpectedKeys(string expected)
        {
            Assert.Contains(expected, DailyListStoredProcs.ValidTypes);
        }

        [Fact]
        public void ValidTypes_AndGetProcName_AreConsistent()
        {
            // Every key in ValidTypes must resolve to a non-null proc name
            foreach (var listType in DailyListStoredProcs.ValidTypes)
            {
                var proc = DailyListStoredProcs.GetProcName(listType);
                Assert.NotNull(proc);
                Assert.StartsWith("[reports].", proc);
            }
        }

        // -----------------------------------------------------------------------
        // Proc name format — all names follow [schema].[name] convention
        // -----------------------------------------------------------------------

        [Fact]
        public void AllProcNames_FollowSchemaNameFormat()
        {
            foreach (var listType in DailyListStoredProcs.ValidTypes)
            {
                var proc = DailyListStoredProcs.GetProcName(listType)!;
                Assert.Matches(@"^\[reports\]\.\[DMCMuniEntry\w+\]$", proc);
            }
        }

        [Fact]
        public void AllProcNames_AreUnique()
        {
            var procs = DailyListStoredProcs.ValidTypes
                .Select(DailyListStoredProcs.GetProcName)
                .ToList();

            Assert.Equal(procs.Count, procs.Distinct().Count());
        }
    }
}
