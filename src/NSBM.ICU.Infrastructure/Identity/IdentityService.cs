using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Common.Models;
using NSBM.ICU.Domain.Entities.People;
using NSBM.ICU.Infrastructure.Persistence;

namespace NSBM.ICU.Infrastructure.Identity;

public sealed class IdentityService(UserManager<ApplicationUser> users, ApplicationDbContext db, IOptions<DemoOptions> demo) : IIdentityService
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

    public Task<bool> IsActiveAsync(string userId, CancellationToken cancellationToken) =>
        db.Users.AnyAsync(u => u.Id == userId && u.IsActive, cancellationToken);

    public async Task<string> CreateAccountAsync(NewAccount account, Person profile, CancellationToken cancellationToken)
    {
        var user = new ApplicationUser
        {
            UserName = account.Email,
            Email = account.Email,
            EmailConfirmed = true,
            FirstName = account.FirstName,
            LastName = account.LastName,
        };

        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        Ensure(await users.CreateAsync(user, account.Password), "Password");
        Ensure(await users.AddToRoleAsync(user, account.Role), "Password");

        profile.UserId = user.Id;
        profile.FirstName = account.FirstName;
        profile.LastName = account.LastName;
        profile.Email = account.Email;
        db.Add(profile);
        await db.SaveChangesAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);
        return user.Id;
    }

    public async Task<IReadOnlyDictionary<string, AccountStatus>> GetAccountStatusesAsync(CancellationToken cancellationToken)
    {
        var accounts = await (
            from user in db.Users.AsNoTracking()
            join userRole in db.UserRoles on user.Id equals userRole.UserId
            join role in db.Roles on userRole.RoleId equals role.Id
            select new AccountStatus(user.Id, user.Email!, role.Name!, user.IsActive))
            .ToListAsync(cancellationToken);

        return accounts.ToDictionary(a => a.UserId);
    }

    public async Task RenameAsync(string userId, string firstName, string lastName, CancellationToken cancellationToken)
    {
        var user = await RequireAsync(userId);
        user.FirstName = firstName;
        user.LastName = lastName;
        Ensure(await users.UpdateAsync(user), "FirstName");
    }

    public async Task SetActiveAsync(string userId, bool isActive, CancellationToken cancellationToken)
    {
        var user = await RequireAsync(userId);
        user.IsActive = isActive;
        Ensure(await users.UpdateAsync(user), "IsActive");
    }

    public async Task ResetPasswordAsync(string userId, string newPassword, CancellationToken cancellationToken)
    {
        var user = await RequireAsync(userId);
        var token = await users.GeneratePasswordResetTokenAsync(user);
        Ensure(await users.ResetPasswordAsync(user, token, newPassword), "NewPassword");
    }

    public async Task ChangePasswordAsync(string userId, string currentPassword, string newPassword, CancellationToken cancellationToken)
    {
        var user = await RequireAsync(userId);
        if (!await users.CheckPasswordAsync(user, currentPassword))
            throw new ValidationException([new ValidationFailure("CurrentPassword", "Your current password is not right.")]);

        Ensure(await users.ChangePasswordAsync(user, currentPassword, newPassword), "NewPassword");
    }

    private async Task<ApplicationUser> RequireAsync(string userId) =>
        await users.FindByIdAsync(userId) ?? throw NotFoundException.For("User", userId);

    /// <summary>Turns identity rule failures into field errors: duplicate e-mail on "Email", the rest on the given field.</summary>
    private static void Ensure(IdentityResult result, string field)
    {
        if (result.Succeeded) return;
        throw new ValidationException(result.Errors.Select(e => new ValidationFailure(
            e.Code is "DuplicateEmail" or "DuplicateUserName" or "InvalidEmail" or "InvalidUserName" ? "Email" : field,
            e.Code is "DuplicateEmail" or "DuplicateUserName" ? "An account with this e-mail already exists." : e.Description)));
    }

    private async Task<AuthenticatedUser> ToAuthenticatedUserAsync(ApplicationUser user)
    {
        var roles = await users.GetRolesAsync(user);
        return new AuthenticatedUser(user.Id, user.Email!, user.FullName, roles.Single());
    }
}
