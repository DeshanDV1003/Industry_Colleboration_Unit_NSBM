using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Evidence;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.Achievements.Dtos;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.Achievements.Queries.GetMyAchievements;

/// <summary>Streak, level, badges, the skills the student can ask a badge for, and profile-card settings.</summary>
public sealed record GetMyAchievementsQuery : IRequest<AchievementsDto>;

public sealed class GetMyAchievementsQueryHandler(IApplicationDbContext db, ICurrentUser currentUser, IDateTime clock)
    : IRequestHandler<GetMyAchievementsQuery, AchievementsDto>
{
    public async Task<AchievementsDto> Handle(GetMyAchievementsQuery request, CancellationToken ct)
    {
        var student = await db.GetCurrentStudentAsync(currentUser, ct);
        var progress = await db.LoadProgressAsync(student, DateOnly.FromDateTime(clock.UtcNow), ct);

        var badges = await db.SkillBadges.AsNoTracking()
            .Where(b => b.StudentId == student.Id)
            .OrderBy(b => b.Status).ThenByDescending(b => b.ReviewedAt ?? b.RequestedAt)
            .Select(b => new BadgeDto(b.Id, b.SkillId, b.Skill!.Name, b.Status, b.RequestedAt, b.ReviewedAt, b.ReviewerName, b.ReviewNote, b.VerificationCode))
            .ToListAsync(ct);
        var statusBySkill = badges.ToDictionary(b => b.SkillId, b => b.Status);

        var evidence = await db.LoadStudentEvidenceAsync(student.Id, ct);
        var skills = await db.Skills.AsNoTracking().OrderBy(s => s.Name).Select(s => new { s.Id, s.Name }).ToListAsync(ct);
        var eligible = skills
            .Where(s => evidence.For(s.Id) == SkillEvidence.PracticeSignal)
            .Select(s => new BadgeEligibleSkillDto(s.Id, s.Name, evidence.SourcesFor(s.Id), statusBySkill.TryGetValue(s.Id, out var status) ? status : null))
            .ToList();

        var certificates = await db.EventRegistrations.CountAsync(r => r.StudentId == student.Id && r.CertificateCode != null, ct);

        return new AchievementsDto(progress, badges, eligible, certificates,
            new ProfileCardSettingsDto(student.ShareProfileCard, student.ShareProfileCard ? student.ProfileCardSlug : null));
    }
}
