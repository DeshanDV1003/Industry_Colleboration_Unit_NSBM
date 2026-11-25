using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.Baseline.Dtos;

namespace NSBM.ICU.Application.Features.Baseline.Queries.GetBaseline;

/// <summary>Returns the baseline questions for the student's chosen path and their latest result, if any.</summary>
public sealed record GetBaselineQuery : IRequest<BaselineDto>;

public sealed class GetBaselineQueryHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IRequestHandler<GetBaselineQuery, BaselineDto>
{
    public async Task<BaselineDto> Handle(GetBaselineQuery request, CancellationToken cancellationToken)
    {
        var student = await db.GetCurrentStudentAsync(currentUser, cancellationToken);
        if (student.CareerPathId is not { } pathId)
            return new BaselineDto(null, null, [], null);

        var pathName = await db.CareerPaths.Where(p => p.Id == pathId).Select(p => p.Name).SingleAsync(cancellationToken);
        var questions = await db.LoadQuestionsAsync(pathId, cancellationToken);
        var latest = await db.LoadLatestAttemptAsync(student.Id, pathId, cancellationToken);

        var questionDtos = questions
            .Select(q => new BaselineQuestionDto(q.Id, q.Prompt, q.Skill!.Name,
                q.Options.Select(o => new BaselineOptionDto(o.Id, o.Text)).ToList()))
            .ToList();

        return new BaselineDto(pathId, pathName, questionDtos,
            latest is null ? null : BaselineResultBuilder.Build(latest, questions));
    }
}
