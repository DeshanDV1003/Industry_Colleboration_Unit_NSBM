using MediatR;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.Achievements.Dtos;
using NSBM.ICU.Domain.Rules;

namespace NSBM.ICU.Application.Features.Achievements.Commands.SetProfileCardSharing;

/// <summary>Turns the public profile card on or off. Off by default; the student decides.</summary>
public sealed record SetProfileCardSharingCommand(bool Share) : IRequest<ProfileCardSettingsDto>;

public sealed class SetProfileCardSharingCommandHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IRequestHandler<SetProfileCardSharingCommand, ProfileCardSettingsDto>
{
    public async Task<ProfileCardSettingsDto> Handle(SetProfileCardSharingCommand request, CancellationToken ct)
    {
        var student = await db.GetCurrentStudentAsync(currentUser, ct);
        student.SetProfileCardSharing(request.Share, ShareCodeRules.NewProfileSlug);
        await db.SaveChangesAsync(ct);
        return new ProfileCardSettingsDto(student.ShareProfileCard, student.ShareProfileCard ? student.ProfileCardSlug : null);
    }
}
