using FluentValidation;
using NSBM.ICU.Application.Common.Models;

namespace NSBM.ICU.Application.Common.Moderation;

/// <summary>Rules shared by every review command: a known decision, and a reason whenever content is rejected.</summary>
public abstract class ReviewCommandValidator<TCommand> : AbstractValidator<TCommand>
    where TCommand : IReviewCommand
{
    protected ReviewCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Decision).IsInEnum();
        RuleFor(x => x.Note).MaximumLength(1000);
        RuleFor(x => x.Note).NotEmpty()
            .When(x => x.Decision == ReviewDecision.Reject)
            .WithMessage("Tell the company why this was rejected so they can improve it.");
    }
}
