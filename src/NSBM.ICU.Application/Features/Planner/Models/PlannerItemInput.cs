using FluentValidation;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.Planner.Models;

public sealed record PlannerItemInput(PlannerItemKind Kind, string Title, string? Notes, DateOnly? DueDate);

public sealed class PlannerItemInputValidator : AbstractValidator<PlannerItemInput>
{
    public PlannerItemInputValidator()
    {
        RuleFor(x => x.Kind).IsInEnum();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Notes).MaximumLength(1000);
        RuleFor(x => x.DueDate).NotNull().When(x => x.Kind == PlannerItemKind.Deadline).WithMessage("A deadline needs a due date.");
    }
}
