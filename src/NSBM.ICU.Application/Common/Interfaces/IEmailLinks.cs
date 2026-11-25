namespace NSBM.ICU.Application.Common.Interfaces;

/// <summary>Where links in e-mails point: the public address of the web app (Email__AppBaseUrl).</summary>
public interface IEmailLinks
{
    string AppBaseUrl { get; }
}
