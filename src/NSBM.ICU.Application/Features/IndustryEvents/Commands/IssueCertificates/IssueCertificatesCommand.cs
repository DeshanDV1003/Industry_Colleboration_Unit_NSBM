using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Auditing;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Common.Notifications;
using NSBM.ICU.Domain.Enums;
using NSBM.ICU.Domain.Rules;

namespace NSBM.ICU.Application.Features.IndustryEvents.Commands.IssueCertificates;

/// <summary>
/// After the event ends, gives every attendee who has no certificate yet a verifiable participation
/// certificate. Returns how many were issued; running it again only covers newly marked attendees.
/// </summary>
public sealed record IssueCertificatesCommand(int EventId) : IRequest<int>;

public sealed class IssueCertificatesCommandHandler(IApplicationDbContext db, ICurrentUser currentUser, IDateTime clock, IAuditTrail audit)
    : IRequestHandler<IssueCertificatesCommand, int>
{
    public async Task<int> Handle(IssueCertificatesCommand request, CancellationToken ct)
    {
        var partner = await db.GetCurrentPartnerAsync(currentUser, ct);
        var industryEvent = await db.LoadEventAggregateAsync(request.EventId, ct);
        if (industryEvent.CompanyId != partner.CompanyId)
            throw new ForbiddenAccessException("This event belongs to another company.");
        if (!industryEvent.HasEnded(DateOnly.FromDateTime(clock.UtcNow)))
            throw new BusinessRuleException("Certificates can be issued after the event has ended.");

        var now = clock.UtcNow;
        var issuedTo = new List<int>();
        foreach (var registration in industryEvent.Registrations)
        {
            if (registration.TryIssueCertificate(ShareCodeRules.NewCertificateCode(), now))
                issuedTo.Add(registration.StudentId);
        }
        if (issuedTo.Count == 0)
            throw new BusinessRuleException("No attendees are waiting for a certificate. Mark attendance first.");

        var userIds = await db.Students.Where(s => issuedTo.Contains(s.Id)).Select(s => s.UserId).ToListAsync(ct);
        db.Notify(userIds, NotificationKind.CertificateIssued, $"Certificate issued: {industryEvent.Title}",
            "Your participation certificate is ready to view and share.", NotificationLinks.Certificates, now);
        await audit.RecordAsync(AuditCategory.Applications, AuditActions.CertificatesIssued,
            $"Issued {issuedTo.Count} certificate{(issuedTo.Count == 1 ? "" : "s")} for \"{industryEvent.Title}\"", "IndustryEvent", industryEvent.Id.ToString(), ct);

        await db.SaveChangesAsync(ct);
        return issuedTo.Count;
    }
}
