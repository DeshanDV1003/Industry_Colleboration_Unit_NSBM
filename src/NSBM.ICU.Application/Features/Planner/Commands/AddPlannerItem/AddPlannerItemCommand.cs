using FluentValidation;
using MediatR;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.Planner.Models;
using NSBM.ICU.Domain.Entities.Planning;

namespace NSBM.ICU.Application.Features.Planner.Commands.AddPlannerItem;

/// <summary>Adds a goal, task or deadline — for example from a skill gap or interview practice.</summary>
public sealed record AddPlannerItemCommand(PlannerItemInput Item) : IRequest<int>;

public sealed class AddPlannerItemCommandValidator : AbstractValidator<AddPlannerItemCommand>
{
    public AddPlannerItemCommandValidator() => RuleFor(x => x.Item).NotNull().SetValidator(new PlannerItemInputValidator());
}

public sealed class AddPlannerItemCommandHandler(IApplicationDbContext db, ICurrentUser currentUser) : IRequestHandler<AddPlannerItemCommand, int>
{
    public async Task<int> Handle(AddPlannerItemCommand request, CancellationToken cancellationToken)
    {
        var student = await db.GetCurrentStudentAsync(currentUser, cancellationToken);
        var item = new PlannerItem
        {
            StudentId = student.Id,
            Kind = request.Item.Kind,
            Title = request.Item.Title.Trim(),
            Notes = string.IsNullOrWhiteSpace(request.Item.Notes) ? null : request.Item.Notes.Trim(),
            DueDate = request.Item.DueDate,
        };
        db.PlannerItems.Add(item);
        await db.SaveChangesAsync(cancellationToken);
        return item.Id;
    }
}
