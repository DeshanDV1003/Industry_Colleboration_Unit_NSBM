using FluentValidation;
using MediatR;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.Vacancies.Models;
using NSBM.ICU.Domain.Entities.Opportunities;
using NSBM.ICU.Application.Common.Notifications;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.Vacancies.Commands.CreateVacancy;

/// <summary>Posts a vacancy for the partner's company. It stays Pending until a Unit admin reviews it.</summary>
public sealed record CreateVacancyCommand(VacancyInput Vacancy) : IRequest<int>;

public sealed class CreateVacancyCommandValidator : AbstractValidator<CreateVacancyCommand>
{
    public CreateVacancyCommandValidator(IDateTime clock)
    {
        RuleFor(x => x.Vacancy).NotNull().SetValidator(new VacancyInputValidator(clock));
    }
}

public sealed class CreateVacancyCommandHandler(IApplicationDbContext db, ICurrentUser currentUser, IDateTime clock)
    : IRequestHandler<CreateVacancyCommand, int>
{
    public async Task<int> Handle(CreateVacancyCommand request, CancellationToken cancellationToken)
    {
        var partner = await db.GetCurrentPartnerAsync(currentUser, cancellationToken);
        await db.EnsureReferencesExistAsync(request.Vacancy, cancellationToken);

        var vacancy = new Vacancy { CompanyId = partner.CompanyId, AuthorPartnerId = partner.Id };
        request.Vacancy.ApplyTo(vacancy);

        db.Vacancies.Add(vacancy);
        db.Notify(await db.AdminUserIdsAsync(cancellationToken), NotificationKind.ContentSubmitted,
            $"New vacancy to review: {vacancy.Title}", $"{partner.FullName} submitted it for approval.", NotificationLinks.VacancyApprovals, clock.UtcNow);
        await db.SaveChangesAsync(cancellationToken);
        return vacancy.Id;
    }
}
