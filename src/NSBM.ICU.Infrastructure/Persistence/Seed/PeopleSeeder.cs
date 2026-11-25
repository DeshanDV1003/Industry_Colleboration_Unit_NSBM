using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Domain.Constants;
using NSBM.ICU.Domain.Entities.Organisations;
using NSBM.ICU.Domain.Entities.People;
using NSBM.ICU.Infrastructure.Identity;

namespace NSBM.ICU.Infrastructure.Persistence.Seed;

/// <summary>
/// Seeds partner companies, one account per role, extra partner contacts and a small student cohort.
/// Idempotent per record, so new demo people can be added without resetting the database.
/// The unit head is real; every other person and company is fictional demo data.
/// </summary>
public sealed class PeopleSeeder(ApplicationDbContext db, UserManager<ApplicationUser> users)
{
    public const string LankaCloudWorks = "Lanka CloudWorks (Pvt) Ltd";
    public const string SerendibSoftwareLabs = "Serendib Software Labs";
    public const string CeylonQualityFirst = "Ceylon QualityFirst";

    private const string SerendibPartnerEmail = "partner2@nsbm.demo";
    private const string QualityFirstPartnerEmail = "partner3@nsbm.demo";

    private static readonly (string StudentNo, string First, string Last, string Programme, int Year)[] Cohort =
    [
        ("DEMO-002", "Kavindi", "Perera", "BSc (Hons) Computer Science", 3),
        ("DEMO-003", "Tharindu", "Silva", "BSc (Hons) Software Engineering", 2),
        ("DEMO-004", "Amaya", "Jayawardena", "BSc (Hons) Computer Networks", 4),
        ("DEMO-005", "Ravindu", "Bandara", "BSc (Hons) Software Engineering", 3),
    ];

    /// <summary>Every account this seeder creates. The demo reset removes any other account.</summary>
    public static IReadOnlyList<string> SeededEmails { get; } =
    [
        .. DemoAccounts.All.Select(a => a.Email),
        SerendibPartnerEmail,
        QualityFirstPartnerEmail,
        .. Cohort.Select(c => CohortEmail(c.StudentNo)),
    ];

    private static string CohortEmail(string studentNo) => $"{studentNo.ToLowerInvariant()}@nsbm.demo";

    private const string Department = "Department of Software Engineering and Computer Security";
    private const string CohortPassword = "Student@123";
    private const string PartnerPassword = "Industry@123";

    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        var companies = await EnsureCompaniesAsync(cancellationToken);

        await EnsureAsync(DemoAccounts.Admin, "Pavithra", "Subhashini", userId => new Administrator
        {
            UserId = userId,
            Designation = "Head, Industry Collaboration Unit",
        });

        await EnsureAsync(DemoAccounts.Lecturer, "Nadeesha", "Wickramasinghe", userId => new Lecturer
        {
            UserId = userId,
            StaffNo = "ST-1042",
            Designation = "Lecturer",
            Department = Department,
        });

        await EnsureAsync(DemoAccounts.Registrar, "Chamari", "Herath", userId => new Registrar
        {
            UserId = userId,
            Designation = "Assistant Registrar",
            Office = "Registrar's Office, Academic Administration",
        });

        await EnsureAsync(DemoAccounts.Industry, "Ishara", "Gunasekara", userId => NewPartner(userId, companies[LankaCloudWorks], "Engineering Manager"));
        await EnsureAsync(Partner(SerendibPartnerEmail), "Dilan", "Rajapaksha", userId => NewPartner(userId, companies[SerendibSoftwareLabs], "Senior Software Engineer"));
        await EnsureAsync(Partner(QualityFirstPartnerEmail), "Nethmi", "Senanayake", userId => NewPartner(userId, companies[CeylonQualityFirst], "QA Lead"));

        await EnsureAsync(DemoAccounts.Student, "Sahan", "Fernando", userId => NewStudent(userId, "DEMO-001", "BSc (Hons) Software Engineering", 3));

        foreach (var (studentNo, first, last, programme, year) in Cohort)
        {
            var account = new DemoAccounts.Account(Roles.Student, CohortEmail(studentNo), CohortPassword);
            await EnsureAsync(account, first, last, userId => NewStudent(userId, studentNo, programme, year));
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    private async Task<Dictionary<string, Company>> EnsureCompaniesAsync(CancellationToken ct)
    {
        Company[] seeds =
        [
            new() { Name = LankaCloudWorks, Sector = "Cloud & DevOps", Location = "Colombo 03", IsVerified = true,
                    Description = "Fictional cloud consultancy used for demo tasks and vacancies." },
            new() { Name = SerendibSoftwareLabs, Sector = "Software Engineering", Location = "Colombo 07", IsVerified = true,
                    Description = "Fictional product engineering company used for demo tasks and vacancies." },
            new() { Name = CeylonQualityFirst, Sector = "Quality Assurance", Location = "Kandy", IsVerified = true,
                    Description = "Fictional software testing company used for demo tasks and vacancies." },
        ];

        var existing = await db.Companies.ToDictionaryAsync(c => c.Name, ct);
        foreach (var company in seeds.Where(c => !existing.ContainsKey(c.Name)))
        {
            db.Companies.Add(company);
            existing[company.Name] = company;
        }

        await db.SaveChangesAsync(ct);
        return existing;
    }

    private static DemoAccounts.Account Partner(string email) => new(Roles.Industry, email, PartnerPassword);

    private static IndustryPartner NewPartner(string userId, Company company, string jobTitle) => new()
    {
        UserId = userId,
        CompanyId = company.Id,
        JobTitle = jobTitle,
    };

    private static Student NewStudent(string userId, string studentNo, string programme, int year) => new()
    {
        UserId = userId,
        StudentNo = studentNo,
        Programme = programme,
        Batch = $"{2026 - year + 1}.1",
        AcademicYear = year,
    };

    private async Task EnsureAsync<TProfile>(DemoAccounts.Account account, string firstName, string lastName, Func<string, TProfile> profileFactory)
        where TProfile : Person
    {
        if (await users.FindByEmailAsync(account.Email) is not null)
            return;

        var user = new ApplicationUser
        {
            UserName = account.Email,
            Email = account.Email,
            EmailConfirmed = true,
            FirstName = firstName,
            LastName = lastName,
        };

        var created = await users.CreateAsync(user, account.Password);
        if (!created.Succeeded)
            throw new InvalidOperationException($"Seeding {account.Email} failed: {string.Join("; ", created.Errors.Select(e => e.Description))}");

        await users.AddToRoleAsync(user, account.Role);

        var profile = profileFactory(user.Id);
        profile.FirstName = firstName;
        profile.LastName = lastName;
        profile.Email = account.Email;
        db.Add(profile);
    }
}
