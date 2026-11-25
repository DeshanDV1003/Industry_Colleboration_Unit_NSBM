using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Common.Moderation;
using NSBM.ICU.Application.Features.IndustryEvents.Models;

namespace NSBM.ICU.Application.Features.IndustryEvents.Commands.UpdateIndustryEvent;

/// <summary>Edits a pending or rejected event and sends it back to the review queue.</summary>
public sealed record UpdateIndustryEventCommand(int Id, IndustryEventInput Event) : IRequest;

public sealed class UpdateIndustryEventCommandValidator : AbstractValidator<UpdateIndustryEventCommand>
{
    public UpdateIndustryEventCommandValidator(IDateTime clock)
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Event).NotNull().SetValidator(new IndustryEventInputValidator(clock));
    }
}

public sealed class UpdateIndustryEventCommandHandler(IApplicationDbContext db, ICurrentUser currentUser) : IRequestHandler<UpdateIndustryEventCommand>
{
    public async Task Handle(UpdateIndustryEventCommand request, CancellationToken ct)
    {
        var partner = await db.GetCurrentPartnerAsync(currentUser, ct);
        var industryEvent = await db.IndustryEvents.FirstOrDefaultAsync(e => e.Id == request.Id, ct)
                            ?? throw NotFoundException.For("Event", request.Id);

        industryEvent.EnsureEditableBy(partner.CompanyId);
        request.Event.ApplyTo(industryEvent);
        industryEvent.ResubmitForReview();
        await db.SaveChangesAsync(ct);
    }
}
