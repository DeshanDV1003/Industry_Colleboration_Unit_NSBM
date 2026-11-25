using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Domain.Enums;
using NSBM.ICU.Application.Common.Notifications;

namespace NSBM.ICU.Application.Features.Candidates.Commands.UpdateApplicationStatus;

/// <summary>A person at the company moves an application (e.g. shortlist, invite to interview). The student sees it immediately.</summary>
public sealed record UpdateApplicationStatusCommand(int ApplicationId, ApplicationStatus Status) : IRequest;

public sealed class UpdateApplicationStatusCommandValidator : AbstractValidator<UpdateApplicationStatusCommand>
{
    public UpdateApplicationStatusCommandValidator()
    {
        RuleFor(x => x.ApplicationId).GreaterThan(0);
        RuleFor(x => x.Status).IsInEnum()
            .NotEqual(ApplicationStatus.Submitted).WithMessage("An application cannot be moved back to Submitted.");
    }
}

public sealed class UpdateApplicationStatusCommandHandler(IApplicationDbContext db, ICurrentUser currentUser, IDateTime clock)
    : IRequestHandler<UpdateApplicationStatusCommand>
{
    public async Task Handle(UpdateApplicationStatusCommand request, CancellationToken cancellationToken)
    {
        var partner = await db.GetCurrentPartnerAsync(currentUser, cancellationToken);
        var application = await db.InternshipApplications
            .Include(a => a.Vacancy)
            .FirstOrDefaultAsync(a => a.Id == request.ApplicationId, cancellationToken)
            ?? throw NotFoundException.For("Application", request.ApplicationId);

        if (application.Vacancy!.CompanyId != partner.CompanyId)
            throw new ForbiddenAccessException("This application is for another company's vacancy.");

        if (application.Status == request.Status) return;

        application.ChangeStatus(request.Status, clock.UtcNow);
        var companyName = await db.Companies.Where(c => c.Id == partner.CompanyId).Select(c => c.Name).SingleAsync(cancellationToken);
        db.Notify(await db.StudentUserIdAsync(application.StudentId, cancellationToken), NotificationKind.ApplicationUpdate,
            $"Application update: {application.Vacancy.Title}",
            $"{companyName} moved your application to \"{StatusText(request.Status)}\".",
            NotificationLinks.Applications, clock.UtcNow);
        await db.SaveChangesAsync(cancellationToken);
    }

    private static string StatusText(ApplicationStatus status) => status switch
    {
        ApplicationStatus.UnderReview => "Under review",
        ApplicationStatus.InterviewInvited => "Invited to interview",
        ApplicationStatus.NotSelected => "Not selected",
        _ => status.ToString(),
    };
}
