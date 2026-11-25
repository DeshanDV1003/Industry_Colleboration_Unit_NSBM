using FluentValidation;
using MediatR;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Common.Notifications;
using NSBM.ICU.Application.Features.IndustryEvents.Models;
using NSBM.ICU.Domain.Entities.Events;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.IndustryEvents.Commands.CreateIndustryEvent;

/// <summary>A partner proposes an event. It stays Pending until a Unit admin reviews it.</summary>
public sealed record CreateIndustryEventCommand(IndustryEventInput Event) : IRequest<int>;

public sealed class CreateIndustryEventCommandValidator : AbstractValidator<CreateIndustryEventCommand>
{
    public CreateIndustryEventCommandValidator(IDateTime clock) => RuleFor(x => x.Event).NotNull().SetValidator(new IndustryEventInputValidator(clock));
}

public sealed class CreateIndustryEventCommandHandler(IApplicationDbContext db, ICurrentUser currentUser, IDateTime clock)
    : IRequestHandler<CreateIndustryEventCommand, int>
{
    public async Task<int> Handle(CreateIndustryEventCommand request, CancellationToken ct)
    {
        var partner = await db.GetCurrentPartnerAsync(currentUser, ct);
        var industryEvent = new IndustryEvent { CompanyId = partner.CompanyId, AuthorPartnerId = partner.Id };
        request.Event.ApplyTo(industryEvent);
        db.IndustryEvents.Add(industryEvent);

        db.Notify(await db.AdminUserIdsAsync(ct), NotificationKind.ContentSubmitted,
            $"New event to review: {industryEvent.Title}", $"{partner.FullName} submitted it for approval.",
            NotificationLinks.EventApprovals, clock.UtcNow);
        await db.SaveChangesAsync(ct);
        return industryEvent.Id;
    }
}
