using FluentValidation;

namespace NSBM.ICU.Application.Common.Validation;

/// <summary>
/// The password policy, checked before the request reaches Identity so the message is friendly.
/// Mirrors the Identity options in Infrastructure (8+ characters with a digit).
/// </summary>
public static class PasswordRules
{
    public const int MinLength = 8;

    public static IRuleBuilderOptions<T, string> ValidPassword<T>(this IRuleBuilder<T, string> rule) =>
        rule.NotEmpty().WithMessage("Enter a password.")
            .MinimumLength(MinLength).WithMessage($"Use at least {MinLength} characters.")
            .Matches("[0-9]").WithMessage("Include at least one number.")
            .MaximumLength(100);
}
