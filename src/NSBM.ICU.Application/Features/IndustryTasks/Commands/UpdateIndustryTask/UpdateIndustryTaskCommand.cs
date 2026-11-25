using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Common.Moderation;
using NSBM.ICU.Application.Features.IndustryTasks.Models;

namespace NSBM.ICU.Application.Features.IndustryTasks.Commands.UpdateIndustryTask;

/// <summary>Edits a pending or rejected task and sends it back to the review queue.</summary>
public sealed record UpdateIndustryTaskCommand(int Id, IndustryTaskInput Task) : IRequest;

public sealed class UpdateIndustryTaskCommandValidator : AbstractValidator<UpdateIndustryTaskCommand>
{
    public UpdateIndustryTaskCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Task).NotNull().SetValidator(new IndustryTaskInputValidator());
    }
}

public sealed class UpdateIndustryTaskCommandHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IRequestHandler<UpdateIndustryTaskCommand>
{
    public async Task Handle(UpdateIndustryTaskCommand request, CancellationToken cancellationToken)
    {
        var partner = await db.GetCurrentPartnerAsync(currentUser, cancellationToken);

        var task = await db.IndustryTasks
            .Include(t => t.Skills)
            .Include(t => t.Checks)
            .Include(t => t.KnowledgeQuestions)
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken)
            ?? throw NotFoundException.For("Task", request.Id);

        task.EnsureEditableBy(partner.CompanyId);

        await db.EnsureReferencesExistAsync(request.Task, cancellationToken);
        request.Task.ApplyTo(task);
        task.ResubmitForReview();

        await db.SaveChangesAsync(cancellationToken);
    }
}
