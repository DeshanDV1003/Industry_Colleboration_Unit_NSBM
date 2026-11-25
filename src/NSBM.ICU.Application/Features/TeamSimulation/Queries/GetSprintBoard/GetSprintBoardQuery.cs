using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.TeamSimulation.Dtos;

namespace NSBM.ICU.Application.Features.TeamSimulation.Queries.GetSprintBoard;

public sealed record GetSprintBoardQuery : IRequest<SprintBoardDto>;

public sealed class GetSprintBoardQueryHandler(IApplicationDbContext db, ICurrentUser currentUser) : IRequestHandler<GetSprintBoardQuery, SprintBoardDto>
{
    private const string You = "You";

    public async Task<SprintBoardDto> Handle(GetSprintBoardQuery request, CancellationToken cancellationToken)
    {
        var student = await db.GetCurrentStudentAsync(currentUser, cancellationToken);
        var myProgress = await db.SprintItemProgress.AsNoTracking()
            .Where(p => p.StudentId == student.Id)
            .ToDictionaryAsync(p => p.WorkItemId, p => p.Status, cancellationToken);

        var items = await db.SprintWorkItems.AsNoTracking()
            .OrderBy(i => i.SortOrder)
            .Select(i => new { i.Id, i.Key, i.Title, i.Description, i.StoryPoints, i.CareerPathId, Role = i.CareerPath!.Name, i.TeammateName, i.TeammateStatus })
            .ToListAsync(cancellationToken);

        var myRole = items.FirstOrDefault(i => i.CareerPathId == student.CareerPathId)?.Role;

        return new SprintBoardDto(myRole, items.Select(i =>
        {
            var mine = myProgress.TryGetValue(i.Id, out var status);
            return new SprintItemDto(i.Id, i.Key, i.Title, i.Description, i.StoryPoints, i.Role,
                Owner: mine ? You : i.TeammateName,
                Status: mine ? status : i.TeammateStatus,
                IsMine: mine,
                CanPickUp: !mine && i.CareerPathId == student.CareerPathId);
        }).ToList());
    }
}
