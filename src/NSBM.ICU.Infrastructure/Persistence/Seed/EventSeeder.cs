using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Domain.Entities.Events;
using NSBM.ICU.Domain.Rules;
using NSBM.ICU.Infrastructure.Persistence.Seed.Data;

namespace NSBM.ICU.Infrastructure.Persistence.Seed;

/// <summary>
/// Loads sample partner events with registrations, a team and issued certificates. Past and running
/// events are built directly here because the aggregate (correctly) refuses registrations after closing.
/// </summary>
public sealed class EventSeeder(ApplicationDbContext db)
{
    private const string Reviewer = "Pavithra Subhashini";

    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        if (await db.IndustryEvents.AnyAsync(cancellationToken))
            return;

        var now = DateTime.UtcNow;
        var today = DateOnly.FromDateTime(now);
        var companies = await db.Companies.ToDictionaryAsync(c => c.Name, c => c.Id, cancellationToken);
        var partners = await db.IndustryPartners.ToDictionaryAsync(p => p.Email, p => p.Id, cancellationToken);
        var students = await db.Students.ToDictionaryAsync(s => s.StudentNo, s => s.Id, cancellationToken);

        foreach (var seed in EventData.Events)
        {
            var start = today.AddDays(seed.StartsInDays);
            var industryEvent = new IndustryEvent
            {
                CompanyId = companies[seed.Company],
                AuthorPartnerId = partners[seed.AuthorEmail],
                Title = seed.Title,
                Description = seed.Description,
                Format = seed.Format,
                Location = seed.Location,
                StartDate = start,
                EndDate = start.AddDays(seed.Days - 1),
                RegistrationClosesOn = today.AddDays(seed.RegistrationClosesInDays),
                Capacity = seed.Capacity,
                MinTeamSize = seed.MinTeam,
                MaxTeamSize = seed.MaxTeam,
            };
            if (seed.Approved) industryEvent.Approve(Reviewer, now.AddDays(Math.Min(seed.StartsInDays, 0) - 10), note: null);

            var registrations = new Dictionary<string, EventRegistration>();
            foreach (var studentNo in seed.RegisteredStudentNos.Where(students.ContainsKey))
            {
                var registration = new EventRegistration { StudentId = students[studentNo], RegisteredAt = now.AddDays(Math.Min(seed.StartsInDays, 0) - 5) };
                industryEvent.Registrations.Add(registration);
                registrations[studentNo] = registration;
            }

            foreach (var teamSeed in seed.Teams.Where(t => registrations.ContainsKey(t.CaptainStudentNo)))
            {
                var team = new EventTeam
                {
                    Name = teamSeed.Name,
                    JoinCode = ShareCodeRules.NewTeamJoinCode(),
                    CaptainStudentId = students[teamSeed.CaptainStudentNo],
                    CreatedAt = now.AddDays(Math.Min(seed.StartsInDays, 0) - 4),
                };
                industryEvent.Teams.Add(team);
                foreach (var member in teamSeed.MemberStudentNos.Prepend(teamSeed.CaptainStudentNo).Where(registrations.ContainsKey))
                    registrations[member].Team = team;
            }

            foreach (var studentNo in seed.AttendedStudentNos.Where(registrations.ContainsKey))
            {
                registrations[studentNo].MarkAttendance(true);
                if (seed.CertificatesIssued) registrations[studentNo].TryIssueCertificate(ShareCodeRules.NewCertificateCode(), now.AddDays(seed.StartsInDays + 2));
            }

            db.IndustryEvents.Add(industryEvent);
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}
