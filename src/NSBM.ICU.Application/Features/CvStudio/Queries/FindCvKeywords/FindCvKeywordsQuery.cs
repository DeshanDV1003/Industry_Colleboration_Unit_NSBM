using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.CvStudio.Dtos;
using NSBM.ICU.Domain.Rules;

namespace NSBM.ICU.Application.Features.CvStudio.Queries.FindCvKeywords;

/// <summary>Highlights catalogue skills mentioned in pasted CV text. No skill level and no ATS score.</summary>
public sealed record FindCvKeywordsQuery(string Text) : IRequest<IReadOnlyList<CvKeywordMatchDto>>;

public sealed class FindCvKeywordsQueryValidator : AbstractValidator<FindCvKeywordsQuery>
{
    public FindCvKeywordsQueryValidator()
    {
        RuleFor(x => x.Text).NotEmpty().WithMessage("Paste some text from your CV first.").MaximumLength(20000);
    }
}

public sealed class FindCvKeywordsQueryHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IRequestHandler<FindCvKeywordsQuery, IReadOnlyList<CvKeywordMatchDto>>
{
    public async Task<IReadOnlyList<CvKeywordMatchDto>> Handle(FindCvKeywordsQuery request, CancellationToken cancellationToken)
    {
        var student = await db.GetCurrentStudentAsync(currentUser, cancellationToken);
        var listed = await db.StudentSkills.Where(s => s.StudentId == student.Id).Select(s => s.SkillId).ToListAsync(cancellationToken);
        var skills = await db.Skills.AsNoTracking().OrderBy(s => s.Name).ToListAsync(cancellationToken);

        return skills
            .Where(s => CvKeywordRules.Mentions(request.Text, s.Name))
            .Select(s => new CvKeywordMatchDto(s.Id, s.Name, listed.Contains(s.Id)))
            .ToList();
    }
}
