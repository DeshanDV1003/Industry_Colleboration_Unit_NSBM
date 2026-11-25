using NSBM.ICU.Application.Common.Models;

namespace NSBM.ICU.Application.Features.Auth.Dtos;

public sealed record AuthResponseDto(
    string AccessToken,
    DateTime ExpiresAt,
    string UserId,
    string Email,
    string FullName,
    string Role)
{
    public static AuthResponseDto From(AuthenticatedUser user, AccessToken token) =>
        new(token.Value, token.ExpiresAt, user.UserId, user.Email, user.FullName, user.Role);
}
