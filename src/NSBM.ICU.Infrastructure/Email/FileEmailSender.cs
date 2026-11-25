using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NSBM.ICU.Application.Common.Email;
using NSBM.ICU.Application.Common.Interfaces;

namespace NSBM.ICU.Infrastructure.Email;

/// <summary>Development: writes each e-mail as an .eml file (open it in Outlook or any mail app). Nothing leaves the machine.</summary>
public sealed class FileEmailSender(IOptions<EmailOptions> options, IHostEnvironment environment) : IEmailSender
{
    public bool IsEnabled => true;

    public async Task SendAsync(EmailMessage message, CancellationToken cancellationToken)
    {
        var folder = Path.IsPathRooted(options.Value.PickupDirectory)
            ? options.Value.PickupDirectory
            : Path.Combine(environment.ContentRootPath, options.Value.PickupDirectory);
        Directory.CreateDirectory(folder);

        var path = Path.Combine(folder, $"{DateTime.UtcNow:yyyyMMdd-HHmmss-fff}-{Guid.NewGuid():N}.eml");
        await EmailMime.Build(message, options.Value).WriteToAsync(path, cancellationToken);
    }
}
