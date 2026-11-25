namespace NSBM.ICU.Application.Common.Models;

/// <summary>Identity facts about a signed-in account, independent of how identity is stored.</summary>
public sealed record AuthenticatedUser(string UserId, string Email, string FullName, string Role);
