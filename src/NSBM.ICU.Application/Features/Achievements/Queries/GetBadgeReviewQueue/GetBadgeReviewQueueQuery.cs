using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.Achievements.Dtos;

namespace NSBM.ICU.Application.Features.Achievements.Queries.GetBadgeReviewQueue;

/// <summary>Every badge request for lecturers; the page filters by status so tab counts stay accurate.</summary>
public sealed record GetBadgeReviewQueueQuery : IRequest<IReadOnlyList<BadgeReviewItemDto>>;

public sealed class GetBadgeReviewQueueQueryHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IRequestHandler<GetBadgeReviewQueueQuery, IReadOnlyList<BadgeReviewItemDto>>
{
    public async Task<IReadOnlyList<BadgeReviewItemDto>> Handle(GetBadgeReviewQueueQuery request, CancellationToken ct)
    {
        await db.GetCurrentLecturerAsync(currentUser, ct);
        var rows = await db.SkillBadges.AsNoTracking()
            .OrderBy(b => b.RequestedAt)
            .Join(db.Students, b => b.StudentId, s => s.Id, (b, s) => new
            {
                b.Id, s.FirstName, s.LastName, s.StudentNo, s.Programme, CareerPath = s.CareerPath!.Name, Skill = b.Skill!.Name,
                b.EvidenceSummary, b.StudentNote, b.RequestedAt, b.Status, b.ReviewerName, b.ReviewNote, b.ReviewedAt,
            })
            .ToListAsync(ct);

        return rows.Select(r => new BadgeReviewItemDto(r.Id, $"{r.FirstName} {r.LastName}", r.StudentNo, r.Programme, r.CareerPath, r.Skill,
                r.EvidenceSummary.Split('\n', StringSplitOptions.RemoveEmptyEntries), r.StudentNote, r.RequestedAt, r.Status,
                r.ReviewerName, r.ReviewNote, r.ReviewedAt))
            .ToList();
    }
}
