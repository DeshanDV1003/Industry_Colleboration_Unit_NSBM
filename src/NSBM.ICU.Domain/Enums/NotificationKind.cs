namespace NSBM.ICU.Domain.Enums;

/// <summary>What happened, so the web app can choose an icon and the user can filter.</summary>
public enum NotificationKind
{
    ApplicationUpdate = 1,
    MentorReply = 2,
    TaskReview = 3,
    LecturerNote = 4,
    TaskAssigned = 5,
    ContentReviewed = 6,
    ContentSubmitted = 7,
    CalendarEvent = 8,
    IndustryEvent = 9,
    CertificateIssued = 10,
    BadgeRequested = 11,
    BadgeReviewed = 12,
    PartnerEnquiry = 13,
    HallAssigned = 14,
    HallBookingCancelled = 15,
    Announcement = 16,
    HallNeeded = 17,
}
