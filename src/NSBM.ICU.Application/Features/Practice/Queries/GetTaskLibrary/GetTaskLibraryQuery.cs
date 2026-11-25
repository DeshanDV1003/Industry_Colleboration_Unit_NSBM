using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.Practice.Dtos;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.Practice.Queries.GetTaskLibrary;

/// <summary>Approved industry tasks, filterable by role, level, work situation and text, with the student's own progress.</summary>
public sealed record GetTaskLibraryQuery(int? CareerPathId, LearningStage? Level, WorkSituation? Situation, string? Search)
    : IRequest<IReadOnlyList<TaskLibraryItemDto>>;

public sealed class GetTaskLibraryQueryHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IRequestHandler<GetTaskLibraryQuery, IReadOnlyList<TaskLibraryItemDto>>
{
    public async Task<IReadOnlyList<TaskLibraryItemDto>> Handle(GetTaskLibraryQuery request, CancellationToken cancellationToken)
    {
        var student = await db.GetCurrentStudentAsync(currentUser, cancellationToken);

        var tasks = db.ApprovedTasks().AsNoTracking();
        if (request.CareerPathId is { } pathId) tasks = tasks.Where(t => t.CareerPathId == pathId);
        if (request.Level is { } level) tasks = tasks.Where(t => t.Level == level);
        if (request.Situation is { } situation) tasks = tasks.Where(t => t.Situation == situation);
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim();
            tasks = tasks.Where(t => t.Title.Contains(term) || t.Summary.Contains(term) || t.Company!.Name.Contains(term));
        }

        return await tasks
            // Lecturer-assigned tasks first, then by role, level and title.
            .OrderByDescending(t => db.TaskAssignments.Any(a => a.TaskId == t.Id && a.StudentId == student.Id))
            .ThenBy(t => t.CareerPath!.SortOrder).ThenBy(t => t.Level).ThenBy(t => t.Title)
            .Select(t => new TaskLibraryItemDto(
                t.Id,
                t.Title,
                t.Summary,
                t.Company!.Name,
                t.CareerPathId,
                t.CareerPath!.Name,
                t.Level,
                t.Situation,
                t.EstimatedMinutes,
                t.Skills.OrderBy(s => s.Skill!.Name).Select(s => s.Skill!.Name).ToList(),
                t.Checks.Count,
                db.TaskAttempts.Count(a => a.TaskId == t.Id && a.StudentId == student.Id),
                db.TaskAttempts.Where(a => a.TaskId == t.Id && a.StudentId == student.Id).Max(a => (int?)a.PassedChecks),
                db.KnowledgeCheckSubmissions.Any(k => k.TaskId == t.Id && k.StudentId == student.Id),
                db.TaskAssignments.Where(a => a.TaskId == t.Id && a.StudentId == student.Id).Select(a => a.LecturerName).FirstOrDefault()))
            .ToListAsync(cancellationToken);
    }
}
