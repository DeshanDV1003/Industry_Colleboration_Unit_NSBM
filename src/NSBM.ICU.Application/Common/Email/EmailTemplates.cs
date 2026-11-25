using System.Net;
using System.Text;

namespace NSBM.ICU.Application.Common.Email;

/// <summary>One item in an update or digest e-mail.</summary>
public sealed record EmailItem(string Title, string Body, string? Link);

/// <summary>
/// Builds every e-mail the platform sends. Pure, so wording and links are unit tested. Text is HTML-encoded;
/// links always point at <c>appBaseUrl</c>, never at anything taken from user input.
/// </summary>
public static class EmailTemplates
{
    private const string Unit = "NSBM Industry Collaboration Unit";
    private const string Green = "#0B7A43";

    public static EmailMessage Updates(string to, string name, IReadOnlyList<EmailItem> items, string appBaseUrl)
    {
        var subject = items.Count == 1 ? items[0].Title : $"{items.Count} new updates from the Industry Collaboration Unit";
        var intro = items.Count == 1 ? "You have a new update:" : $"You have {items.Count} new updates:";
        return Build(to, name, subject, intro, items, appBaseUrl,
            "You receive these because e-mail updates are on. Change this under My settings.");
    }

    public static EmailMessage WeeklyDigest(string to, string name, IReadOnlyList<EmailItem> unread, string appBaseUrl)
    {
        var intro = unread.Count == 0
            ? "You are all caught up: no unread updates. Open the platform to see your week ahead."
            : $"You have {unread.Count} unread update{(unread.Count == 1 ? "" : "s")}. The most recent:";
        return Build(to, name, "Your week at the Industry Collaboration Unit", intro, unread.Take(10).ToList(), appBaseUrl,
            "You receive this weekly summary because it is on. Change this under My settings.");
    }

    /// <summary>
    /// The reset link. Unlike other e-mails, the link carries a secret, so it is built here from the trusted base URL
    /// and never stored as a notification.
    /// </summary>
    public static EmailMessage PasswordReset(string to, string name, string resetUrl, int validMinutes)
    {
        string E(string value) => WebUtility.HtmlEncode(value);
        var text = $"Hello {name},\n\nSomeone asked to reset the password for your {Unit} account.\n\n" +
                   $"Choose a new password here (the link works once, for {validMinutes} minutes):\n{resetUrl}\n\n" +
                   "If you did not ask for this, ignore this e-mail: your password stays the same.\n\n" + Unit;
        var html = "<div style=\"font-family:Segoe UI,Arial,sans-serif;max-width:560px;color:#1f2937\">" +
                   $"<div style=\"background:{Green};color:#fff;padding:14px 18px;font-weight:600\">{E(Unit)}</div><div style=\"padding:18px\">" +
                   $"<p>Hello {E(name)},</p><p>Someone asked to reset the password for your account.</p>" +
                   $"<p><a href=\"{E(resetUrl)}\" style=\"display:inline-block;background:{Green};color:#fff;padding:8px 14px;border-radius:6px;text-decoration:none\">Choose a new password</a></p>" +
                   $"<p style=\"font-size:13px;color:#4b5563\">The link works once, for {validMinutes} minutes. If you did not ask for this, ignore this e-mail: your password stays the same.</p>" +
                   "</div></div>";
        return new EmailMessage(to, name, "Reset your password", text, html);
    }

    public static EmailMessage Test(string to, string name, string appBaseUrl) =>
        Build(to, name, "Test e-mail from the Industry Collaboration Unit",
            "E-mail delivery is working. This message was sent from the platform's settings page.", [], appBaseUrl, null);

    private static EmailMessage Build(string to, string name, string subject, string intro, IReadOnlyList<EmailItem> items,
        string appBaseUrl, string? footer)
    {
        var baseUrl = appBaseUrl.TrimEnd('/');
        string Url(string? link) => link is { Length: > 0 } && link.StartsWith('/') ? baseUrl + link : baseUrl + "/notifications";

        var text = new StringBuilder()
            .AppendLine($"Hello {name},").AppendLine().AppendLine(intro).AppendLine();
        foreach (var item in items)
            text.AppendLine($"• {item.Title}").AppendLine($"  {item.Body}").AppendLine($"  {Url(item.Link)}").AppendLine();
        text.AppendLine($"Open the platform: {baseUrl}/notifications").AppendLine().AppendLine(Unit);
        if (footer is not null) text.AppendLine().AppendLine(footer);

        string E(string value) => WebUtility.HtmlEncode(value);
        var html = new StringBuilder()
            .Append("<div style=\"font-family:Segoe UI,Arial,sans-serif;max-width:560px;color:#1f2937\">")
            .Append($"<div style=\"background:{Green};color:#fff;padding:14px 18px;font-weight:600\">{E(Unit)}</div>")
            .Append("<div style=\"padding:18px\">")
            .Append($"<p>Hello {E(name)},</p><p>{E(intro)}</p>");
        foreach (var item in items)
            html.Append("<div style=\"border:1px solid #e5e7eb;border-radius:8px;padding:10px 12px;margin:8px 0\">")
                .Append($"<a href=\"{E(Url(item.Link))}\" style=\"color:{Green};font-weight:600;text-decoration:none\">{E(item.Title)}</a>")
                .Append($"<div style=\"font-size:14px;color:#4b5563;margin-top:4px\">{E(item.Body)}</div></div>");
        html.Append($"<p><a href=\"{E(baseUrl + "/notifications")}\" style=\"display:inline-block;background:{Green};color:#fff;padding:8px 14px;border-radius:6px;text-decoration:none\">Open the platform</a></p>");
        if (footer is not null) html.Append($"<p style=\"font-size:12px;color:#6b7280\">{E(footer)}</p>");
        html.Append("</div></div>");

        return new EmailMessage(to, name, subject, text.ToString(), html.ToString());
    }
}
