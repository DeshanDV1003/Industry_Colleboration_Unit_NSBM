using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Infrastructure.Persistence.Seed.Data;

/// <summary>Fictional partner events. Dates are days from the seeding day, so the demo always looks current. Pure data.</summary>
internal static class EventData
{
    public sealed record TeamSeed(string Name, string CaptainStudentNo, string[] MemberStudentNos);

    public sealed record EventSeed(
        string Key, string Company, string AuthorEmail, EventFormat Format, string Title, string Description, string Location,
        int StartsInDays, int Days, int RegistrationClosesInDays, int Capacity, int? MinTeam, int? MaxTeam, bool Approved,
        string[] RegisteredStudentNos, TeamSeed[] Teams, string[] AttendedStudentNos, bool CertificatesIssued);

    public static readonly EventSeed[] Events =
    [
        new("green-hack", PeopleSeeder.SerendibSoftwareLabs, "partner2@nsbm.demo", EventFormat.Hackathon,
            "Green Hack 48: build for sustainability",
            "A 48-hour hackathon to build a working prototype that helps a Sri Lankan community use less energy, water or paper. "
            + "Serendib engineers mentor every team, and the top three teams present to the company's product leads.",
            "NSBM Innovation Hub", StartsInDays: 20, Days: 2, RegistrationClosesInDays: 15, Capacity: 60, MinTeam: 2, MaxTeam: 4, Approved: true,
            RegisteredStudentNos: ["DEMO-002", "DEMO-003"], Teams: [new("Green Byte", "DEMO-002", ["DEMO-003"])],
            AttendedStudentNos: [], CertificatesIssued: false),

        new("testing-talk", PeopleSeeder.CeylonQualityFirst, "partner3@nsbm.demo", EventFormat.TechTalk,
            "Tech talk: how we test a banking app before every release",
            "Ceylon QualityFirst's QA lead walks through a real release checklist: risk-based test planning, automation in CI and what still needs a human tester. Q&A at the end.",
            "Online (link sent after registration)", StartsInDays: 6, Days: 1, RegistrationClosesInDays: 5, Capacity: 150, MinTeam: null, MaxTeam: null, Approved: true,
            RegisteredStudentNos: ["DEMO-002"], Teams: [], AttendedStudentNos: [], CertificatesIssued: false),

        new("container-workshop", PeopleSeeder.LankaCloudWorks, "partner@nsbm.demo", EventFormat.Workshop,
            "Hands-on workshop: deploy your first container",
            "Bring a laptop. Package a small Node.js API in a container, run it locally, then deploy it to a practice cloud account with a Lanka CloudWorks engineer.",
            "Faculty of Computing, Lab 2", StartsInDays: 30, Days: 1, RegistrationClosesInDays: 27, Capacity: 30, MinTeam: null, MaxTeam: null, Approved: false,
            RegisteredStudentNos: [], Teams: [], AttendedStudentNos: [], CertificatesIssued: false),

        new("bug-bash", PeopleSeeder.CeylonQualityFirst, "partner3@nsbm.demo", EventFormat.Challenge,
            "Bug Bash Challenge: find the most bugs in a practice shop",
            "Teams explore a deliberately buggy online shop and report as many clear, reproducible bugs as they can in three hours.",
            "Faculty of Computing, Lab 1", StartsInDays: -21, Days: 1, RegistrationClosesInDays: -23, Capacity: 40, MinTeam: 2, MaxTeam: 3, Approved: true,
            RegisteredStudentNos: ["DEMO-001", "DEMO-002", "DEMO-004"], Teams: [new("Null Pointers", "DEMO-001", ["DEMO-004"])],
            AttendedStudentNos: ["DEMO-001", "DEMO-002", "DEMO-004"], CertificatesIssued: true),
    ];
}
