using FluentValidation;
using MediatR;
using NSBM.ICU.Application.Common.Auditing;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.Settings.Commands.UpdateAdminProfile;

/// <summary>Updates the signed-in administrator's displayed designation.</summary>
public sealed record UpdateAdminProfileCommand(string Designation) : IRequest;

public sealed class UpdateAdminProfileCommandValidator : AbstractValidator<UpdateAdminProfileCommand>
{
    public UpdateAdminProfileCommandValidator() => RuleFor(x => x.Designation).NotEmpty().MaximumLength(150);
}

public sealed class UpdateAdminProfileCommandHandler(IApplicationDbContext db, ICurrentUser currentUser, IAuditTrail audit) : IRequestHandler<UpdateAdminProfileCommand>
{
    public async Task Handle(UpdateAdminProfileCommand request, CancellationToken cancellationToken)
    {
        var admin = await db.GetCurrentAdministratorAsync(currentUser, cancellationToken);
        admin.Designation = request.Designation.Trim();
        await audit.RecordAsync(AuditCategory.Platform, AuditActions.AdminProfileUpdated, $"Changed their designation to \"{admin.Designation}\"",
            cancellationToken: cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
    }
}
