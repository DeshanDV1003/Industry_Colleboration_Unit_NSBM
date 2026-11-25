using NSBM.ICU.Domain.Enums;
using NSBM.ICU.Infrastructure.Identity;

namespace NSBM.ICU.Infrastructure.Persistence.Seed.Data;

/// <summary>Fictional vacancies and a demo cohort profile. Pure data — the logic lives in <see cref="OpportunitySeeder"/>.</summary>
internal static class OpportunityData
{
    public sealed record VacancySeed(
        string Key,
        string Company,
        string AuthorEmail,
        string CareerPathSlug,
        string Title,
        string Description,
        WorkMode WorkMode,
        string Location,
        int DurationMonths,
        int ClosesInDays,
        string[] Required,
        string[] Preferred,
        bool Approved);

    public sealed record ApplicationSeed(string VacancyKey, ApplicationStatus Status, string CoverNote);

    public sealed record CohortSeed(string StudentNo, string CareerPathSlug, string[] Skills, bool SharesProfile, ApplicationSeed? Application);

    private const string Serendib = PeopleSeeder.SerendibSoftwareLabs;
    private const string QualityFirst = PeopleSeeder.CeylonQualityFirst;
    private const string CloudWorks = PeopleSeeder.LankaCloudWorks;

    public static readonly VacancySeed[] Vacancies =
    [
        new("se-intern", Serendib, "partner2@nsbm.demo", "software-engineering",
            "Associate Software Engineer Intern",
            "Join a product squad building our order-management platform. You will fix bugs, build small features with a mentor, write tests and take part in sprint ceremonies.",
            WorkMode.Hybrid, "Colombo 07", 6, 45,
            Required: ["Object-oriented design", "Git & version control", "SQL", "REST APIs"],
            Preferred: ["JavaScript", "C#", "Unit testing"],
            Approved: true),

        new("frontend-intern", Serendib, "partner2@nsbm.demo", "software-engineering",
            "Frontend Developer Intern",
            "Help our web team build accessible customer-facing screens. You will turn designs into components, connect them to APIs and improve test coverage.",
            WorkMode.Remote, "Remote (Sri Lanka)", 6, 30,
            Required: ["JavaScript", "Git & version control", "REST APIs"],
            Preferred: ["Unit testing", "Communication"],
            Approved: true),

        new("qa-intern", QualityFirst, "partner3@nsbm.demo", "quality-assurance",
            "QA Engineer Intern",
            "Work with our testing team on client projects: design test cases, run regression cycles, report defects clearly and learn how we plan releases.",
            WorkMode.OnSite, "Kandy", 6, 40,
            Required: ["Test case design", "Bug reporting", "SQL"],
            Preferred: ["Test automation", "API testing", "Agile & Scrum"],
            Approved: true),

        new("devops-intern", CloudWorks, DemoAccounts.Industry.Email, "devops",
            "DevOps Intern",
            "Support our platform team: maintain CI/CD pipelines, containerise services and help monitor client environments.",
            WorkMode.Hybrid, "Colombo 03", 6, 50,
            Required: ["Linux & shell", "Git & version control", "Docker", "CI/CD pipelines"],
            Preferred: ["Python", "Infrastructure as code", "Monitoring & logging"],
            Approved: true),

        new("cloud-intern", CloudWorks, DemoAccounts.Industry.Email, "cloud-engineering",
            "Cloud Support Intern",
            "Help customers run workloads on AWS and Azure: provision resources, troubleshoot networking and access issues, and document fixes.",
            WorkMode.Hybrid, "Colombo 03", 6, 35,
            Required: ["Cloud fundamentals", "Networking basics", "Linux & shell"],
            Preferred: ["Cloud security & IAM", "Docker", "Communication"],
            Approved: true),

        new("automation-intern", QualityFirst, "partner3@nsbm.demo", "quality-assurance",
            "Test Automation Intern",
            "Build and maintain automated UI and API test suites that run in our clients' pipelines.",
            WorkMode.Remote, "Remote (Sri Lanka)", 6, 60,
            Required: ["Test automation", "JavaScript", "Git & version control"],
            Preferred: ["API testing", "CI/CD pipelines"],
            Approved: false),
    ];

    /// <summary>Demo progress for the fictional cohort so partner and lecturer views have data. DEMO-001 is left for live demos.</summary>
    public static readonly CohortSeed[] Cohort =
    [
        new("DEMO-002", "software-engineering",
            ["JavaScript", "Git & version control", "SQL", "REST APIs", "Object-oriented design"], SharesProfile: true,
            new("se-intern", ApplicationStatus.Shortlisted, "I enjoy building backend features and have used SQL and REST APIs in two university projects.")),

        new("DEMO-003", "quality-assurance",
            ["Test case design", "Bug reporting", "SQL"], SharesProfile: false,
            new("qa-intern", ApplicationStatus.UnderReview, "I like finding edge cases and writing clear bug reports. I want to learn how QA works on client projects.")),

        new("DEMO-004", "cloud-engineering",
            ["Linux & shell", "Networking basics", "Cloud fundamentals", "Docker"], SharesProfile: true,
            new("cloud-intern", ApplicationStatus.InterviewInvited, "I set up a small web app on a cloud free tier and want to grow into cloud support.")),

        new("DEMO-005", "devops",
            ["Linux & shell", "Git & version control", "Docker", "CI/CD pipelines", "Python"], SharesProfile: true,
            new("devops-intern", ApplicationStatus.Submitted, "I automate my own projects with GitHub Actions and Docker and would love to do this for real systems.")),
    ];
}
