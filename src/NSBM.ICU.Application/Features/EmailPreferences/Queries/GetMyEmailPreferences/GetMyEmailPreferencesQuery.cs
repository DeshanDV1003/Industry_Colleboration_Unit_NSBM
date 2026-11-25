using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.EmailPreferences.Dtos;
using NSBM.ICU.Domain.Entities.Notifications;

namespace NSBM.ICU.Application.Features.EmailPreferences.Queries.GetMyEmailPreferences;

/// <summary>The signed-in person's e-mail choices; the defaults (everything on) until they change them.</summary>
public sealed record GetMyEmailPreferencesQuery : IRequest<EmailPreferencesDto>;

public sealed class GetMyEmailPreferencesQueryHandler(IApplicationDbContext db, ICurrentUser currentUser, IEmailSender email)
    : IRequestHandler<GetMyEmailPreferencesQuery, EmailPreferencesDto>
{
    public async Task<EmailPreferencesDto> Handle(GetMyEmailPreferencesQuery request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? throw new AuthenticationFailedException("You are not signed in.");
        var saved = await db.EmailPreferences.AsNoTracking().FirstOrDefaultAsync(p => p.UserId == userId, ct) ?? new EmailPreference();
        return new EmailPreferencesDto(saved.NotificationsByEmail, saved.WeeklyDigestByEmail, email.IsEnabled);
    }
}
