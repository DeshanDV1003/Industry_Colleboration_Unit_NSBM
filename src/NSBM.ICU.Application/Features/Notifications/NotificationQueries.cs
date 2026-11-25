using NSBM.ICU.Application.Features.Notifications.Dtos;
using NSBM.ICU.Domain.Entities.Notifications;

namespace NSBM.ICU.Application.Features.Notifications;

/// <summary>Shared filters and projections for the signed-in user's notifications.</summary>
internal static class NotificationQueries
{
    public static IQueryable<Notification> For(this IQueryable<Notification> source, string? userId) =>
        source.Where(n => n.RecipientUserId == userId);

    public static IQueryable<NotificationDto> ToDtos(this IQueryable<Notification> source) =>
        source.OrderByDescending(n => n.CreatedAt).ThenByDescending(n => n.Id)
            .Select(n => new NotificationDto(n.Id, n.Kind, n.Title, n.Body, n.Link, n.CreatedAt, n.ReadAt != null));
}
