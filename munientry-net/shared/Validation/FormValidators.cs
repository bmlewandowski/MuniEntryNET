using FluentValidation;
using Munientry.Shared.Dtos;
using System.Linq.Expressions;

namespace Munientry.Shared.Validation;

// ─────────────────────────────────────────────────────────────────────────────
// Shared constants (mirrors Python crim_checks.py / scheduling_checks.py)
// ─────────────────────────────────────────────────────────────────────────────

file static class Rules
{
    /// Bond types that do not carry a dollar amount — mirrors Python NO_BOND_AMOUNT_TYPES.
    internal static readonly IReadOnlySet<string> NoAmountBondTypes =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Recognizance (OR) Bond",
            "Continue Existing Bond",
            "No Bond",
        };

    /// Adds the two bond-amount consistency rules to any validator that has BondType/BondAmount.
    internal static void AddBondRules<T>(AbstractValidator<T> v,
        Expression<Func<T, string?>> bondTypeExpr,
        Expression<Func<T, string?>> bondAmountExpr)
    {
        var getBondType = bondTypeExpr.Compile();

        // check_if_no_bond_amount — bond type requires amount but none set
        v.RuleFor(bondAmountExpr)
            .NotEmpty()
            .Must(a => !string.Equals(a, "None", StringComparison.OrdinalIgnoreCase))
            .When(x => getBondType(x) is { } bt && !NoAmountBondTypes.Contains(bt))
            .WithMessage("A bond amount is required for the selected bond type.");

        // check_if_improper_bond_type — no-amount bond type but an amount was set
        v.RuleFor(bondAmountExpr)
            .Must(a => string.IsNullOrEmpty(a) || string.Equals(a, "None", StringComparison.OrdinalIgnoreCase))
            .When(x => getBondType(x) is { } bt && NoAmountBondTypes.Contains(bt))
            .WithMessage("The selected bond type does not permit a bond amount.");
    }

    /// check_defense_counsel — counsel name required unless waived.
    internal static void AddDefenseCounselRule<T>(AbstractValidator<T> v,
        Expression<Func<T, string?>> counselNameExpr,
        Expression<Func<T, bool>> waivedExpr)
    {
        var getWaived = waivedExpr.Compile();

        v.RuleFor(counselNameExpr)
            .NotEmpty()
            .When(x => !getWaived(x))
            .WithMessage("Defense counsel name is required, or check 'Counsel Waived'.");
    }

    /// Plea / admission date must be prior to today.
    internal static IRuleBuilderOptions<T, DateTime?> MustBeInPast<T>(
        this IRuleBuilder<T, DateTime?> rule) =>
        rule.Must(d => d.HasValue && d.Value.Date < DateTime.Today)
            .WithMessage("The date must be a date prior to today.");

    /// Trial / pretrial date must be after today.
    internal static IRuleBuilderOptions<T, DateTime?> MustBeInFuture<T>(
        this IRuleBuilder<T, DateTime?> rule) =>
        rule.Must(d => d.HasValue && d.Value.Date > DateTime.Today)
            .WithMessage("The date must be a date after today.");
}

// ─────────────────────────────────────────────────────────────────────────────
// Criminal plea validators
// ─────────────────────────────────────────────────────────────────────────────

/// <summary>
/// Mirrors: check_defense_counsel (warning → enforced server-side)
/// + bond rules from BondChecks (check_if_no_bond_amount, check_if_improper_bond_type).
/// Python NotGuiltyBondCheckList does not include check_plea_date.
/// </summary>
public sealed class NotGuiltyPleaValidator : AbstractValidator<NotGuiltyPleaDto>
{
    public NotGuiltyPleaValidator()
    {
        Rules.AddDefenseCounselRule(this, x => x.DefenseCounselName, x => x.DefenseCounselWaived);
        Rules.AddBondRules(this, x => x.BondType, x => x.BondAmount);
    }
}

/// <summary>
/// Mirrors: check_defense_counsel (warning → enforced server-side)
/// + check_if_jail_suspended_more_than_imposed (required hard stop).
/// Python JailCCPleaCheckList does not include check_plea_date.
/// </summary>
public sealed class JailCcPleaValidator : AbstractValidator<JailCcPleaDto>
{
    public JailCcPleaValidator()
    {
        Rules.AddDefenseCounselRule(this, x => x.DefenseCounselName, x => x.DefenseCounselWaived);

        // check_if_jail_suspended_more_than_imposed — suspended days must not exceed imposed days.
        RuleForEach(x => x.ChargeItems)
            .Must(item =>
            {
                if (!int.TryParse(item.JailDays, out var imposed)) return true;
                if (!int.TryParse(item.JailDaysSuspended, out var suspended)) return true;
                return suspended <= imposed;
            })
            .WithMessage("Jail days suspended cannot exceed jail days imposed.");
    }
}

