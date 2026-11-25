using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;

namespace NSBM.ICU.Application.Features.Careers.Commands.SelectCareerPath;

/// <summary>Sets the career role the signed-in student is preparing for. The learning pages follow this choice.</summary>
public sealed record SelectCareerPathCommand(int CareerPathId) : IRequest;

public sealed class SelectCareerPathCommandValidator : AbstractValidator<SelectCareerPathCommand>
{
    public SelectCareerPathCommandValidator()
    {
        RuleFor(x => x.CareerPathId).GreaterThan(0);
    }
}

public sealed class SelectCareerPathCommandHandler(IApplicationDbContext db, ICurrentUser currentUser, IDateTime clock)
    : IRequestHandler<SelectCareerPathCommand>
{
    public async Task Handle(SelectCareerPathCommand request, CancellationToken cancellationToken)
    {
        if (!await db.CareerPaths.AnyAsync(p => p.Id == request.CareerPathId, cancellationToken))
            throw NotFoundException.For("Career path", request.CareerPathId);

        var student = await db.GetCurrentStudentAsync(currentUser, cancellationToken);
        student.ChooseCareerPath(request.CareerPathId, clock.UtcNow);

        await db.SaveChangesAsync(cancellationToken);
    }
}
