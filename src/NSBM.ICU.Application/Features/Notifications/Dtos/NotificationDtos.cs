using NSBM.ICU.Application.Features.Calendar.Dtos;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.Notifications.Dtos;

public sealed record NotificationDto(int Id, NotificationKind Kind, string Title, string Body, string? Link, DateTime CreatedAt, bool IsRead);

public sealed record NotificationListDto(int UnreadCount, IReadOnlyList<NotificationDto> Items);

/// <summary>A suggested next action. The web app turns the kind into wording and a link.</summary>
public enum NextStepKind
{
    ChooseCareerPath = 1,
    TakeBaseline = 2,
    ContinueRoadmap = 3,
    OverduePlannerItems = 4,
    TryAssignedTask = 5,
    ReviewPendingContent = 6,
    AllocateEventHalls = 7,
}

public sealed record NextStepDto(NextStepKind Kind, string? Subject = null, int? Count = null, int? TargetId = null);

/// <summary>"Your week": what is coming up in the next seven days, unread messages and suggested next steps.</summary>
public sealed record WeeklyDigestDto(
    DateOnly From,
    DateOnly To,
    IReadOnlyList<CalendarEntryDto> Upcoming,
    int UnreadCount,
    IReadOnlyList<NotificationDto> RecentUnread,
    IReadOnlyList<NextStepDto> NextSteps);
