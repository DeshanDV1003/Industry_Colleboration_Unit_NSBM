using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.Announcements.Dtos;

public sealed record AnnouncementDto(
    int Id,
    AnnouncementAudience Audience,
    string Title,
    string Message,
    string SentByName,
    DateTime SentAt,
    int RecipientCount);
