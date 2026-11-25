using FluentValidation;
using MediatR;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.Planner.Models;

namespace NSBM.ICU.Application.Features.Planner.Commands.UpdatePlannerItem;

public sealed record UpdatePlannerItemCommand(int Id, PlannerItemInput Item) : IRequest;

public sealed class UpdatePlannerItemCommandValidator : AbstractValidator<UpdatePlannerItemCommand>
{
    public UpdatePlannerItemCommandValidator() => RuleFor(x => x.Item).NotNull().SetValidator(new PlannerItemInputValidator());
}

public sealed class UpdatePlannerItemCommandHandler(IApplicationDbContext db, ICurrentUser currentUser) : IRequestHandler<UpdatePlannerItemCommand>
{
    public async Task Handle(UpdatePlannerItemCommand request, CancellationToken cancellationToken)
    {
        var item = await db.GetOwnPlannerItemAsync(currentUser, request.Id, cancellationToken);
        item.Kind = request.Item.Kind;
        item.Title = request.Item.Title.Trim();
        item.Notes = string.IsNullOrWhiteSpace(request.Item.Notes) ? null : request.Item.Notes.Trim();
        item.DueDate = request.Item.DueDate;
        await db.SaveChangesAsync(cancellationToken);
    }
}
