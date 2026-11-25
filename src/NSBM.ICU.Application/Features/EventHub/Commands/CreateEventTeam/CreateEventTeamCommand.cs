using FluentValidation;
using MediatR;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;

namespace NSBM.ICU.Application.Features.EventHub.Commands.CreateEventTeam;

/// <summary>Starts a team for a team event. The creator becomes captain and gets a join code to share.</summary>
public sealed record CreateEventTeamCommand(int EventId, string Name) : IRequest<string>;

public sealed class CreateEventTeamCommandValidator : AbstractValidator<CreateEventTeamCommand>
{
    public CreateEventTeamCommandValidator() => RuleFor(x => x.Name).NotEmpty().MinimumLength(2).MaximumLength(60);
}

public sealed class CreateEventTeamCommandHandler(IApplicationDbContext db, ICurrentUser currentUser, IDateTime clock)
    : IRequestHandler<CreateEventTeamCommand, string>
{
    public async Task<string> Handle(CreateEventTeamCommand request, CancellationToken ct)
    {
        var student = await db.GetCurrentStudentAsync(currentUser, ct);
        var industryEvent = await db.LoadApprovedEventAsync(request.EventId, ct);
        var team = industryEvent.CreateTeam(student.Id, request.Name.Trim(), clock.UtcNow);
        await db.SaveChangesAsync(ct);
        return team.JoinCode;
    }
}
