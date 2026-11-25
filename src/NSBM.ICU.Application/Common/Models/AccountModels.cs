namespace NSBM.ICU.Application.Common.Models;

/// <summary>A sign-in account to create. The role profile is saved with it in one transaction.</summary>
public sealed record NewAccount(string Role, string Email, string FirstName, string LastName, string Password);

/// <summary>The identity facts an administrator sees for an account.</summary>
public sealed record AccountStatus(string UserId, string Email, string Role, bool IsActive);
