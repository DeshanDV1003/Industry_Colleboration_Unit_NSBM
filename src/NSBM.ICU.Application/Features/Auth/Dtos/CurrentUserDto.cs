namespace NSBM.ICU.Application.Features.Auth.Dtos;

/// <summary>The signed-in account together with the headline facts of its role profile.</summary>
public sealed record CurrentUserDto(
    string UserId,
    string Email,
    string FullName,
    string Role,
    int? ProfileId,
    string? Headline,
    string? Organisation);
