using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.Mentoring.Dtos;

namespace NSBM.ICU.Application.Features.Mentoring.Queries.GetMyMentorQuestions;

public sealed record GetMyMentorQuestionsQuery : IRequest<IReadOnlyList<MentorQuestionDto>>;

public sealed class GetMyMentorQuestionsQueryHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IRequestHandler<GetMyMentorQuestionsQuery, IReadOnlyList<MentorQuestionDto>>
{
    public async Task<IReadOnlyList<MentorQuestionDto>> Handle(GetMyMentorQuestionsQuery request, CancellationToken cancellationToken)
    {
        var student = await db.GetCurrentStudentAsync(currentUser, cancellationToken);
        return await db.MentorQuestions.AsNoTracking()
            .Where(q => q.StudentId == student.Id)
            .OrderByDescending(q => q.AskedAt)
            .Select(MentorQuestionProjection.ToDto)
            .ToListAsync(cancellationToken);
    }
}
