using MimeKit;
using NSBM.ICU.Application.Common.Email;

namespace NSBM.ICU.Infrastructure.Email;

/// <summary>Builds the MIME message shared by every sender: plain text with an HTML alternative.</summary>
internal static class EmailMime
{
    public static MimeMessage Build(EmailMessage message, EmailOptions options)
    {
        var mime = new MimeMessage();
        mime.From.Add(new MailboxAddress(options.FromName, options.FromAddress));
        mime.To.Add(new MailboxAddress(message.ToName, message.To));
        mime.Subject = message.Subject;
        mime.Body = new BodyBuilder { TextBody = message.TextBody, HtmlBody = message.HtmlBody }.ToMessageBody();
        return mime;
    }
}
