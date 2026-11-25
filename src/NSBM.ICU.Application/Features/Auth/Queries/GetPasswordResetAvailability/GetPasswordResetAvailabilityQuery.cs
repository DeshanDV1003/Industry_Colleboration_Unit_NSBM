using MediatR;
using NSBM.ICU.Application.Common.Interfaces;

namespace NSBM.ICU.Application.Features.Auth.Queries.GetPasswordResetAvailability;

public sealed record PasswordResetAvailabilityDto(bool Available, int LinkValidMinutes);

/// <summary>Whether this server can e-mail reset links, so the sign-in page can offer it or point to the Unit instead.</summary>
public sealed record GetPasswordResetAvailabilityQuery : IRequest<PasswordResetAvailabilityDto>;

public sealed class GetPasswordResetAvailabilityQueryHandler(IPasswordResetMailer mailer)
    : IRequestHandler<GetPasswordResetAvailabilityQuery, PasswordResetAvailabilityDto>
{
    public Task<PasswordResetAvailabilityDto> Handle(GetPasswordResetAvailabilityQuery request, CancellationToken ct) =>
        Task.FromResult(new PasswordResetAvailabilityDto(mailer.IsAvailable, mailer.LinkValidMinutes));
}
