namespace NSBM.ICU.Application.Common.Email;

/// <summary>A ready-to-send e-mail with a plain-text body and an HTML body that says the same thing.</summary>
public sealed record EmailMessage(string To, string ToName, string Subject, string TextBody, string HtmlBody);
