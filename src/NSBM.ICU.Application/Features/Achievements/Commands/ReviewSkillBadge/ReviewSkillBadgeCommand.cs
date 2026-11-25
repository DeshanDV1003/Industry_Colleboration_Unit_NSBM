using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Common.Models;
using NSBM.ICU.Application.Common.Moderation;
using NSBM.ICU.Application.Common.Notifications;
using NSBM.ICU.Domain.Enums;
using NSBM.ICU.Domain.Rules;

namespace NSBM.ICU.Application.Features.Achievements.Commands.ReviewSkillBadge;

/// <summary>A lecturer approves a badge (giving it a verification code) or declines it with advice.</summary>
public sealed record ReviewSkillBadgeCommand(int Id, ReviewDecision Decision, string? Note) : IRequest, IReviewCommand;

public sealed class ReviewSkillBadgeCommandValidator : ReviewCommandValidator<ReviewSkillBadgeCommand>;

public sealed class ReviewSkillBadgeCommandHandler(IApplicationDbContext db, ICurrentUser currentUser, IDateTime clock)
    : IRequestHandler<ReviewSkillBadgeCommand>
{
    public async Task Handle(ReviewSkillBadgeCommand request, CancellationToken ct)
    {
        var lecturer = await db.GetCurrentLecturerAsync(currentUser, ct);
        var badge = await db.SkillBadges.Include(b => b.Skill).FirstOrDefaultAsync(b => b.Id == request.Id, ct)
                    ?? throw NotFoundException.For("Badge request", request.Id);

        var now = clock.UtcNow;
        var approved = request.Decision == ReviewDecision.Approve;
        if (approved)
        {
            string code;
            do code = ShareCodeRules.NewBadgeCode();
            while (await db.SkillBadges.AnyAsync(b => b.VerificationCode == code, ct));
            badge.Approve(lecturer.FullName, code, request.Note, now);
        }
        else
        {
            badge.Decline(lecturer.FullName, request.Note!, now);
        }

        db.Notify(await db.StudentUserIdAsync(badge.StudentId, ct), NotificationKind.BadgeReviewed,
            approved ? $"Badge earned: {badge.Skill!.Name}" : $"Badge not yet: {badge.Skill!.Name}",
            approved ? $"{lecturer.FullName} approved your badge. It is now on your profile card." : $"{lecturer.FullName}: {request.Note}",
            NotificationLinks.Achievements, now);
        await db.SaveChangesAsync(ct);
    }
}
