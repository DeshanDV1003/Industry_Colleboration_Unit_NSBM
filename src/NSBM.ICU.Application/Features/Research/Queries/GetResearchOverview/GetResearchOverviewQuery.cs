using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Evidence;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.Research.Dtos;
using NSBM.ICU.Domain.Enums;
using NSBM.ICU.Domain.Rules;

namespace NSBM.ICU.Application.Features.Research.Queries.GetResearchOverview;

public sealed record GetResearchOverviewQuery : IRequest<ResearchOverviewDto>;

public sealed class GetResearchOverviewQueryHandler(IApplicationDbContext db) : IRequestHandler<GetResearchOverviewQuery, ResearchOverviewDto>
{
    private const int TopGapCount = 6;

    public async Task<ResearchOverviewDto> Handle(GetResearchOverviewQuery request, CancellationToken ct)
    {
        var students = await db.Students.AsNoTracking()
            .Select(s => new { s.Id, s.CareerPathId, Path = s.CareerPath != null ? s.CareerPath.Name : null }).ToListAsync(ct);

        var baselines = await db.BaselineAttempts.AsNoTracking().Select(a => new { a.CorrectCount, a.QuestionCount }).ToListAsync(ct);
        var attempts = await db.TaskAttempts.AsNoTracking().Select(a => new { a.PassedChecks, a.TotalChecks }).ToListAsync(ct);
        var applications = await db.InternshipApplications.AsNoTracking().GroupBy(a => a.Status)
            .Select(g => new ApplicationStatusCountDto(g.Key, g.Count())).ToListAsync(ct);

        return new ResearchOverviewDto(
            students.Count,
            students.Count(s => s.CareerPathId is not null),
            students.Where(s => s.Path is not null).GroupBy(s => s.Path!).Select(g => new CountByLabelDto(g.Key, g.Count())).OrderByDescending(x => x.Count).ToList(),
            baselines.Count,
            baselines.Count == 0 ? 0 : (int)Math.Round(baselines.Average(b => SkillSignalRules.Percent(b.CorrectCount, b.QuestionCount))),
            await db.IndustryTasks.CountAsync(t => t.Status == ModerationStatus.Approved, ct),
            attempts.Count,
            attempts.Count(a => a.TotalChecks > 0 && a.PassedChecks == a.TotalChecks),
            await db.KnowledgeCheckSubmissions.CountAsync(ct),
            await db.InterviewAnswers.CountAsync(ct),
            await db.Vacancies.CountAsync(v => v.Status == ModerationStatus.Approved, ct),
            applications.OrderBy(a => a.Status).ToList(),
            await db.MentorQuestions.CountAsync(ct),
            await db.MentorQuestions.CountAsync(q => q.Reply != null, ct),
            await db.SprintItemProgress.CountAsync(p => p.Status == SprintItemStatus.Done, ct),
            await LoadSkillGapsAsync(students.Where(s => s.CareerPathId is not null).Select(s => (s.Id, s.CareerPathId!.Value)).ToList(), ct));
    }

    /// <summary>Core skills most often missing (not listed and no practice signal) among students on that path.</summary>
    private async Task<IReadOnlyList<SkillGapDto>> LoadSkillGapsAsync(IReadOnlyList<(int StudentId, int PathId)> students, CancellationToken ct)
    {
        var coreSkills = await db.CareerPaths.AsNoTracking()
            .SelectMany(p => p.Skills.Where(s => s.Importance == SkillImportance.Core)
                .Select(s => new { CareerPathId = p.Id, s.SkillId, Skill = s.Skill!.Name, Path = p.Name }))
            .ToListAsync(ct);

        var evidence = new Dictionary<int, StudentEvidence>();
        foreach (var (studentId, _) in students)
            evidence[studentId] = await db.LoadStudentEvidenceAsync(studentId, ct);

        return coreSkills
            .Select(cs =>
            {
                var onPath = students.Where(s => s.PathId == cs.CareerPathId).ToList();
                var missing = onPath.Count(s => evidence[s.StudentId].For(cs.SkillId) == SkillEvidence.Missing);
                return new SkillGapDto(cs.Skill, cs.Path, missing, onPath.Count);
            })
            .Where(g => g.StudentsMissing > 0)
            .OrderByDescending(g => (double)g.StudentsMissing / g.StudentsOnPath).ThenByDescending(g => g.StudentsMissing)
            .Take(TopGapCount)
            .ToList();
    }
}
