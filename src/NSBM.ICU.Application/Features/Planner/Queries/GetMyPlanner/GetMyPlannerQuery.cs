using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.Planner.Dtos;

namespace NSBM.ICU.Application.Features.Planner.Queries.GetMyPlanner;

/// <summary>Open items first (soonest due date first, undated last), then completed items.</summary>
public sealed record GetMyPlannerQuery : IRequest<IReadOnlyList<PlannerItemDto>>;

public sealed class GetMyPlannerQueryHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IRequestHandler<GetMyPlannerQuery, IReadOnlyList<PlannerItemDto>>
{
    public async Task<IReadOnlyList<PlannerItemDto>> Handle(GetMyPlannerQuery request, CancellationToken cancellationToken)
    {
        var student = await db.GetCurrentStudentAsync(currentUser, cancellationToken);
        return await db.PlannerItems.AsNoTracking()
            .Where(p => p.StudentId == student.Id)
            .OrderBy(p => p.IsDone).ThenBy(p => p.DueDate == null).ThenBy(p => p.DueDate).ThenBy(p => p.CreatedAt)
            .Select(p => new PlannerItemDto(p.Id, p.Kind, p.Title, p.Notes, p.DueDate, p.IsDone, p.CompletedAt))
            .ToListAsync(cancellationToken);
    }
}
