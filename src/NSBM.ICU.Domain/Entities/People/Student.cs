using NSBM.ICU.Domain.Entities.Careers;

namespace NSBM.ICU.Domain.Entities.People;

/// <summary>An IT undergraduate preparing for industry work and internships.</summary>
public class Student : Person
{
    public string StudentNo { get; set; } = string.Empty;
    public string Programme { get; set; } = string.Empty;
    public string Batch { get; set; } = string.Empty;
    public int AcademicYear { get; set; }
    public bool ShareProfileWithPartners { get; set; }

    /// <summary>Whether the public profile card (badges, certificates, streak) can be opened by anyone with its link.</summary>
    public bool ShareProfileCard { get; private set; }
    /// <summary>The unguessable part of the profile card link. Kept when sharing is turned off, so a re-shared link still works.</summary>
    public string? ProfileCardSlug { get; private set; }

    public int? CareerPathId { get; private set; }
    public CareerPath? CareerPath { get; private set; }
    public DateTime? CareerPathSelectedAt { get; private set; }

    /// <summary>Sets the career role the student is preparing for. Re-selecting the current path is a no-op.</summary>
    public void ChooseCareerPath(int careerPathId, DateTime selectedAt)
    {
        if (CareerPathId == careerPathId) return;
        CareerPathId = careerPathId;
        CareerPathSelectedAt = selectedAt;
    }

    /// <summary>Turns the public profile card on or off. The first time it is turned on it gets its link.</summary>
    public void SetProfileCardSharing(bool share, Func<string> newSlug)
    {
        if (share) ProfileCardSlug ??= newSlug();
        ShareProfileCard = share;
    }
}
