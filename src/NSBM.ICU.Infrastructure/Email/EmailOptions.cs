namespace NSBM.ICU.Infrastructure.Email;

/// <summary>How e-mail is delivered. Off unless configured; the SMTP password comes from the environment, never a committed file.</summary>
public sealed class EmailOptions
{
    public const string SectionName = "Email";

    /// <summary>Off (default), File (writes .eml files to <see cref="PickupDirectory"/>, for development) or Smtp.</summary>
    public EmailMode Mode { get; init; } = EmailMode.Off;
    public string FromAddress { get; init; } = "no-reply@nsbm.ac.lk";
    public string FromName { get; init; } = "NSBM Industry Collaboration Unit";
    /// <summary>The web app's public address; every link in an e-mail starts with it.</summary>
    public string AppBaseUrl { get; init; } = "http://localhost:5301";
    /// <summary>For File mode; relative paths are under the API's content root.</summary>
    public string PickupDirectory { get; init; } = "App_Data/mail";
    public SmtpOptions Smtp { get; init; } = new();

    public int DispatchIntervalSeconds { get; init; } = 30;
    /// <summary>Older unsent notifications are not e-mailed (e.g. after e-mail was off for a while).</summary>
    public int MaxAgeHours { get; init; } = 48;
    public DayOfWeek DigestDay { get; init; } = DayOfWeek.Monday;
    public int DigestHour { get; init; } = 7;
    public string TimeZone { get; init; } = "Asia/Colombo";
}

public enum EmailMode
{
    Off = 0,
    File = 1,
    Smtp = 2,
}

public sealed class SmtpOptions
{
    public string Host { get; init; } = string.Empty;
    public int Port { get; init; } = 587;
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    /// <summary>StartTls (port 587, default), SslOnConnect (port 465), Auto or None.</summary>
    public string Security { get; init; } = "StartTls";
}
