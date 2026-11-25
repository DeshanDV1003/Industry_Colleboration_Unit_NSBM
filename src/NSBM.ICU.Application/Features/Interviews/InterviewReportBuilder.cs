using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.Interviews.Dtos;
using NSBM.ICU.Domain.Entities.People;

namespace NSBM.ICU.Application.Features.Interviews;

/// <summary>Builds a student's interview report: their latest answer to every question practised, soft skills first.</summary>
internal static class InterviewReportBuilder
{
    public static async Task<InterviewReportDto> BuildInterviewReportAsync(this IApplicationDbContext db, Student student, DateTime generatedAt, CancellationToken ct)
    {
        var answers = await db.InterviewAnswers.AsNoTracking()
            .Include(a => a.Question).ThenInclude(q => q!.CareerPath)
            .Where(a => a.StudentId == student.Id)
            .ToListAsync(ct);

        var items = answers
            .GroupBy(a => a.QuestionId)
            .Select(g => g.MaxBy(a => a.AnsweredAt)!)
            .OrderBy(a => a.Question!.Stage)
            .ThenBy(a => a.Question!.CareerPath?.Name)
            .ThenBy(a => a.Question!.ParentQuestionId ?? a.Question.Id)
            .ThenBy(a => a.Question!.ParentQuestionId.HasValue)
            .Select(a => new ReportItemDto(
                a.Question!.Stage,
                a.Question.CareerPath?.Name,
                a.Question.Topic,
                a.Question.Prompt,
                a.Question.ParentQuestionId.HasValue,
                InterviewFeedback.Build(a, a.Question)))
            .ToList();

        return new InterviewReportDto(student.FullName, student.StudentNo, generatedAt, items);
    }
}
