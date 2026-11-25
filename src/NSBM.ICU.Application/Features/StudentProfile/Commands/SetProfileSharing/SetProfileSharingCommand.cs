using MediatR;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;

namespace NSBM.ICU.Application.Features.StudentProfile.Commands.SetProfileSharing;

/// <summary>Opts the student in or out of letting partner companies view their profile evidence.</summary>
public sealed record SetProfileSharingCommand(bool Share) : IRequest;

public sealed class SetProfileSharingCommandHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IRequestHandler<SetProfileSharingCommand>
{
    public async Task Handle(SetProfileSharingCommand request, CancellationToken cancellationToken)
    {
        var student = await db.GetCurrentStudentAsync(currentUser, cancellationToken);
        student.ShareProfileWithPartners = request.Share;
        await db.SaveChangesAsync(cancellationToken);
    }
}
