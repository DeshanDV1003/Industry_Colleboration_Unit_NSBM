using FluentValidation;
using MediatR;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Common.Notifications;
using NSBM.ICU.Domain.Entities.Organisations;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.Showcase.Commands.SubmitPartnerEnquiry;

/// <summary>
/// A company asks to partner with the Unit from the public website. <paramref name="Website"/> is a hidden
/// "honeypot" field people never see: when a bot fills it in, the request looks successful but nothing is saved.
/// </summary>
public sealed record SubmitPartnerEnquiryCommand(
    string OrganisationName,
    string ContactName,
    string Email,
    string? Phone,
    PartnerInterest Interest,
    string Message,
    string? Website) : IRequest;

public sealed class SubmitPartnerEnquiryCommandValidator : AbstractValidator<SubmitPartnerEnquiryCommand>
{
    public SubmitPartnerEnquiryCommandValidator()
    {
        RuleFor(x => x.OrganisationName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.ContactName).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(200);
        RuleFor(x => x.Phone).MaximumLength(30).Matches(@"^[0-9+()\-\s]*$").WithMessage("Use digits, spaces and + ( ) - only.");
        RuleFor(x => x.Interest).IsInEnum();
        RuleFor(x => x.Message).NotEmpty().MinimumLength(20).WithMessage("Tell us a little more (at least 20 characters).").MaximumLength(2000);
    }
}

public sealed class SubmitPartnerEnquiryCommandHandler(IApplicationDbContext db, IDateTime clock) : IRequestHandler<SubmitPartnerEnquiryCommand>
{
    public async Task Handle(SubmitPartnerEnquiryCommand request, CancellationToken ct)
    {
        if (!string.IsNullOrWhiteSpace(request.Website)) return;

        var now = clock.UtcNow;
        var enquiry = new PartnerEnquiry
        {
            OrganisationName = request.OrganisationName.Trim(),
            ContactName = request.ContactName.Trim(),
            Email = request.Email.Trim(),
            Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim(),
            Interest = request.Interest,
            Message = request.Message.Trim(),
            SubmittedAt = now,
        };
        db.PartnerEnquiries.Add(enquiry);
        db.Notify(await db.AdminUserIdsAsync(ct), NotificationKind.PartnerEnquiry,
            $"Partner enquiry: {enquiry.OrganisationName}", $"{enquiry.ContactName} is interested in {request.Interest}.",
            NotificationLinks.PartnerEnquiries, now);
        await db.SaveChangesAsync(ct);
    }
}
