namespace NSBM.ICU.Domain.Entities.Notifications;

/// <summary>
/// One person's e-mail choices. A person without a row gets the defaults (everything on). Security alerts are
/// always e-mailed, whatever is chosen here.
/// </summary>
public class EmailPreference
{
    public string UserId { get; set; } = string.Empty;
    public bool NotificationsByEmail { get; set; } = true;
    public bool WeeklyDigestByEmail { get; set; } = true;
    /// <summary>The last weekly digest sent, so a restart never sends the same week twice.</summary>
    public DateTime? LastDigestSentAt { get; set; }
}
