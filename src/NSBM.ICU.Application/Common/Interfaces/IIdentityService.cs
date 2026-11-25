using NSBM.ICU.Application.Common.Models;

namespace NSBM.ICU.Application.Common.Interfaces;

public interface IIdentityService
{
    /// <summary>Verifies credentials; returns <c>null</c> when they are invalid or the account is inactive.</summary>
    Task<AuthenticatedUser?> AuthenticateAsync(string email, string password, CancellationToken cancellationToken);

    /// <summary>Resolves the configured demo account for a role; returns <c>null</c> when demo sign-in is unavailable.</summary>
    Task<AuthenticatedUser?> FindDemoAccountAsync(string role, CancellationToken cancellationToken);

    /// <summary>Returns an active account by id, or <c>null</c>.</summary>
    Task<AuthenticatedUser?> FindByIdAsync(string userId, CancellationToken cancellationToken);
}
