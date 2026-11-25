using FluentValidation;
using MediatR;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.IndustryTasks.Models;
using NSBM.ICU.Domain.Entities.Tasks;
using NSBM.ICU.Application.Common.Notifications;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.IndustryTasks.Commands.CreateIndustryTask;

/// <summary>Creates a task for the partner's company. It starts as Pending until a Unit admin reviews it.</summary>
public sealed record CreateIndustryTaskCommand(IndustryTaskInput Task) : IRequest<int>;

public sealed class CreateIndustryTaskCommandValidator : AbstractValidator<CreateIndustryTaskCommand>
{
    public CreateIndustryTaskCommandValidator()
    {
        RuleFor(x => x.Task).NotNull().SetValidator(new IndustryTaskInputValidator());
    }
}

public sealed class CreateIndustryTaskCommandHandler(IApplicationDbContext db, ICurrentUser currentUser, IDateTime clock)
    : IRequestHandler<CreateIndustryTaskCommand, int>
{
    public async Task<int> Handle(CreateIndustryTaskCommand request, CancellationToken cancellationToken)
    {
        var partner = await db.GetCurrentPartnerAsync(currentUser, cancellationToken);
        await db.EnsureReferencesExistAsync(request.Task, cancellationToken);

        var task = new IndustryTask { CompanyId = partner.CompanyId, AuthorPartnerId = partner.Id };
        request.Task.ApplyTo(task);

        db.IndustryTasks.Add(task);
        db.Notify(await db.AdminUserIdsAsync(cancellationToken), NotificationKind.ContentSubmitted,
            $"New industry task to review: {task.Title}", $"{partner.FullName} submitted it for approval.", NotificationLinks.TaskApprovals, clock.UtcNow);
        await db.SaveChangesAsync(cancellationToken);
        return task.Id;
    }
}
