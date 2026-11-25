using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.Dashboard.Dtos;
using NSBM.ICU.Domain.Enums;
using NSBM.ICU.Domain.Rules;

namespace NSBM.ICU.Application.Features.Dashboard.Queries.GetStudentOverview;

public sealed record GetStudentOverviewQuery : IRequest<StudentOverviewDto>;

public sealed class GetStudentOverviewQueryHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IRequestHandler<GetStudentOverviewQuery, StudentOverviewDto>
{
    private static readonly RoadmapProgressDto NoRoadmap = new(0, 0, 0, null);

    public async Task<StudentOverviewDto> Handle(GetStudentOverviewQuery request, CancellationToken cancellationToken)
    {
        var student = await db.GetCurrentStudentAsync(currentUser, cancellationToken);
        if (student.CareerPathId is not { } pathId)
            return new StudentOverviewDto(null, NoRoadmap, null);

        var career = await db.CareerPaths.AsNoTracking()
            .Where(p => p.Id == pathId)
            .Select(p => new CareerChoiceDto(p.Id, p.Slug, p.Name, p.Tagline, student.CareerPathSelectedAt))
            .SingleAsync(cancellationToken);

        return new StudentOverviewDto(career,
            await LoadRoadmapAsync(student.Id, pathId, cancellationToken),
            await LoadBaselineAsync(student.Id, pathId, cancellationToken));
    }

    private async Task<RoadmapProgressDto> LoadRoadmapAsync(int studentId, int pathId, CancellationToken ct)
    {
        var steps = await db.RoadmapSteps.AsNoTracking()
            .Where(s => s.CareerPathId == pathId)
            .OrderBy(s => s.SortOrder)
            .Select(s => new { s.Title, Done = db.RoadmapStepCompletions.Any(c => c.StudentId == studentId && c.RoadmapStepId == s.Id) })
            .ToListAsync(ct);

        var completed = steps.Count(s => s.Done);
        return new RoadmapProgressDto(completed, steps.Count, SkillSignalRules.Percent(completed, steps.Count),
            steps.FirstOrDefault(s => !s.Done)?.Title);
    }

    private async Task<BaselineSummaryDto?> LoadBaselineAsync(int studentId, int pathId, CancellationToken ct)
    {
        var latest = await db.BaselineAttempts.AsNoTracking()
            .Where(a => a.StudentId == studentId && a.CareerPathId == pathId)
            .OrderByDescending(a => a.CompletedAt)
            .Select(a => new
            {
                a.CompletedAt,
                a.CorrectCount,
                a.QuestionCount,
                Answers = a.Answers.Select(x => new { SkillName = x.Question!.Skill!.Name, x.IsCorrect }).ToList(),
            })
            .FirstOrDefaultAsync(ct);

        if (latest is null) return null;

        var signals = latest.Answers
            .GroupBy(a => a.SkillName)
            .Select(g => (Skill: g.Key, Signal: SkillSignalRules.Classify(g.Count(a => a.IsCorrect), g.Count())))
            .OrderBy(x => x.Skill)
            .ToList();

        return new BaselineSummaryDto(
            latest.CompletedAt,
            SkillSignalRules.Percent(latest.CorrectCount, latest.QuestionCount),
            signals.Where(s => s.Signal == SkillSignal.Strong).Select(s => s.Skill).ToList(),
            signals.Where(s => s.Signal == SkillSignal.NeedsPractice).Select(s => s.Skill).ToList());
    }
}
