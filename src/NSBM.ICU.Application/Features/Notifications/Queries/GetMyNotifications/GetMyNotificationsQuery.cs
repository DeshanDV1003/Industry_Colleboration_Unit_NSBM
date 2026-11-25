using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.Notifications.Dtos;

namespace NSBM.ICU.Application.Features.Notifications.Queries.GetMyNotifications;

/// <summary>The newest notifications for the signed-in user, plus how many are unread.</summary>
public sealed record GetMyNotificationsQuery(int Take = 50, bool UnreadOnly = false) : IRequest<NotificationListDto>;

public sealed class GetMyNotificationsQueryValidator : AbstractValidator<GetMyNotificationsQuery>
{
    public GetMyNotificationsQueryValidator() => RuleFor(x => x.Take).InclusiveBetween(1, 200);
}

public sealed class GetMyNotificationsQueryHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IRequestHandler<GetMyNotificationsQuery, NotificationListDto>
{
    public async Task<NotificationListDto> Handle(GetMyNotificationsQuery request, CancellationToken ct)
    {
        var mine = db.Notifications.AsNoTracking().For(currentUser.UserId);
        var unread = await mine.CountAsync(n => n.ReadAt == null, ct);
        var items = await (request.UnreadOnly ? mine.Where(n => n.ReadAt == null) : mine).ToDtos().Take(request.Take).ToListAsync(ct);
        return new NotificationListDto(unread, items);
    }
}
