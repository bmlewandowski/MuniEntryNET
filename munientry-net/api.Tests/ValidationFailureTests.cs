using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Munientry.Shared.Dtos;
using Munientry.Api.Tests.Infrastructure;
using Xunit;

namespace Munientry.Api.Tests
{
    /// <summary>
    /// Validation failure tests — verifies that the FluentValidationFilter wired to the
    /// /api/v1 route group returns HTTP 422 (Unprocessable Entity) when domain rules are
    /// violated, and HTTP 200 when the corresponding rules are satisfied.
    ///
    /// Each region covers one validator from shared/Validation/FormValidators.cs.
    /// Where a rule is shared across many DTOs (e.g. AddDefenseCounselRule, AddBondRules),
    /// the most representative endpoint is used to avoid testing the same rule N times.
    /// </summary>
    public class ValidationFailureTests : IClassFixture<MuniEntryWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public ValidationFailureTests(MuniEntryWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        // ── Shared helpers ────────────────────────────────────────────────────

        private const HttpStatusCode Unprocessable = HttpStatusCode.UnprocessableEntity;

        /// Reads the RFC 7807 Problem Details body as a JsonDocument.
        private static async Task<JsonDocument> ReadProblemAsync(HttpResponseMessage response)
        {
            var body = await response.Content.ReadAsStringAsync();
            return JsonDocument.Parse(body);
        }

        // ── Defense counsel rule — NotGuiltyPleaValidator (representative) ────
        //
        // Rules.AddDefenseCounselRule is shared by ~10 validators.
        // NotGuiltyPleaValidator is used here as a single representative so the
        // rule is exercised once end-to-end through the full validation pipeline.

        [Fact]
        public async Task PostNotGuiltyPlea_CounselNameNull_WaivedFalse_Returns422()
        {
            var dto = ValidNotGuiltyDto();
            dto.DefenseCounselName = null;
            dto.DefenseCounselWaived = false;

            var response = await _client.PostAsJsonAsync("/api/v1/notguiltyplea", dto);

            Assert.Equal(Unprocessable, response.StatusCode);
        }

        [Fact]
        public async Task PostNotGuiltyPlea_CounselNameEmpty_WaivedFalse_Returns422()
        {
            var dto = ValidNotGuiltyDto();
            dto.DefenseCounselName = "";
            dto.DefenseCounselWaived = false;

            var response = await _client.PostAsJsonAsync("/api/v1/notguiltyplea", dto);

            Assert.Equal(Unprocessable, response.StatusCode);
        }

        [Fact]
        public async Task PostNotGuiltyPlea_CounselWaived_NullName_Returns200WithDocx()
        {
            // When counsel is waived the name field is optional — validation must pass.
            var dto = ValidNotGuiltyDto();
            dto.DefenseCounselName = null;
            dto.DefenseCounselWaived = true;

            var response = await _client.PostAsJsonAsync("/api/v1/notguiltyplea", dto);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task PostNotGuiltyPlea_ValidationFailure_Returns422WithErrorsKey()
        {
            // Verify the RFC 7807 response shape: status=422, "errors" dict present.
            var dto = ValidNotGuiltyDto();
            dto.DefenseCounselName = null;
            dto.DefenseCounselWaived = false;

            var response = await _client.PostAsJsonAsync("/api/v1/notguiltyplea", dto);

            Assert.Equal(Unprocessable, response.StatusCode);

            using var doc = await ReadProblemAsync(response);
            Assert.Equal(422, doc.RootElement.GetProperty("status").GetInt32());
            Assert.True(
                doc.RootElement.TryGetProperty("errors", out _),
                "RFC 7807 validation response must contain an 'errors' key.");
        }

        // ── Bond rules — NotGuiltyPleaValidator ───────────────────────────────
        //
        // Rule A (check_if_no_bond_amount): amount-required bond type must have a non-empty,
        //   non-"None" bond amount.
        // Rule B (check_if_improper_bond_type): no-amount bond types must NOT have an amount.

        [Theory]
        [InlineData("Cash", null)]
        [InlineData("Cash", "")]
        [InlineData("Cash", "None")]
        [InlineData("Personal Recognizance", null)]
        public async Task PostNotGuiltyPlea_AmountRequiredBondType_MissingAmount_Returns422(
            string bondType, string? bondAmount)
        {
            var dto = ValidNotGuiltyDto();
            dto.BondType = bondType;
            dto.BondAmount = bondAmount;

            var response = await _client.PostAsJsonAsync("/api/v1/notguiltyplea", dto);

            Assert.Equal(Unprocessable, response.StatusCode);
        }

        [Theory]
        [InlineData("Recognizance (OR) Bond", "500")]
        [InlineData("Continue Existing Bond", "1000")]
        [InlineData("No Bond", "250")]
        public async Task PostNotGuiltyPlea_NoAmountBondType_WithAmount_Returns422(
            string bondType, string bondAmount)
        {
            var dto = ValidNotGuiltyDto();
            dto.BondType = bondType;
            dto.BondAmount = bondAmount;

            var response = await _client.PostAsJsonAsync("/api/v1/notguiltyplea", dto);

            Assert.Equal(Unprocessable, response.StatusCode);
        }

        [Theory]
        [InlineData("Recognizance (OR) Bond", null)]
        [InlineData("No Bond", "")]
        [InlineData("Continue Existing Bond", null)]
        public async Task PostNotGuiltyPlea_NoAmountBondType_NoAmount_Returns200WithDocx(
            string bondType, string? bondAmount)
        {
            // No-amount bond type + no amount — both bond rules satisfied.
            var dto = ValidNotGuiltyDto();
            dto.BondType = bondType;
            dto.BondAmount = bondAmount;

            var response = await _client.PostAsJsonAsync("/api/v1/notguiltyplea", dto);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        // ── JailCcPleaValidator — suspended jail days vs imposed ──────────────
        //
        // check_if_jail_suspended_more_than_imposed: for each ChargeItem,
        //   JailDaysSuspended must not exceed JailDays.

        [Fact]
        public async Task PostJailCcPlea_SuspendedDaysExceedImposed_Returns422()
        {
            var dto = ValidJailCcPleaDto();
            dto.ChargeItems =
            [
                new SentencingChargeItemDto
                {
                    Offense = "OVI", Statute = "4511.19A1A", Degree = "M1",
                    Plea = "Guilty", Finding = "Guilty",
                    FinesAmount = "375.00", FinesSuspended = "0.00",
                    JailDays = "180",
                    JailDaysSuspended = "181"   // suspended > imposed → invalid
                }
            ];

            var response = await _client.PostAsJsonAsync("/api/v1/jailccplea", dto);

            Assert.Equal(Unprocessable, response.StatusCode);
        }

        [Fact]
        public async Task PostJailCcPlea_SuspendedEqualsImposed_Returns200WithDocx()
        {
            // Suspended == imposed is valid (border case).
            var dto = ValidJailCcPleaDto();
            dto.ChargeItems =
            [
                new SentencingChargeItemDto
                {
                    Offense = "OVI", Statute = "4511.19A1A", Degree = "M1",
                    Plea = "Guilty", Finding = "Guilty",
                    FinesAmount = "375.00", FinesSuspended = "0.00",
                    JailDays = "180",
                    JailDaysSuspended = "180"   // equal → allowed
                }
            ];

            var response = await _client.PostAsJsonAsync("/api/v1/jailccplea", dto);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task PostJailCcPlea_MultipleChargeItems_OneBadSuspension_Returns422()
        {
            // All items are evaluated; a single offending item must trigger 422.
            var dto = ValidJailCcPleaDto();
            dto.ChargeItems =
            [
                new SentencingChargeItemDto
                {
                    Offense = "OVI", JailDays = "30", JailDaysSuspended = "0",
                    FinesAmount = "0", FinesSuspended = "0", Plea = "Guilty", Finding = "Guilty"
                },
                new SentencingChargeItemDto
                {
                    Offense = "DUS", JailDays = "10", JailDaysSuspended = "11",  // bad
                    FinesAmount = "0", FinesSuspended = "0", Plea = "Guilty", Finding = "Guilty"
                }
            ];

            var response = await _client.PostAsJsonAsync("/api/v1/jailccplea", dto);

            Assert.Equal(Unprocessable, response.StatusCode);
        }

        // ── DiversionPleaValidator ─────────────────────────────────────────────
        //
        // check_if_diversion_program_selected: DiversionType must not be empty.
        // Also includes the shared defense counsel rule.

        [Fact]
        public async Task PostDiversionPlea_NullDiversionType_Returns422()
        {
            var dto = ValidDiversionPleaDto();
            dto.DiversionType = null;

            var response = await _client.PostAsJsonAsync("/api/v1/diversionplea", dto);

            Assert.Equal(Unprocessable, response.StatusCode);
        }

        [Fact]
        public async Task PostDiversionPlea_EmptyDiversionType_Returns422()
        {
            var dto = ValidDiversionPleaDto();
            dto.DiversionType = "";

            var response = await _client.PostAsJsonAsync("/api/v1/diversionplea", dto);

            Assert.Equal(Unprocessable, response.StatusCode);
        }

        [Fact]
        public async Task PostDiversionPlea_CounselMissing_WaivedFalse_Returns422()
        {
            var dto = ValidDiversionPleaDto();
            dto.DefenseCounselName = null;
            dto.DefenseCounselWaived = false;

            var response = await _client.PostAsJsonAsync("/api/v1/diversionplea", dto);

            Assert.Equal(Unprocessable, response.StatusCode);
        }

        // ── BondHearingValidator ───────────────────────────────────────────────
        //
        // Three rules: defense counsel, bond modification decision, and bond rules.
        // Note: the existing BondHearingApiTests.PostBondHearing_ReturnsDocx fixture
        // does not supply BondModificationDecision; ValidBondHearingDto() below does.

        [Fact]
        public async Task PostBondHearing_NoBondModificationDecision_Returns422()
        {
            var dto = ValidBondHearingDto();
            dto.BondModificationDecision = null;

            var response = await _client.PostAsJsonAsync("/api/v1/bondhearing", dto);

            Assert.Equal(Unprocessable, response.StatusCode);
        }

        [Fact]
        public async Task PostBondHearing_EmptyBondModificationDecision_Returns422()
        {
            var dto = ValidBondHearingDto();
            dto.BondModificationDecision = "";

            var response = await _client.PostAsJsonAsync("/api/v1/bondhearing", dto);

            Assert.Equal(Unprocessable, response.StatusCode);
        }

        [Fact]
        public async Task PostBondHearing_CounselMissing_WaivedFalse_Returns422()
        {
            var dto = ValidBondHearingDto();
            dto.DefenseCounselName = null;
            dto.DefenseCounselWaived = false;

            var response = await _client.PostAsJsonAsync("/api/v1/bondhearing", dto);

            Assert.Equal(Unprocessable, response.StatusCode);
        }

        [Fact]
        public async Task PostBondHearing_AllRulesSatisfied_Returns200WithDocx()
        {
            // Provides all required fields including BondModificationDecision.
            var response = await _client.PostAsJsonAsync("/api/v1/bondhearing", ValidBondHearingDto());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                response.Content.Headers.ContentType?.MediaType);
        }

        // ── LeapSentencingValidator — plea date ───────────────────────────────
        //
        // check_leap_plea_date: LeapPleaDate must not be null and must be before today.

        [Fact]
        public async Task PostLeapSentencing_NullLeapPleaDate_Returns422()
        {
            var dto = ValidLeapSentencingDto();
            dto.LeapPleaDate = null;

            var response = await _client.PostAsJsonAsync("/api/v1/leapsentencing", dto);

            Assert.Equal(Unprocessable, response.StatusCode);
        }

        [Fact]
        public async Task PostLeapSentencing_FutureLeapPleaDate_Returns422()
        {
            var dto = ValidLeapSentencingDto();
            dto.LeapPleaDate = DateTime.Today.AddDays(1);

            var response = await _client.PostAsJsonAsync("/api/v1/leapsentencing", dto);

            Assert.Equal(Unprocessable, response.StatusCode);
        }

        [Fact]
        public async Task PostLeapSentencing_TodayAsLeapPleaDate_Returns422()
        {
            // MustBeInPast requires date < today, so today itself is rejected.
            var dto = ValidLeapSentencingDto();
            dto.LeapPleaDate = DateTime.Today;

            var response = await _client.PostAsJsonAsync("/api/v1/leapsentencing", dto);

            Assert.Equal(Unprocessable, response.StatusCode);
        }

        [Fact]
        public async Task PostLeapSentencing_CounselMissing_WaivedFalse_Returns422()
        {
            var dto = ValidLeapSentencingDto();
            dto.DefenseCounselName = null;
            dto.DefenseCounselWaived = false;

            var response = await _client.PostAsJsonAsync("/api/v1/leapsentencing", dto);

            Assert.Equal(Unprocessable, response.StatusCode);
        }

        // ── SchedulingEntryValidator ───────────────────────────────────────────
        //
        // check_trial_date: JuryTrialDate must not be null and must be after today.
        // check_final_pretrial_date: FinalPretrialDate must be after today when provided.

        [Fact]
        public async Task PostSchedulingEntry_NullJuryTrialDate_Returns422()
        {
            var dto = ValidSchedulingDto();
            dto.JuryTrialDate = null;

            var response = await _client.PostAsJsonAsync("/api/v1/schedulingentry", dto);

            Assert.Equal(Unprocessable, response.StatusCode);
        }

        [Fact]
        public async Task PostSchedulingEntry_PastJuryTrialDate_Returns422()
        {
            var dto = ValidSchedulingDto();
            dto.JuryTrialDate = DateTime.Today.AddDays(-1);

            var response = await _client.PostAsJsonAsync("/api/v1/schedulingentry", dto);

            Assert.Equal(Unprocessable, response.StatusCode);
        }

        [Fact]
        public async Task PostSchedulingEntry_TodayAsJuryTrialDate_Returns422()
        {
            // MustBeInFuture requires date > today, so today itself is rejected.
            var dto = ValidSchedulingDto();
            dto.JuryTrialDate = DateTime.Today;

            var response = await _client.PostAsJsonAsync("/api/v1/schedulingentry", dto);

            Assert.Equal(Unprocessable, response.StatusCode);
        }

        [Fact]
        public async Task PostSchedulingEntry_FinalPretrialDateInPast_Returns422()
        {
            // FinalPretrialDate is conditional: only validated when a value is present.
            var dto = ValidSchedulingDto();
            dto.FinalPretrialDate = DateTime.Today.AddDays(-7);

            var response = await _client.PostAsJsonAsync("/api/v1/schedulingentry", dto);

            Assert.Equal(Unprocessable, response.StatusCode);
        }

        [Fact]
        public async Task PostSchedulingEntry_NullFinalPretrialDate_Returns200WithDocx()
        {
            // FinalPretrialDate is optional — omitting it should not trigger a failure.
            var dto = ValidSchedulingDto();
            dto.FinalPretrialDate = null;

            var response = await _client.PostAsJsonAsync("/api/v1/schedulingentry", dto);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        // ── FineOnlyPleaValidator ──────────────────────────────────────────────
        //
        // Only rule: defense counsel. Tests using this validator confirm that
        // the registered validator fires even for simpler (no bond/date) DTOs.

        [Fact]
        public async Task PostFineOnlyPlea_CounselMissing_WaivedFalse_Returns422()
        {
            var dto = ValidFineOnlyPleaDto();
            dto.DefenseCounselName = null;
            dto.DefenseCounselWaived = false;

            var response = await _client.PostAsJsonAsync("/api/v1/fineonlyplea", dto);

            Assert.Equal(Unprocessable, response.StatusCode);
        }

        [Fact]
        public async Task PostFineOnlyPlea_CounselWaived_NullName_Returns200WithDocx()
        {
            var dto = ValidFineOnlyPleaDto();
            dto.DefenseCounselName = null;
            dto.DefenseCounselWaived = true;

            var response = await _client.PostAsJsonAsync("/api/v1/fineonlyplea", dto);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        // ── Judicial officer rule — split-field validators (NotGuiltyPlea representative) ──
        //
        // Rules.AddJudicialOfficerRule is applied to every validator whose DTO exposes
        // JudicialOfficerFirstName / JudicialOfficerLastName (NotGuiltyPlea, JailCcPlea,
        // FineOnlyPlea, DiversionPlea, DiversionDialog, BondHearing, all LEAP variants).
        // NotGuiltyPleaValidator is used here as the representative.
        // The single-field variant (SchedulingEntry) is exercised in its own region below.

        [Fact]
        public async Task PostNotGuiltyPlea_NullOfficerFirstName_Returns422()
        {
            var dto = ValidNotGuiltyDto();
            dto.JudicialOfficerFirstName = null;

            var response = await _client.PostAsJsonAsync("/api/v1/notguiltyplea", dto);

            Assert.Equal(Unprocessable, response.StatusCode);
        }

        [Fact]
        public async Task PostNotGuiltyPlea_EmptyOfficerLastName_Returns422()
        {
            var dto = ValidNotGuiltyDto();
            dto.JudicialOfficerLastName = "";

            var response = await _client.PostAsJsonAsync("/api/v1/notguiltyplea", dto);

            Assert.Equal(Unprocessable, response.StatusCode);
        }

        [Fact]
        public async Task PostNotGuiltyPlea_BothOfficerFieldsNull_Returns422()
        {
            var dto = ValidNotGuiltyDto();
            dto.JudicialOfficerFirstName = null;
            dto.JudicialOfficerLastName  = null;

            var response = await _client.PostAsJsonAsync("/api/v1/notguiltyplea", dto);

            Assert.Equal(Unprocessable, response.StatusCode);
        }

        [Fact]
        public async Task PostNotGuiltyPlea_WithValidOfficerFields_Returns200WithDocx()
        {
            // Confirm a fully valid DTO still returns 200 after the rule was added.
            var dto = ValidNotGuiltyDto();  // already includes officer fields

            var response = await _client.PostAsJsonAsync("/api/v1/notguiltyplea", dto);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        // ── Judicial officer rule — single-field variant (SchedulingEntry) ────

        [Fact]
        public async Task PostSchedulingEntry_EmptyJudicialOfficer_Returns422()
        {
            var dto = ValidSchedulingDto();
            dto.JudicialOfficer = "";

            var response = await _client.PostAsJsonAsync("/api/v1/schedulingentry", dto);

            Assert.Equal(Unprocessable, response.StatusCode);
        }

        // ── Valid DTO factories ───────────────────────────────────────────────
        //
        // Each factory returns a fully valid DTO so that individual tests can
        // mutate exactly one field to exercise one rule at a time.

        private static NotGuiltyPleaDto ValidNotGuiltyDto() => new()
        {
            DefendantFirstName        = "Test",
            DefendantLastName         = "User",
            CaseNumber                = "2026-CR-100",
            DefenseCounselName        = "Jane Smith",
            DefenseCounselType        = "Public Defender",
            DefenseCounselWaived      = false,
            AppearanceReason          = "Not guilty plea",
            PleaDate                  = DateTime.Today.AddDays(-1),
            Charges                   = "OVI",
            BondType                  = "Cash",
            BondAmount                = "500",
            JudicialOfficerFirstName  = "John",
            JudicialOfficerLastName   = "Doe",
            JudicialOfficerType       = "Judge",
        };

        private static JailCcPleaDto ValidJailCcPleaDto() => new()
        {
            DefendantFirstName        = "Test",
            DefendantLastName         = "User",
            CaseNumber                = "2026-CR-101",
            Date                      = DateTime.Today.AddDays(-1),
            DefenseCounselName        = "Jane Smith",
            DefenseCounselType        = "Public Defender",
            DefenseCounselWaived      = false,
            JudicialOfficerFirstName  = "John",
            JudicialOfficerLastName   = "Doe",
            JudicialOfficerType       = "Judge",
        };

        private static DiversionPleaDto ValidDiversionPleaDto() => new()
        {
            DefendantFirstName        = "Test",
            DefendantLastName         = "User",
            CaseNumber                = "2026-CR-102",
            Date                      = DateTime.Today.AddDays(-1),
            DefenseCounselName        = "Jane Smith",
            DefenseCounselWaived      = false,
            DiversionType             = "Standard",
            JudicialOfficerFirstName  = "John",
            JudicialOfficerLastName   = "Doe",
            JudicialOfficerType       = "Judge",
        };

        private static BondHearingDto ValidBondHearingDto() => new()
        {
            DefendantFirstName        = "Test",
            DefendantLastName         = "User",
            CaseNumber                = "2026-CR-103",
            EntryDate                 = DateTime.Today,
            DefenseCounselName        = "Jane Smith",
            DefenseCounselWaived      = false,
            BondModificationDecision  = "Bond continued",
            BondType                  = "Cash",
            BondAmount                = "1000",
            JudicialOfficerFirstName  = "John",
            JudicialOfficerLastName   = "Doe",
            JudicialOfficerType       = "Judge",
        };

        private static LeapSentencingDto ValidLeapSentencingDto() => new()
        {
            DefendantFirstName        = "Test",
            DefendantLastName         = "User",
            CaseNumber                = "2026-CR-104",
            DefenseCounselName        = "Jane Smith",
            DefenseCounselWaived      = false,
            LeapPleaDate              = DateTime.Today.AddDays(-1),
            JudicialOfficerFirstName  = "John",
            JudicialOfficerLastName   = "Doe",
            JudicialOfficerType       = "Judge",
        };

        private static SchedulingEntryDto ValidSchedulingDto() => new()
        {
            DefendantFirstName = "Test",
            DefendantLastName  = "User",
            CaseNumber         = "2026-CR-105",
            ArrestSummonsDate  = DateTime.Today,
            JudicialOfficer    = "Rohrer",
            JuryTrialDate      = DateTime.Today.AddDays(21),
        };

        private static FineOnlyPleaDto ValidFineOnlyPleaDto() => new()
        {
            DefendantFirstName        = "Test",
            DefendantLastName         = "User",
            CaseNumber                = "2026-CR-106",
            DefenseCounselName        = "Jane Smith",
            DefenseCounselWaived      = false,
            JudicialOfficerFirstName  = "John",
            JudicialOfficerLastName   = "Doe",
            JudicialOfficerType       = "Judge",
        };
    }
}
