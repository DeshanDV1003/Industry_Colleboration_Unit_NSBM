using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.Interviews.Dtos;
using NSBM.ICU.Domain.Entities.Interviews;
using NSBM.ICU.Domain.Entities.Learning;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.Interviews.Queries.GetInterviewSession;

/// <summary>
/// The questions of one stage with the student's latest answer to each. Technical questions use the given
/// career path, or the student's chosen path when none is given.
/// </summary>
public sealed record GetInterviewSessionQuery(InterviewStage Stage, int? CareerPathId) : IRequest<InterviewSessionDto>;

public sealed class GetInterviewSessionQueryHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IRequestHandler<GetInterviewSessionQuery, InterviewSessionDto>
{
    public async Task<InterviewSessionDto> Handle(GetInterviewSessionQuery request, CancellationToken cancellationToken)
    {
        var student = await db.GetCurrentStudentAsync(currentUser, cancellationToken);
        var pathId = request.Stage == InterviewStage.Technical ? request.CareerPathId ?? student.CareerPathId : null;

        if (request.Stage == InterviewStage.Technical && pathId is null)
            return new InterviewSessionDto(request.Stage, null, null, []);

        var pathName = pathId is null ? null
            : await db.CareerPaths.Where(p => p.Id == pathId).Select(p => p.Name).FirstOrDefaultAsync(cancellationToken);

        var questions = await db.InterviewQuestions.AsNoTracking()
            .Include(q => q.FollowUps)
            .Where(q => q.Stage == request.Stage && q.CareerPathId == pathId && q.ParentQuestionId == null)
            .OrderBy(q => q.SortOrder)
            .ToListAsync(cancellationToken);

        var questionIds = questions.SelectMany(q => q.FollowUps.Select(f => f.Id).Append(q.Id)).ToList();
        var latest = (await db.InterviewAnswers.AsNoTracking()
                .Where(a => a.StudentId == student.Id && questionIds.Contains(a.QuestionId))
                .ToListAsync(cancellationToken))
            .GroupBy(a => a.QuestionId)
            .ToDictionary(g => g.Key, g => g.MaxBy(a => a.AnsweredAt)!);

        return new InterviewSessionDto(request.Stage, pathId, pathName, questions.Select(q => ToDto(q, latest)).ToList());
    }

    private static InterviewQuestionDto ToDto(InterviewQuestion question, IReadOnlyDictionary<int, InterviewAnswer> latest) => new(
        question.Id,
        question.Topic,
        question.Prompt,
        question.Tip,
        latest.TryGetValue(question.Id, out var answer) ? InterviewFeedback.Build(answer, question) : null,
        question.FollowUps.OrderBy(f => f.SortOrder).Select(f => ToDto(f, latest)).ToList());
}
