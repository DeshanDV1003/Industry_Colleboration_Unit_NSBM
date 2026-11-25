using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Domain.Entities.Profiles;

namespace NSBM.ICU.Application.Features.StudentProfile.Commands.UpdateMySkills;

/// <summary>Replaces the skills the student lists. Listing a skill is a claim — it never creates a practice signal.</summary>
public sealed record UpdateMySkillsCommand(IReadOnlyList<int> SkillIds) : IRequest;

public sealed class UpdateMySkillsCommandValidator : AbstractValidator<UpdateMySkillsCommand>
{
    public UpdateMySkillsCommandValidator()
    {
        RuleFor(x => x.SkillIds).NotNull();
        RuleFor(x => x.SkillIds).Must(ids => ids.Distinct().Count() == ids.Count).WithMessage("Each skill can be listed once.");
    }
}

public sealed class UpdateMySkillsCommandHandler(IApplicationDbContext db, ICurrentUser currentUser, IDateTime clock)
    : IRequestHandler<UpdateMySkillsCommand>
{
    public async Task Handle(UpdateMySkillsCommand request, CancellationToken cancellationToken)
    {
        var student = await db.GetCurrentStudentAsync(currentUser, cancellationToken);

        if (await db.Skills.CountAsync(s => request.SkillIds.Contains(s.Id), cancellationToken) != request.SkillIds.Count)
            throw new BusinessRuleException("One or more selected skills do not exist.");

        var existing = await db.StudentSkills.Where(s => s.StudentId == student.Id).ToListAsync(cancellationToken);
        var wanted = request.SkillIds.ToHashSet();

        db.StudentSkills.RemoveRange(existing.Where(s => !wanted.Contains(s.SkillId)));
        foreach (var skillId in wanted.Except(existing.Select(s => s.SkillId)))
            db.StudentSkills.Add(new StudentSkill { StudentId = student.Id, SkillId = skillId, AddedAt = clock.UtcNow });

        await db.SaveChangesAsync(cancellationToken);
    }
}
