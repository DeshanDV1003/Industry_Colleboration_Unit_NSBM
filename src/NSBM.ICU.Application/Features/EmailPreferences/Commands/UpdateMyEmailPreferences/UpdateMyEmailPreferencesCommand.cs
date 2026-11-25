using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Domain.Entities.Notifications;

namespace NSBM.ICU.Application.Features.EmailPreferences.Commands.UpdateMyEmailPreferences;

/// <summary>Turns e-mail updates and the weekly summary on or off for the signed-in person. Security alerts always send.</summary>
public sealed record UpdateMyEmailPreferencesCommand(bool NotificationsByEmail, bool WeeklyDigestByEmail) : IRequest;

public sealed class UpdateMyEmailPreferencesCommandHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IRequestHandler<UpdateMyEmailPreferencesCommand>
{
    public async Task Handle(UpdateMyEmailPreferencesCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? throw new AuthenticationFailedException("You are not signed in.");
        var preference = await db.EmailPreferences.FirstOrDefaultAsync(p => p.UserId == userId, ct);
        if (preference is null)
        {
            preference = new EmailPreference { UserId = userId };
            db.EmailPreferences.Add(preference);
        }

        preference.NotificationsByEmail = request.NotificationsByEmail;
        preference.WeeklyDigestByEmail = request.WeeklyDigestByEmail;
        await db.SaveChangesAsync(ct);
    }
}
