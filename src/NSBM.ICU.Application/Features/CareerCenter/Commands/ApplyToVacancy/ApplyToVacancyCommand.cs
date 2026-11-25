using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Evidence;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Domain.Entities.Opportunities;

namespace NSBM.ICU.Application.Features.CareerCenter.Commands.ApplyToVacancy;

/// <summary>Sends a practice application to an open vacancy. The company sees it in its Industry portal.</summary>
public sealed record ApplyToVacancyCommand(int VacancyId, string CoverNote) : IRequest<int>;

public sealed class ApplyToVacancyCommandValidator : AbstractValidator<ApplyToVacancyCommand>
{
    public ApplyToVacancyCommandValidator()
    {
        RuleFor(x => x.VacancyId).GreaterThan(0);
        RuleFor(x => x.CoverNote)
            .Must(note => (note?.Trim().Length ?? 0) >= 30).WithMessage("Write at least a couple of sentences (30 characters) about why you are applying.")
            .MaximumLength(2000);
    }
}

public sealed class ApplyToVacancyCommandHandler(IApplicationDbContext db, ICurrentUser currentUser, IDateTime clock)
    : IRequestHandler<ApplyToVacancyCommand, int>
{
    public async Task<int> Handle(ApplyToVacancyCommand request, CancellationToken cancellationToken)
    {
        var student = await db.GetCurrentStudentAsync(currentUser, cancellationToken);

        var requiredSkills = await db.OpenVacancies(clock)
            .Where(v => v.Id == request.VacancyId)
            .Select(v => v.Skills.Where(s => s.IsRequired).Select(s => new { s.SkillId, s.Skill!.Name }).ToList())
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new BusinessRuleException("This vacancy is not open for applications.");

        if (await db.InternshipApplications.AnyAsync(a => a.VacancyId == request.VacancyId && a.StudentId == student.Id, cancellationToken))
            throw new BusinessRuleException("You have already applied to this vacancy.");

        var evidence = await db.LoadStudentEvidenceAsync(student.Id, cancellationToken);
        var required = evidence.Describe(requiredSkills.Select(s => (s.SkillId, s.Name)));

        var application = new InternshipApplication
        {
            VacancyId = request.VacancyId,
            StudentId = student.Id,
            CoverNote = request.CoverNote.Trim(),
            MatchPercentAtSubmission = StudentEvidence.MatchPercent(required),
        };
        application.Submit(clock.UtcNow);

        db.InternshipApplications.Add(application);
        await db.SaveChangesAsync(cancellationToken);
        return application.Id;
    }
}
