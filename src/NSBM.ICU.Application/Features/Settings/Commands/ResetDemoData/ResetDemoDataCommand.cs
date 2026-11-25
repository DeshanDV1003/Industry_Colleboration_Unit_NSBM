using MediatR;
using NSBM.ICU.Application.Common.Auditing;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.Settings.Commands.ResetDemoData;

/// <summary>Restores the fictional demo data. This changes only demo records — it is not university-wide control.</summary>
public sealed record ResetDemoDataCommand : IRequest;

public sealed class ResetDemoDataCommandHandler(IApplicationDbContext db, ICurrentUser currentUser, IDemoDataService demo, IAuditTrail audit) : IRequestHandler<ResetDemoDataCommand>
{
    public async Task Handle(ResetDemoDataCommand request, CancellationToken cancellationToken)
    {
        await db.GetCurrentAdministratorAsync(currentUser, cancellationToken);
        if (!demo.IsEnabled)
            throw new ForbiddenAccessException("Demo reset is only available in demo mode.");

        await demo.ResetAsync(cancellationToken);
        // Recorded after the reset, which clears the change tracker; the audit log itself is never reset.
        await audit.RecordAsync(AuditCategory.Platform, AuditActions.DemoReset, "Reset the demo data", cancellationToken: cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
    }
}
