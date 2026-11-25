using NSBM.ICU.Domain.Common;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Domain.Entities.Notifications;

/// <summary>A message the Unit admin sent to a whole audience. Each recipient also gets a notification.</summary>
public class Announcement : BaseEntity
{
    public AnnouncementAudience Audience { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string SentByName { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
    public int RecipientCount { get; set; }
}
