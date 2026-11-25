using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Common.Models;
using NSBM.ICU.Application.Common.Moderation;
using NSBM.ICU.Application.Common.Notifications;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.IndustryEvents.Commands.ReviewIndustryEvent;

/// <summary>Publishes a pending event to students, or returns it to the company with a reason.</summary>
public sealed record ReviewIndustryEventCommand(int Id, ReviewDecision Decision, string? Note) : IRequest, IReviewCommand;

public sealed class ReviewIndustryEventCommandValidator : ReviewCommandValidator<ReviewIndustryEventCommand>;

public sealed class ReviewIndustryEventCommandHandler(IApplicationDbContext db, ICurrentUser currentUser, IDateTime clock)
    : IRequestHandler<ReviewIndustryEventCommand>
{
    public async Task Handle(ReviewIndustryEventCommand request, CancellationToken ct)
    {
        var reviewer = await db.GetCurrentAdministratorAsync(currentUser, ct);
        var industryEvent = await db.IndustryEvents.Include(e => e.Company).FirstOrDefaultAsync(e => e.Id == request.Id, ct)
                            ?? throw NotFoundException.For("Event", request.Id);

        industryEvent.ApplyReview(request, reviewer.FullName, clock.UtcNow);
        var approved = request.Decision == ReviewDecision.Approve;
        db.Notify(await db.CompanyUserIdsAsync(industryEvent.CompanyId, ct), NotificationKind.ContentReviewed,
            approved ? $"Approved: {industryEvent.Title}" : $"Changes needed: {industryEvent.Title}",
            approved ? "Your event is now open for student registration." : $"The Unit asked for changes: {request.Note}",
            NotificationLinks.PartnerEvents, clock.UtcNow);

        // Approval is the moment students can register, so every student hears about it,
        // and the registrar can allocate a hall for an on-campus event.
        if (approved)
        {
            db.Notify(await db.RegistrarUserIdsAsync(ct), NotificationKind.HallNeeded,
                $"Hall needed: {industryEvent.Title}",
                $"{industryEvent.Company!.Name}, {industryEvent.StartDate:d MMM yyyy}, up to {industryEvent.Capacity} people.",
                NotificationLinks.HallBookings, clock.UtcNow);
            db.Notify(await db.Students.Select(s => s.UserId).ToListAsync(ct), NotificationKind.IndustryEvent,
                $"New event: {industryEvent.Title}",
                $"{industryEvent.Company!.Name} is hosting it on {industryEvent.StartDate:d MMM}. Register by {industryEvent.RegistrationClosesOn:d MMM}.",
                NotificationLinks.Event(industryEvent.Id), clock.UtcNow);
        }

        await db.SaveChangesAsync(ct);
    }
}
