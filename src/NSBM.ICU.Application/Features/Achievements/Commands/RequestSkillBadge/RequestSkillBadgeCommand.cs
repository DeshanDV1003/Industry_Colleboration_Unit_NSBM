using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Evidence;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Common.Notifications;
using NSBM.ICU.Domain.Entities.Profiles;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.Achievements.Commands.RequestSkillBadge;

/// <summary>
/// Asks a lecturer to review a skill badge. Allowed only when the platform has a practice signal for the skill;
/// the evidence is captured at request time so the reviewer sees exactly what supported it.
/// </summary>
public sealed record RequestSkillBadgeCommand(int SkillId, string? Note) : IRequest<int>;

public sealed class RequestSkillBadgeCommandValidator : AbstractValidator<RequestSkillBadgeCommand>
{
    public RequestSkillBadgeCommandValidator()
    {
        RuleFor(x => x.SkillId).GreaterThan(0);
        RuleFor(x => x.Note).MaximumLength(500);
    }
}

public sealed class RequestSkillBadgeCommandHandler(IApplicationDbContext db, ICurrentUser currentUser, IDateTime clock)
    : IRequestHandler<RequestSkillBadgeCommand, int>
{
    public async Task<int> Handle(RequestSkillBadgeCommand request, CancellationToken ct)
    {
        var student = await db.GetCurrentStudentAsync(currentUser, ct);
        var skillName = await db.Skills.Where(s => s.Id == request.SkillId).Select(s => s.Name).FirstOrDefaultAsync(ct)
                        ?? throw NotFoundException.For("Skill", request.SkillId);

        var evidence = await db.LoadStudentEvidenceAsync(student.Id, ct);
        if (evidence.For(request.SkillId) != SkillEvidence.PracticeSignal)
            throw new BusinessRuleException("Practise this skill first: a correct baseline answer, a fully passed industry task or a knowledge check that uses it.");
        var summary = string.Join('\n', evidence.SourcesFor(request.SkillId));

        var now = clock.UtcNow;
        var badge = await db.SkillBadges.FirstOrDefaultAsync(b => b.StudentId == student.Id && b.SkillId == request.SkillId, ct);
        if (badge is null)
        {
            badge = SkillBadge.Request(student.Id, request.SkillId, summary, request.Note, now);
            db.SkillBadges.Add(badge);
        }
        else
        {
            badge.RequestAgain(summary, request.Note, now);
        }

        db.Notify(await db.Lecturers.Select(l => l.UserId).ToListAsync(ct), NotificationKind.BadgeRequested,
            $"Badge request: {skillName}", $"{student.FullName} ({student.StudentNo}) asked for a badge.", NotificationLinks.BadgeReviews, now);
        await db.SaveChangesAsync(ct);
        return badge.Id;
    }
}
