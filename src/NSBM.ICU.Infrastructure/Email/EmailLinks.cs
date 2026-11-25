using Microsoft.Extensions.Options;
using NSBM.ICU.Application.Common.Interfaces;

namespace NSBM.ICU.Infrastructure.Email;

/// <summary>Reads the web app's public address from the e-mail settings.</summary>
public sealed class EmailLinks(IOptions<EmailOptions> options) : IEmailLinks
{
    public string AppBaseUrl => options.Value.AppBaseUrl;
}
