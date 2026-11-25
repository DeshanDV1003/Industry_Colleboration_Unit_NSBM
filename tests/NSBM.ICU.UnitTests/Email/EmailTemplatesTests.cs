using System.Text.RegularExpressions;
using NSBM.ICU.Application.Common.Email;
using NSBM.ICU.Domain.Enums;
using NSBM.ICU.Domain.Rules;

namespace NSBM.ICU.UnitTests.Email;

public sealed class EmailTemplatesTests
{
    private const string BaseUrl = "https://icu.nsbm.ac.lk/";

    [Fact]
    public void One_update_uses_its_title_as_the_subject()
    {
        var mail = EmailTemplates.Updates("a@x.lk", "Sahan", [new EmailItem("Hall assigned: Software Architecture", "LH-101, Mon 9:00", "/calendar")], BaseUrl);
        Assert.Equal("Hall assigned: Software Architecture", mail.Subject);
        Assert.Contains("https://icu.nsbm.ac.lk/calendar", mail.TextBody);
    }

    [Fact]
    public void Several_updates_are_batched_into_one_email()
    {
        var mail = EmailTemplates.Updates("a@x.lk", "Sahan", [new("One", "a", null), new("Two", "b", null), new("Three", "c", null)], BaseUrl);
        Assert.Equal("3 new updates from the Industry Collaboration Unit", mail.Subject);
        Assert.Contains("Three", mail.HtmlBody);
    }

    [Fact]
    public void Text_from_the_platform_is_html_encoded()
    {
        var mail = EmailTemplates.Updates("a@x.lk", "<Sahan>", [new EmailItem("<script>alert(1)</script>", "a & b", null)], BaseUrl);
        Assert.DoesNotContain("<script>", mail.HtmlBody);
        Assert.Contains("&lt;script&gt;", mail.HtmlBody);
        Assert.Contains("&lt;Sahan&gt;", mail.HtmlBody);
    }

    [Theory]
    [InlineData("https://evil.example/phish")]
    [InlineData("//evil.example")]
    [InlineData("javascript:alert(1)")]
    public void Links_never_leave_the_platform(string link)
    {
        var mail = EmailTemplates.Updates("a@x.lk", "Sahan", [new EmailItem("T", "B", link)], BaseUrl);
        var hrefs = Regex.Matches(mail.HtmlBody, "href=\"([^\"]*)\"").Select(m => m.Groups[1].Value).ToList();
        Assert.NotEmpty(hrefs);
        Assert.All(hrefs, href => Assert.StartsWith("https://icu.nsbm.ac.lk/", href));
    }

    [Fact]
    public void Reset_email_carries_the_link_and_says_how_long_it_works()
    {
        const string url = "https://icu.nsbm.ac.lk/reset-password?email=a%40x.lk&token=abc_123";
        var mail = EmailTemplates.PasswordReset("a@x.lk", "Amaya", url, 60);
        Assert.Equal("Reset your password", mail.Subject);
        Assert.Contains(url, mail.TextBody);
        Assert.Contains("60 minutes", mail.TextBody);
        Assert.Contains("ignore this e-mail", mail.TextBody);
        Assert.Contains("href=\"https://icu.nsbm.ac.lk/reset-password?email=a%40x.lk&amp;token=abc_123\"", mail.HtmlBody);
    }

    [Fact]
    public void Digest_says_when_there_is_nothing_new() =>
        Assert.Contains("all caught up", EmailTemplates.WeeklyDigest("a@x.lk", "Sahan", [], BaseUrl).TextBody);

    [Theory]
    [InlineData(NotificationKind.HallAssigned, true, true)]
    [InlineData(NotificationKind.HallAssigned, false, false)]
    [InlineData(NotificationKind.AccountSecurity, false, true)]
    [InlineData(NotificationKind.CalendarEvent, true, false)]
    [InlineData(NotificationKind.IndustryEvent, true, false)]
    [InlineData(NotificationKind.Welcome, true, true)]
    public void Policy_decides_what_is_emailed(NotificationKind kind, bool wantsEmails, bool emailed) =>
        Assert.Equal(emailed, EmailPolicy.ShouldEmail(kind, wantsEmails));
}
