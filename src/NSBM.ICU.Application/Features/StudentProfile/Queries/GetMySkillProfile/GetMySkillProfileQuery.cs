using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.StudentProfile.Dtos;

namespace NSBM.ICU.Application.Features.StudentProfile.Queries.GetMySkillProfile;

public sealed record GetMySkillProfileQuery : IRequest<MySkillProfileDto>;

public sealed class GetMySkillProfileQueryHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IRequestHandler<GetMySkillProfileQuery, MySkillProfileDto>
{
    public async Task<MySkillProfileDto> Handle(GetMySkillProfileQuery request, CancellationToken cancellationToken)
    {
        var student = await db.GetCurrentStudentAsync(currentUser, cancellationToken);
        var skillIds = await db.StudentSkills
            .Where(s => s.StudentId == student.Id)
            .Select(s => s.SkillId)
            .ToListAsync(cancellationToken);

        return new MySkillProfileDto(skillIds, student.ShareProfileWithPartners);
    }
}
