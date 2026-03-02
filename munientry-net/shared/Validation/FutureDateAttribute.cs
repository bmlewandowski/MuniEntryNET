using System;
using System.ComponentModel.DataAnnotations;

namespace Munientry.Shared.Validation
{
    /// <summary>
    /// Validates that a <see cref="DateTime"/> or <see cref="DateTime?"/> value is strictly
    /// after today (i.e. in the future).  Mirrors the Python <c>check_trial_date</c> /
    /// <c>check_final_pretrial_date</c> required checks in <c>scheduling_checks.py</c>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class FutureDateAttribute : ValidationAttribute
    {
        public FutureDateAttribute()
            : base("The {0} must be a date after today.") { }

        public override bool IsValid(object? value)
        {
            if (value is null) return true; // let [Required] handle the null case
            var date = value switch
            {
                DateTime dt => dt,
                DateTimeOffset dto => dto.DateTime,
                _ => (DateTime?)null
            };
            return date.HasValue && date.Value.Date > DateTime.Today;
        }

        public override string FormatErrorMessage(string name)
            => $"The {name} must be a date after today.";
    }
}
