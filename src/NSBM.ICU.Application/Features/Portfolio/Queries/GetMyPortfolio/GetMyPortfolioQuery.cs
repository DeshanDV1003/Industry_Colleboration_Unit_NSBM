using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Evidence;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.Portfolio.Dtos;
using NSBM.ICU.Application.Features.Practice.Dtos;
using NSBM.ICU.Domain.Enums;
using NSBM.ICU.Domain.Rules;

namespace NSBM.ICU.Application.Features.Portfolio.Queries.GetMyPortfolio;

public sealed record GetMyPortfolioQuery : IRequest<PortfolioDto>;

public sealed class GetMyPortfolioQueryHandler(IApplicationDbContext db, ICurrentUser currentUser) : IRequestHandler<GetMyPortfolioQuery, PortfolioDto>
{
    public async Task<PortfolioDto> Handle(GetMyPortfolioQuery request, CancellationToken ct)
    {
        var student = await db.GetCurrentStudentAsync(currentUser, ct);
        var pathId = student.CareerPathId;

        var pathName = pathId is null ? null : await db.CareerPaths.Where(p => p.Id == pathId).Select(p => p.Name).FirstOrDefaultAsync(ct);
        var headline = await db.CvProfiles.Where(c => c.StudentId == student.Id).Select(c => c.Headline).FirstOrDefaultAsync(ct);

        var roadmapTotal = pathId is null ? 0 : await db.RoadmapSteps.CountAsync(s => s.CareerPathId == pathId, ct);
        var roadmapDone = pathId is null ? 0 : await db.RoadmapStepCompletions.CountAsync(c => c.StudentId == student.Id && c.RoadmapStep!.CareerPathId == pathId, ct);

        var baseline = pathId is null ? null : await db.BaselineAttempts
            .Where(a => a.StudentId == student.Id && a.CareerPathId == pathId)
            .OrderByDescending(a => a.CompletedAt)
            .Select(a => new { a.CorrectCount, a.QuestionCount })
            .FirstOrDefaultAsync(ct);

        var evidence = await db.LoadStudentEvidenceAsync(student.Id, ct);
        var skills = await db.Skills.AsNoTracking().OrderBy(s => s.Name).Select(s => new { s.Id, s.Name }).ToListAsync(ct);
        var practised = skills.Where(s => evidence.For(s.Id) == SkillEvidence.PracticeSignal).Select(s => s.Name).ToList();
        var listedOnly = skills.Where(s => evidence.For(s.Id) == SkillEvidence.ListedOnly).Select(s => s.Name).ToList();

        var tasks = await LoadTasksAsync(student.Id, ct);

        var answers = (await db.InterviewAnswers.AsNoTracking()
                .Where(a => a.StudentId == student.Id)
                .Select(a => new { a.QuestionId, a.AnsweredAt, a.MatchedKeywords, Stage = a.Question!.Stage, a.Question.Keywords })
                .ToListAsync(ct))
            .GroupBy(a => a.QuestionId).Select(g => g.MaxBy(a => a.AnsweredAt)!).ToList();
        var coverage = answers.Count == 0 ? 0 : (int)Math.Round(answers.Average(a =>
            SkillSignalRules.Percent(Count(a.MatchedKeywords), Count(a.Keywords))));
        var interview = new PortfolioInterviewDto(
            answers.Count(a => a.Stage == InterviewStage.SoftSkills), answers.Count(a => a.Stage == InterviewStage.Technical), coverage);

        var sprintDone = await db.SprintItemProgress.CountAsync(p => p.StudentId == student.Id && p.Status == SprintItemStatus.Done, ct);
        var applications = await db.InternshipApplications.CountAsync(a => a.StudentId == student.Id, ct);
        var notes = await db.SupportNotes.AsNoTracking().Where(n => n.StudentId == student.Id).OrderByDescending(n => n.CreatedAt)
            .Select(n => new LecturerNoteDto(n.LecturerName, n.Text, n.CreatedAt)).ToListAsync(ct);

        return new PortfolioDto(student.FullName, student.StudentNo, student.Programme, pathName, string.IsNullOrWhiteSpace(headline) ? null : headline,
            roadmapDone, roadmapTotal, baseline is null ? null : SkillSignalRules.Percent(baseline.CorrectCount, baseline.QuestionCount),
            practised, listedOnly, tasks, interview, sprintDone, applications, notes);
    }

    private static int Count(string semicolonList) => semicolonList.Split(';', StringSplitOptions.RemoveEmptyEntries).Length;

    private async Task<IReadOnlyList<PortfolioTaskDto>> LoadTasksAsync(int studentId, CancellationToken ct)
    {
        var rows = await db.TaskAttempts.AsNoTracking()
            .Where(a => a.StudentId == studentId)
            .GroupBy(a => new { a.TaskId, a.Task!.Title, Company = a.Task.Company!.Name, a.TotalChecks })
            .Select(g => new
            {
                g.Key.TaskId, g.Key.Title, g.Key.Company, g.Key.TotalChecks,
                Attempts = g.Count(),
                Best = g.Max(a => a.PassedChecks),
                Explained = db.KnowledgeCheckSubmissions.Any(k => k.StudentId == studentId && k.TaskId == g.Key.TaskId),
            })
            .ToListAsync(ct);

        var taskIds = rows.Select(r => r.TaskId).ToList();
        var reviews = (await db.TaskAttemptReviews.AsNoTracking()
                .Where(r => r.Attempt!.StudentId == studentId && taskIds.Contains(r.Attempt.TaskId))
                .Select(r => new { r.Attempt!.TaskId, Dto = new AttemptReviewDto(r.ReviewerName, r.Comment, r.Improvements, r.ReviewedAt) })
                .ToListAsync(ct))
            .ToLookup(r => r.TaskId, r => r.Dto);

        return rows.OrderBy(r => r.Title)
            .Select(r => new PortfolioTaskDto(r.TaskId, r.Title, r.Company, r.Attempts, r.Best, r.TotalChecks, r.Explained,
                reviews[r.TaskId].OrderBy(x => x.ReviewedAt).ToList()))
            .ToList();
    }
}
