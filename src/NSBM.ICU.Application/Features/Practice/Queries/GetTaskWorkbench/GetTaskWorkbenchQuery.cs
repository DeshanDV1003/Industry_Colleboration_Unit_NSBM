using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.Practice.Dtos;

namespace NSBM.ICU.Application.Features.Practice.Queries.GetTaskWorkbench;

/// <summary>A task with its starter code and check descriptions (never the patterns), plus the student's attempts.</summary>
public sealed record GetTaskWorkbenchQuery(int TaskId) : IRequest<WorkbenchDto>;

public sealed class GetTaskWorkbenchQueryHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IRequestHandler<GetTaskWorkbenchQuery, WorkbenchDto>
{
    public async Task<WorkbenchDto> Handle(GetTaskWorkbenchQuery request, CancellationToken cancellationToken)
    {
        var student = await db.GetCurrentStudentAsync(currentUser, cancellationToken);

        var task = await db.ApprovedTasks().AsNoTracking()
            .Where(t => t.Id == request.TaskId)
            .Select(t => new WorkbenchTaskDto(
                t.Id,
                t.Title,
                t.Summary,
                t.Instructions,
                t.Company!.Name,
                t.CareerPath!.Name,
                t.Level,
                t.Situation,
                t.EstimatedMinutes,
                t.StarterLanguage,
                t.StarterCode,
                t.Skills.OrderBy(s => s.Skill!.Name).Select(s => s.Skill!.Name).ToList(),
                t.Checks.OrderBy(c => c.SortOrder).Select(c => new CheckSummaryDto(c.Id, c.Description)).ToList(),
                t.KnowledgeQuestions.Count))
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw NotFoundException.For("Task", request.TaskId);

        var checks = await db.LoadChecksAsync(request.TaskId, cancellationToken);
        var attempts = await db.TaskAttempts.AsNoTracking()
            .Include(a => a.CheckResults)
            .Include(a => a.Reviews)
            .Where(a => a.TaskId == request.TaskId && a.StudentId == student.Id)
            .OrderByDescending(a => a.AttemptNumber)
            .ToListAsync(cancellationToken);

        var knowledgeSubmittedAt = await db.KnowledgeCheckSubmissions
            .Where(k => k.TaskId == request.TaskId && k.StudentId == student.Id)
            .Select(k => (DateTime?)k.SubmittedAt)
            .FirstOrDefaultAsync(cancellationToken);

        return new WorkbenchDto(task, attempts.Select(a => a.ToDto(checks)).ToList(), knowledgeSubmittedAt);
    }
}
