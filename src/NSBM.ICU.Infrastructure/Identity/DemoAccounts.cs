using NSBM.ICU.Domain.Constants;

namespace NSBM.ICU.Infrastructure.Identity;

/// <summary>
/// The primary fictional account of each role. Single source of truth for the seeder and for demo sign-in.
/// </summary>
public static class DemoAccounts
{
    public sealed record Account(string Role, string Email, string Password);

    public static readonly Account Admin = new(Roles.Admin, "icu.head@nsbm.demo", "Admin@123");
    public static readonly Account Lecturer = new(Roles.Lecturer, "lecturer@nsbm.demo", "Lecturer@123");
    public static readonly Account Student = new(Roles.Student, "student@nsbm.demo", "Student@123");
    public static readonly Account Industry = new(Roles.Industry, "partner@nsbm.demo", "Industry@123");
    public static readonly Account Registrar = new(Roles.Registrar, "registrar@nsbm.demo", "Registrar@123");

    public static readonly IReadOnlyList<Account> All = [Admin, Lecturer, Student, Industry, Registrar];

    public static Account? ForRole(string role) => All.FirstOrDefault(a => a.Role == role);
}
