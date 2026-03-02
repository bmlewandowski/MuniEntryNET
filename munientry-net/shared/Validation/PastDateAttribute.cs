using System;
using System.ComponentModel.DataAnnotations;

namespace Munientry.Shared.Validation
{
    /// <summary>
    /// Validates that a <see cref="DateTime"/> or <see cref="DateTime?"/> value is strictly
    /// before today (i.e. in the past).  Mirrors the Python <c>check_plea_date</c> /
    /// <c>check_leap_plea_date</c> required checks in <c>crim_checks.py</c>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class PastDateAttribute : ValidationAttribute
    {
        public PastDateAttribute()
            : base("The {0} must be a date prior to today.") { }

        public override bool IsValid(object? value)
        {
            if (value is null) return true; // let [Required] handle the null case
            var date = value switch
            {
                DateTime dt => dt,
                DateTimeOffset dto => dto.DateTime,
                _ => (DateTime?)null
            };
            return date.HasValue && date.Value.Date < DateTime.Today;
        }

        public override string FormatErrorMessage(string name)
            => $"The {name} must be a date prior to today.";
    }
}
