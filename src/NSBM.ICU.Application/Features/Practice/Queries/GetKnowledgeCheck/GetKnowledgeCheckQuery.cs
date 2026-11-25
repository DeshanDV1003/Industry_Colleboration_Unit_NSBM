using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.Practice.Dtos;

namespace NSBM.ICU.Application.Features.Practice.Queries.GetKnowledgeCheck;

/// <summary>The task's follow-up questions with any answers the student has already saved.</summary>
public sealed record GetKnowledgeCheckQuery(int TaskId) : IRequest<KnowledgeCheckDto>;

public sealed class GetKnowledgeCheckQueryHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IRequestHandler<GetKnowledgeCheckQuery, KnowledgeCheckDto>
{
    public async Task<KnowledgeCheckDto> Handle(GetKnowledgeCheckQuery request, CancellationToken cancellationToken)
    {
        var student = await db.GetCurrentStudentAsync(currentUser, cancellationToken);

        var task = await db.ApprovedTasks().AsNoTracking()
            .Where(t => t.Id == request.TaskId)
            .Select(t => new
            {
                t.Title,
                Questions = t.KnowledgeQuestions.OrderBy(q => q.SortOrder).Select(q => new { q.Id, q.Prompt }).ToList(),
            })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw NotFoundException.For("Task", request.TaskId);

        var submission = await db.KnowledgeCheckSubmissions.AsNoTracking()
            .Include(k => k.Answers)
            .FirstOrDefaultAsync(k => k.TaskId == request.TaskId && k.StudentId == student.Id, cancellationToken);

        var hasAttempt = await db.TaskAttempts.AnyAsync(a => a.TaskId == request.TaskId && a.StudentId == student.Id, cancellationToken);
        var answers = submission?.Answers.ToDictionary(a => a.QuestionId, a => a.Text) ?? [];

        return new KnowledgeCheckDto(
            request.TaskId,
            task.Title,
            hasAttempt,
            submission?.SubmittedAt,
            task.Questions.Select(q => new KnowledgeCheckQuestionDto(q.Id, q.Prompt, answers.GetValueOrDefault(q.Id))).ToList());
    }
}
