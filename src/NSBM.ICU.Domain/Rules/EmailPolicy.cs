using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Domain.Rules;

/// <summary>Which notifications also go out by e-mail.</summary>
public static class EmailPolicy
{
    /// <summary>
    /// Broadcasts sent to a whole audience (a new calendar event, a newly approved partner event) stay in the app;
    /// they would flood inboxes. They are listed in the weekly digest instead.
    /// </summary>
    private static readonly HashSet<NotificationKind> InAppOnly = [NotificationKind.CalendarEvent, NotificationKind.IndustryEvent];

    /// <summary>Account security alerts reach the person even if they turned e-mails off.</summary>
    public static bool IsSecurityAlert(NotificationKind kind) => kind == NotificationKind.AccountSecurity;

    public static bool ShouldEmail(NotificationKind kind, bool personWantsEmails) =>
        IsSecurityAlert(kind) || (personWantsEmails && !InAppOnly.Contains(kind));
}
