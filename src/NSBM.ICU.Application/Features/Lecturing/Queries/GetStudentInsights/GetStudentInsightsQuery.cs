using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Evidence;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.Lecturing.Dtos;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.Lecturing.Queries.GetStudentInsights;

public sealed record GetStudentInsightsQuery(int StudentId) : IRequest<StudentInsightsDto>;

public sealed class GetStudentInsightsQueryHandler(IApplicationDbContext db, IDateTime clock) : IRequestHandler<GetStudentInsightsQuery, StudentInsightsDto>
{
    private const int RecentActivityCount = 10;

    public async Task<StudentInsightsDto> Handle(GetStudentInsightsQuery request, CancellationToken ct)
    {
        var id = request.StudentId;
        var student = await db.Students.AsNoTracking().Where(s => s.Id == id)
            .Select(s => new { s.Id, Name = s.FirstName + " " + s.LastName, s.StudentNo, s.Programme, s.Batch, Path = s.CareerPath != null ? s.CareerPath.Name : null })
            .FirstOrDefaultAsync(ct) ?? throw NotFoundException.For("Student", id);

        var progress = (await db.LoadProgressAsync([id], clock.UtcNow, ct))[id];

        var evidence = await db.LoadStudentEvidenceAsync(id, ct);
        var skills = await db.Skills.AsNoTracking().OrderBy(s => s.Name).Select(s => new { s.Id, s.Name }).ToListAsync(ct);

        var attempts = await db.TaskAttempts.AsNoTracking().Where(a => a.StudentId == id)
            .GroupBy(a => new { a.TaskId, a.Task!.Title })
            .Select(g => new InsightAttemptDto(g.Key.TaskId, g.Key.Title, g.Count(), g.Max(a => a.PassedChecks), g.Max(a => a.TotalChecks), g.Max(a => a.SubmittedAt),
                db.KnowledgeCheckSubmissions.Any(k => k.StudentId == id && k.TaskId == g.Key.TaskId)))
            .ToListAsync(ct);

        var knowledgeChecks = await db.KnowledgeCheckSubmissions.AsNoTracking().Where(k => k.StudentId == id)
            .OrderByDescending(k => k.SubmittedAt)
            .Select(k => new KnowledgeCheckViewDto(k.Task!.Title, k.SubmittedAt,
                k.Answers.OrderBy(a => a.Question!.SortOrder).Select(a => new KnowledgeAnswerViewDto(a.Question!.Prompt, a.Text)).ToList()))
            .ToListAsync(ct);

        var assignments = await db.TaskAssignments.AsNoTracking().Where(a => a.StudentId == id).OrderByDescending(a => a.AssignedAt)
            .Select(a => new TaskAssignmentDto(a.Id, a.TaskId, a.Task!.Title, a.LecturerName, a.Note, a.AssignedAt)).ToListAsync(ct);

        var notes = await db.SupportNotes.AsNoTracking().Where(n => n.StudentId == id).OrderByDescending(n => n.CreatedAt)
            .Select(n => new SupportNoteDto(n.Id, n.LecturerName, n.Text, n.CreatedAt)).ToListAsync(ct);

        return new StudentInsightsDto(student.Id, student.Name, student.StudentNo, student.Programme, student.Batch, student.Path,
            progress.BaselinePercent, progress.RoadmapCompleted, progress.RoadmapTotal, progress.Signals,
            skills.Where(s => evidence.For(s.Id) == SkillEvidence.PracticeSignal).Select(s => s.Name).ToList(),
            skills.Where(s => evidence.For(s.Id) == SkillEvidence.ListedOnly).Select(s => s.Name).ToList(),
            attempts.OrderByDescending(a => a.LastSubmittedAt).ToList(), knowledgeChecks, assignments, notes,
            await LoadRecentActivityAsync(id, ct));
    }

    private async Task<IReadOnlyList<ActivityItemDto>> LoadRecentActivityAsync(int id, CancellationToken ct)
    {
        var take = RecentActivityCount;
        var items = new List<ActivityItemDto>();
        items.AddRange(await db.TaskAttempts.Where(a => a.StudentId == id).OrderByDescending(a => a.SubmittedAt).Take(take)
            .Select(a => new ActivityItemDto(a.SubmittedAt, "Submitted attempt " + a.AttemptNumber + " on " + a.Task!.Title + " (" + a.PassedChecks + "/" + a.TotalChecks + " checks)")).ToListAsync(ct));
        items.AddRange(await db.BaselineAttempts.Where(a => a.StudentId == id).OrderByDescending(a => a.CompletedAt).Take(take)
            .Select(a => new ActivityItemDto(a.CompletedAt, "Completed the " + a.CareerPath!.Name + " skills baseline")).ToListAsync(ct));
        items.AddRange(await db.InterviewAnswers.Where(a => a.StudentId == id).OrderByDescending(a => a.AnsweredAt).Take(take)
            .Select(a => new ActivityItemDto(a.AnsweredAt, "Practised an interview question: " + a.Question!.Topic)).ToListAsync(ct));
        items.AddRange(await db.InternshipApplications.Where(a => a.StudentId == id).OrderByDescending(a => a.SubmittedAt).Take(take)
            .Select(a => new ActivityItemDto(a.SubmittedAt, "Applied to " + a.Vacancy!.Title)).ToListAsync(ct));
        items.AddRange(await db.RoadmapStepCompletions.Where(c => c.StudentId == id).OrderByDescending(c => c.CompletedAt).Take(take)
            .Select(c => new ActivityItemDto(c.CompletedAt, "Completed roadmap step: " + c.RoadmapStep!.Title)).ToListAsync(ct));

        return items.OrderByDescending(i => i.At).Take(take).ToList();
    }
}
