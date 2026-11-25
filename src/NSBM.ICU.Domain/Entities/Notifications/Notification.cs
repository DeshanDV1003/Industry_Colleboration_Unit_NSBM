using NSBM.ICU.Domain.Common;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Domain.Entities.Notifications;

/// <summary>An in-app message for one account, written when something that affects them happens.</summary>
public class Notification : BaseEntity
{
    public string RecipientUserId { get; set; } = string.Empty;
    public NotificationKind Kind { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    /// <summary>The web route to open, e.g. "/applications".</summary>
    public string? Link { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ReadAt { get; private set; }

    /// <summary>When the e-mail sender dealt with this notification (sent it, or skipped it by the recipient's choice).</summary>
    public DateTime? EmailHandledAt { get; private set; }

    public bool IsRead => ReadAt is not null;

    public void MarkRead(DateTime at) => ReadAt ??= at;

    public void MarkEmailHandled(DateTime at) => EmailHandledAt ??= at;
}
