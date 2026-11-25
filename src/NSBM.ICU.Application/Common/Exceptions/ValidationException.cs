using FluentValidation.Results;

namespace NSBM.ICU.Application.Common.Exceptions;

/// <summary>Raised by the validation pipeline when one or more request validators fail.</summary>
public sealed class ValidationException(IEnumerable<ValidationFailure> failures)
    : Exception("One or more validation errors occurred.")
{
    public IReadOnlyDictionary<string, string[]> Errors { get; } = failures
        .GroupBy(f => f.PropertyName, f => f.ErrorMessage)
        .ToDictionary(g => g.Key, g => g.Distinct().ToArray());
}
