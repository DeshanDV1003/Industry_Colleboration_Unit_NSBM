using System.Linq.Expressions;
using NSBM.ICU.Application.Features.Mentoring.Dtos;
using NSBM.ICU.Domain.Entities.Mentoring;

namespace NSBM.ICU.Application.Features.Mentoring;

internal static class MentorQuestionProjection
{
    public static readonly Expression<Func<MentorQuestion, MentorQuestionDto>> ToDto = q => new MentorQuestionDto(
        q.Id,
        q.Title,
        q.Body,
        q.CareerPath != null ? q.CareerPath.Name : null,
        q.Student!.FirstName + " " + q.Student.LastName,
        q.AskedAt,
        q.Reply,
        q.RepliedBy,
        q.RepliedByCompany,
        q.RepliedAt);
}
