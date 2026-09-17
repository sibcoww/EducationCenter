using System.ComponentModel.DataAnnotations;

namespace EducationCenter.Api.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
public sealed class BirthDateAttribute : ValidationAttribute
{
    public BirthDateAttribute()
        : base("{0} is required and must not be in the future (UTC).")
    {
    }

    public override bool IsValid(object? value) =>
        value is DateOnly date &&
        date != DateOnly.MinValue &&
        date <= DateOnly.FromDateTime(DateTime.UtcNow);
}
