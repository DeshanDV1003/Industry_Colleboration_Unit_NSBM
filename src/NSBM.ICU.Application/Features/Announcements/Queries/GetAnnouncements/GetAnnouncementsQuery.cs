using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.Announcements.Dtos;

namespace NSBM.ICU.Application.Features.Announcements.Queries.GetAnnouncements;

/// <summary>Announcements already sent, newest first.</summary>
public sealed record GetAnnouncementsQuery : IRequest<IReadOnlyList<AnnouncementDto>>;

public sealed class GetAnnouncementsQueryHandler(IApplicationDbContext db) : IRequestHandler<GetAnnouncementsQuery, IReadOnlyList<AnnouncementDto>>
{
    public async Task<IReadOnlyList<AnnouncementDto>> Handle(GetAnnouncementsQuery request, CancellationToken ct) =>
        await db.Announcements.AsNoTracking()
            .OrderByDescending(a => a.SentAt)
            .Select(a => new AnnouncementDto(a.Id, a.Audience, a.Title, a.Message, a.SentByName, a.SentAt, a.RecipientCount))
            .ToListAsync(ct);
}
