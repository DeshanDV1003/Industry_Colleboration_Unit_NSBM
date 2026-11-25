using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Common.Moderation;
using NSBM.ICU.Application.Features.Vacancies.Models;

namespace NSBM.ICU.Application.Features.Vacancies.Commands.UpdateVacancy;

/// <summary>Edits a pending or rejected vacancy and sends it back to the review queue.</summary>
public sealed record UpdateVacancyCommand(int Id, VacancyInput Vacancy) : IRequest;

public sealed class UpdateVacancyCommandValidator : AbstractValidator<UpdateVacancyCommand>
{
    public UpdateVacancyCommandValidator(IDateTime clock)
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Vacancy).NotNull().SetValidator(new VacancyInputValidator(clock));
    }
}

public sealed class UpdateVacancyCommandHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IRequestHandler<UpdateVacancyCommand>
{
    public async Task Handle(UpdateVacancyCommand request, CancellationToken cancellationToken)
    {
        var partner = await db.GetCurrentPartnerAsync(currentUser, cancellationToken);
        var vacancy = await db.Vacancies.Include(v => v.Skills).FirstOrDefaultAsync(v => v.Id == request.Id, cancellationToken)
                      ?? throw NotFoundException.For("Vacancy", request.Id);

        vacancy.EnsureEditableBy(partner.CompanyId);
        await db.EnsureReferencesExistAsync(request.Vacancy, cancellationToken);
        request.Vacancy.ApplyTo(vacancy);
        vacancy.ResubmitForReview();

        await db.SaveChangesAsync(cancellationToken);
    }
}
