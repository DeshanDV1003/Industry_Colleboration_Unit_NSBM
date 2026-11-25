namespace NSBM.ICU.Application.Features.EmailPreferences.Dtos;

/// <summary>A person's e-mail choices. <see cref="EmailEnabled"/> says whether this server sends e-mail at all.</summary>
public sealed record EmailPreferencesDto(bool NotificationsByEmail, bool WeeklyDigestByEmail, bool EmailEnabled);
