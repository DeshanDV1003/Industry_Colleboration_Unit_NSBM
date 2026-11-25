using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Common.Models;

namespace NSBM.ICU.Infrastructure.Identity;

public sealed class IdentityService(UserManager<ApplicationUser> users, IOptions<DemoOptions> demo) : IIdentityService
{
    public async Task<AuthenticatedUser?> AuthenticateAsync(string email, string password, CancellationToken cancellationToken)
    {
        var user = await users.FindByEmailAsync(email);
        if (user is not { IsActive: true } || !await users.CheckPasswordAsync(user, password))
            return null;

        return await ToAuthenticatedUserAsync(user);
    }

    public async Task<AuthenticatedUser?> FindDemoAccountAsync(string role, CancellationToken cancellationToken)
    {
        if (!demo.Value.Enabled || DemoAccounts.ForRole(role) is not { } account)
            return null;

        var user = await users.FindByEmailAsync(account.Email);
        return user is { IsActive: true } ? await ToAuthenticatedUserAsync(user) : null;
    }

    public async Task<AuthenticatedUser?> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
        var user = await users.FindByIdAsync(userId);
        return user is { IsActive: true } ? await ToAuthenticatedUserAsync(user) : null;
    }

    private async Task<AuthenticatedUser> ToAuthenticatedUserAsync(ApplicationUser user)
    {
        var roles = await users.GetRolesAsync(user);
        return new AuthenticatedUser(user.Id, user.Email!, user.FullName, roles.Single());
    }
}
