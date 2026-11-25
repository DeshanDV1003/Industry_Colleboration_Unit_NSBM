using FluentValidation;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.Vacancies.Models;

/// <summary>Everything a partner provides when posting or editing an internship vacancy.</summary>
public sealed record VacancyInput(
    string Title,
    string Description,
    int CareerPathId,
    WorkMode WorkMode,
    string Location,
    int DurationMonths,
    DateOnly ClosingDate,
    IReadOnlyList<int> RequiredSkillIds,
    IReadOnlyList<int> PreferredSkillIds);

public sealed class VacancyInputValidator : AbstractValidator<VacancyInput>
{
    public VacancyInputValidator(IDateTime clock)
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(4000);
        RuleFor(x => x.CareerPathId).GreaterThan(0).WithMessage("Choose the job role this vacancy belongs to.");
        RuleFor(x => x.WorkMode).IsInEnum();
        RuleFor(x => x.Location).NotEmpty().MaximumLength(150);
        RuleFor(x => x.DurationMonths).InclusiveBetween(1, 12);
        RuleFor(x => x.ClosingDate)
            .GreaterThan(_ => DateOnly.FromDateTime(clock.UtcNow))
            .WithMessage("The closing date must be in the future.");

        RuleFor(x => x.RequiredSkillIds).NotEmpty().WithMessage("List at least one required skill.");
        RuleFor(x => x.RequiredSkillIds).Must(ids => ids.Distinct().Count() == ids.Count).WithMessage("Each skill can be listed once.");
        RuleFor(x => x.PreferredSkillIds).Must(ids => ids.Distinct().Count() == ids.Count).WithMessage("Each skill can be listed once.");
        RuleFor(x => x)
            .Must(x => !x.RequiredSkillIds.Intersect(x.PreferredSkillIds).Any())
            .WithName(nameof(VacancyInput.PreferredSkillIds))
            .WithMessage("A skill cannot be both required and preferred.");
    }
}
