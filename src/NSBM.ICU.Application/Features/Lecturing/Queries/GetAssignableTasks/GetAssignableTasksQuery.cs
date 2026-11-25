using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.Lecturing.Queries.GetAssignableTasks;

public sealed record AssignableTaskDto(int Id, string Title, string CareerPathName, LearningStage Level, string CompanyName);

/// <summary>Approved industry tasks a lecturer can assign, grouped by role.</summary>
public sealed record GetAssignableTasksQuery : IRequest<IReadOnlyList<AssignableTaskDto>>;

public sealed class GetAssignableTasksQueryHandler(IApplicationDbContext db) : IRequestHandler<GetAssignableTasksQuery, IReadOnlyList<AssignableTaskDto>>
{
    public async Task<IReadOnlyList<AssignableTaskDto>> Handle(GetAssignableTasksQuery request, CancellationToken cancellationToken) =>
        await db.IndustryTasks.AsNoTracking()
            .Where(t => t.Status == ModerationStatus.Approved)
            .OrderBy(t => t.CareerPath!.SortOrder).ThenBy(t => t.Level).ThenBy(t => t.Title)
            .Select(t => new AssignableTaskDto(t.Id, t.Title, t.CareerPath!.Name, t.Level, t.Company!.Name))
            .ToListAsync(cancellationToken);
}
