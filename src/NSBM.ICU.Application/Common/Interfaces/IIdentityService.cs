using NSBM.ICU.Application.Common.Models;
using NSBM.ICU.Domain.Entities.People;

namespace NSBM.ICU.Application.Common.Interfaces;

/// <summary>
/// Sign-in accounts. Methods that change an account throw <see cref="Exceptions.ValidationException"/> with the
/// offending field ("Email", "Password", "CurrentPassword", "NewPassword") when the identity rules reject it.
/// </summary>
public interface IIdentityService
{
    /// <summary>Verifies credentials; returns <c>null</c> when they are invalid or the account is inactive.</summary>
    Task<AuthenticatedUser?> AuthenticateAsync(string email, string password, CancellationToken cancellationToken);

    /// <summary>Resolves the configured demo account for a role; returns <c>null</c> when demo sign-in is unavailable.</summary>
    Task<AuthenticatedUser?> FindDemoAccountAsync(string role, CancellationToken cancellationToken);

    /// <summary>Returns an active account by id, or <c>null</c>.</summary>
    Task<AuthenticatedUser?> FindByIdAsync(string userId, CancellationToken cancellationToken);

    /// <summary>Cheap check used on every authenticated request, so a deactivated account loses access at once.</summary>
    Task<bool> IsActiveAsync(string userId, CancellationToken cancellationToken);

    /// <summary>Creates the account, gives it its role and saves <paramref name="profile"/> for it, all or nothing. Returns the new user id.</summary>
    Task<string> CreateAccountAsync(NewAccount account, Person profile, CancellationToken cancellationToken);

    Task<IReadOnlyDictionary<string, AccountStatus>> GetAccountStatusesAsync(CancellationToken cancellationToken);

    /// <summary>Keeps the account's display name in step with its profile.</summary>
    Task RenameAsync(string userId, string firstName, string lastName, CancellationToken cancellationToken);

    Task SetActiveAsync(string userId, bool isActive, CancellationToken cancellationToken);

    /// <summary>An administrator sets a new password without knowing the old one.</summary>
    Task ResetPasswordAsync(string userId, string newPassword, CancellationToken cancellationToken);

    /// <summary>The account holder changes their own password; the current one must be right.</summary>
    Task ChangePasswordAsync(string userId, string currentPassword, string newPassword, CancellationToken cancellationToken);
}
