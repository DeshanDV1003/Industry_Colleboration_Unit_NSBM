using NSBM.ICU.Domain.Common;
using NSBM.ICU.Domain.Entities.People;
using NSBM.ICU.Domain.Entities.Profiles;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.UnitTests.Achievements;

public sealed class SkillBadgeTests
{
    private static readonly DateTime Now = new(2026, 11, 22, 9, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Approved_badge_gets_a_code_and_cannot_be_requested_again()
    {
        var badge = SkillBadge.Request(1, 2, "Correct baseline answer", "  ", Now);
        Assert.Equal(ModerationStatus.Pending, badge.Status);
        Assert.Null(badge.StudentNote);

        badge.Approve("Dr Lecturer", "BDG-ABCD2345", null, Now);
        Assert.Equal(ModerationStatus.Approved, badge.Status);
        Assert.Equal("BDG-ABCD2345", badge.VerificationCode);
        Assert.Throws<DomainRuleException>(() => badge.RequestAgain("x", null, Now));
        Assert.Throws<DomainRuleException>(() => badge.Decline("Dr Lecturer", "no", Now));
    }

    [Fact]
    public void Declining_needs_advice_and_allows_asking_again()
    {
        var badge = SkillBadge.Request(1, 2, "evidence", null, Now);
        Assert.Throws<DomainRuleException>(() => badge.Decline("Dr Lecturer", " ", Now));

        badge.Decline("Dr Lecturer", "Pass one more task first.", Now);
        Assert.Equal(ModerationStatus.Rejected, badge.Status);
        Assert.Throws<DomainRuleException>(() => badge.Approve("x", "BDG-X", null, Now));

        badge.RequestAgain("more evidence", "Done now", Now.AddDays(3));
        Assert.Equal(ModerationStatus.Pending, badge.Status);
        Assert.Null(badge.ReviewNote);
        Assert.Equal("more evidence", badge.EvidenceSummary);
    }

    [Fact]
    public void Pending_badge_cannot_be_requested_twice()
    {
        var badge = SkillBadge.Request(1, 2, "evidence", null, Now);
        Assert.Throws<DomainRuleException>(() => badge.RequestAgain("evidence", null, Now));
    }

    [Fact]
    public void Profile_card_keeps_its_link_when_turned_off_and_on()
    {
        var student = new Student();
        var made = 0;
        string NewSlug() => $"slug{++made}";

        student.SetProfileCardSharing(true, NewSlug);
        Assert.True(student.ShareProfileCard);
        Assert.Equal("slug1", student.ProfileCardSlug);

        student.SetProfileCardSharing(false, NewSlug);
        Assert.False(student.ShareProfileCard);
        student.SetProfileCardSharing(true, NewSlug);
        Assert.Equal("slug1", student.ProfileCardSlug);
        Assert.Equal(1, made);
    }
}
