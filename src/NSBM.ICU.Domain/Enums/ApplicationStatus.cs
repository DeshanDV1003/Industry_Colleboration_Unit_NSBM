namespace NSBM.ICU.Domain.Enums;

/// <summary>Where a practice internship application stands. Every change is made by a person at the company.</summary>
public enum ApplicationStatus
{
    Submitted = 1,
    UnderReview = 2,
    Shortlisted = 3,
    InterviewInvited = 4,
    NotSelected = 5,
}
