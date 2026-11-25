using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Common.Models;
using NSBM.ICU.Application.Common.Moderation;
using NSBM.ICU.Application.Common.Notifications;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.Vacancies.Commands.ReviewVacancy;

/// <summary>Publishes a pending vacancy in the match center, or rejects it with a reason for the company.</summary>
public sealed record ReviewVacancyCommand(int Id, ReviewDecision Decision, string? Note) : IRequest, IReviewCommand;

public sealed class ReviewVacancyCommandValidator : ReviewCommandValidator<ReviewVacancyCommand>;

public sealed class ReviewVacancyCommandHandler(IApplicationDbContext db, ICurrentUser currentUser, IDateTime clock)
    : IRequestHandler<ReviewVacancyCommand>
{
    public async Task Handle(ReviewVacancyCommand request, CancellationToken cancellationToken)
    {
        var reviewer = await db.GetCurrentAdministratorAsync(currentUser, cancellationToken);
        var vacancy = await db.Vacancies.FirstOrDefaultAsync(v => v.Id == request.Id, cancellationToken)
                      ?? throw NotFoundException.For("Vacancy", request.Id);

        vacancy.ApplyReview(request, reviewer.FullName, clock.UtcNow);
        var approved = request.Decision == ReviewDecision.Approve;
        db.Notify(await db.CompanyUserIdsAsync(vacancy.CompanyId, cancellationToken), NotificationKind.ContentReviewed,
            approved ? $"Approved: {vacancy.Title}" : $"Changes needed: {vacancy.Title}",
            approved ? "Your vacancy is now live for students." : $"The Unit asked for changes: {request.Note}",
            NotificationLinks.PartnerVacancies, clock.UtcNow);
        await db.SaveChangesAsync(cancellationToken);
    }
}
