using FluentValidation;
using MediatR;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Domain.Rules;

namespace NSBM.ICU.Application.Features.EventHub.Commands.JoinEventTeam;

/// <summary>Joins a teammate's team with the join code they shared.</summary>
public sealed record JoinEventTeamCommand(int EventId, string JoinCode) : IRequest;

public sealed class JoinEventTeamCommandValidator : AbstractValidator<JoinEventTeamCommand>
{
    public JoinEventTeamCommandValidator() =>
        RuleFor(x => x.JoinCode).NotEmpty().Must(c => c.Trim().Length == ShareCodeRules.TeamJoinCodeLength)
            .WithMessage($"A join code has {ShareCodeRules.TeamJoinCodeLength} characters.");
}

public sealed class JoinEventTeamCommandHandler(IApplicationDbContext db, ICurrentUser currentUser, IDateTime clock) : IRequestHandler<JoinEventTeamCommand>
{
    public async Task Handle(JoinEventTeamCommand request, CancellationToken ct)
    {
        var student = await db.GetCurrentStudentAsync(currentUser, ct);
        var industryEvent = await db.LoadApprovedEventAsync(request.EventId, ct);
        industryEvent.JoinTeam(student.Id, request.JoinCode, DateOnly.FromDateTime(clock.UtcNow));
        await db.SaveChangesAsync(ct);
    }
}