/// <summary>
/// Mirrors: check_defense_counsel (warning → enforced server-side).
/// Python FineOnlyCheckList does not include check_plea_date.
/// </summary>
public sealed class FineOnlyPleaValidator : AbstractValidator<FineOnlyPleaDto>
{
    public FineOnlyPleaValidator()
    {
        Rules.AddDefenseCounselRule(this, x => x.DefenseCounselName, x => x.DefenseCounselWaived);
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// Diversion validators
// ─────────────────────────────────────────────────────────────────────────────

/// <summary>
/// Mirrors: check_if_diversion_program_selected + check_defense_counsel (warning → enforced server-side).
/// Python DiversionCheckList does not include check_plea_date.
/// </summary>
public sealed class DiversionPleaValidator : AbstractValidator<DiversionPleaDto>
{
    public DiversionPleaValidator()
    {
        // check_if_diversion_program_selected — at least one radio btn must be checked
        RuleFor(x => x.DiversionType)
            .NotEmpty()
            .WithMessage("A diversion program must be selected.");

        Rules.AddDefenseCounselRule(this, x => x.DefenseCounselName, x => x.DefenseCounselWaived);
    }
}

/// <summary>
/// Mirrors: check_defense_counsel (warning → enforced server-side).
/// Python DiversionCheckList does not include a diversion date check.
/// </summary>
public sealed class DiversionDialogValidator : AbstractValidator<DiversionDialogDto>
{
    public DiversionDialogValidator()
    {
        Rules.AddDefenseCounselRule(this, x => x.DefenseCounselName, x => x.DefenseCounselWaived);
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// Bond validators
// ─────────────────────────────────────────────────────────────────────────────

/// <summary>
/// Mirrors: check_defense_counsel (warning → enforced server-side)
/// + check_if_no_bond_modification_decision + check_if_no_bond_amount + check_if_improper_bond_type.
/// </summary>
public sealed class BondHearingValidator : AbstractValidator<BondHearingDto>
{
    public BondHearingValidator()
    {
        Rules.AddDefenseCounselRule(this, x => x.DefenseCounselName, x => x.DefenseCounselWaived);

        // check_if_no_bond_modification_decision — a decision must be selected before saving.
        RuleFor(x => x.BondModificationDecision)
            .NotEmpty()
            .WithMessage("A bond modification decision is required.");

        Rules.AddBondRules(this, x => x.BondType, x => x.BondAmount);
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// LEAP validators
// ─────────────────────────────────────────────────────────────────────────────

/// <summary>
/// Mirrors: check_defense_counsel (warning → enforced server-side).
/// Python LeapAdmissionPleaCheckList does not include check_leap_plea_date.
/// </summary>
public sealed class LeapAdmissionPleaValidator : AbstractValidator<LeapAdmissionPleaDto>
{
    public LeapAdmissionPleaValidator()
    {
        Rules.AddDefenseCounselRule(this, x => x.DefenseCounselName, x => x.DefenseCounselWaived);
    }
}

/// <summary>
/// Mirrors: check_defense_counsel (warning → enforced server-side).
/// Python LeapAdmissionPleaCheckList (reused by this dialog) does not include check_leap_plea_date.
/// </summary>
public sealed class LeapAdmissionAlreadyValidValidator : AbstractValidator<LeapAdmissionAlreadyValidDto>
{
    public LeapAdmissionAlreadyValidValidator()
    {
        Rules.AddDefenseCounselRule(this, x => x.DefenseCounselName, x => x.DefenseCounselWaived);
    }
}

/// <summary>Mirrors: check_leap_plea_date (LeapPleaDate) + check_defense_counsel.</summary>
public sealed class LeapSentencingValidator : AbstractValidator<LeapSentencingDto>
{
    public LeapSentencingValidator()
    {
        RuleFor(x => x.LeapPleaDate).NotNull().MustBeInPast();
        Rules.AddDefenseCounselRule(this, x => x.DefenseCounselName, x => x.DefenseCounselWaived);
    }
}

/// <summary>
/// Mirrors: check_defense_counsel (warning → enforced server-side).
/// Python LeapAdmissionPleaCheckList (reused by this dialog) does not include check_leap_plea_date.
/// </summary>
public sealed class LeapValidSentencingValidator : AbstractValidator<LeapValidSentencingDto>
{
    public LeapValidSentencingValidator()
    {
        Rules.AddDefenseCounselRule(this, x => x.DefenseCounselName, x => x.DefenseCounselWaived);
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// Scheduling validators
// ─────────────────────────────────────────────────────────────────────────────

/// <summary>
/// Mirrors: check_trial_date (required, future) + check_final_pretrial_date (required, future).
/// The day-of-week checks (check_day_of_trial / check_day_of_final_pretrial) are warning checks
/// in Python that the user can override — they are not enforced here.
/// </summary>
public sealed class SchedulingEntryValidator : AbstractValidator<SchedulingEntryDto>
{
    public SchedulingEntryValidator()
    {
        RuleFor(x => x.JuryTrialDate).NotNull().MustBeInFuture();

        // Only validate FinalPretrialDate when it has a value (conditional in Python)
        When(x => x.FinalPretrialDate.HasValue, () =>
        {
            RuleFor(x => x.FinalPretrialDate).MustBeInFuture();
        });
    }
}
