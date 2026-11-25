using NSBM.ICU.Domain.Common;
using NSBM.ICU.Domain.Entities.Organisations;
using NSBM.ICU.Domain.Enums;
using NSBM.ICU.Domain.Rules;

namespace NSBM.ICU.Domain.Entities.Events;

/// <summary>
/// A tech talk, workshop, challenge or hackathon hosted by a partner company. Students see it after the
/// Unit approves it. This aggregate owns the registration and team rules; load <see cref="Registrations"/>
/// and <see cref="Teams"/> before calling its methods.
/// </summary>
public class IndustryEvent : PartnerContent
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public EventFormat Format { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public string Location { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public DateOnly RegistrationClosesOn { get; set; }
    /// <summary>Both null for an individual event; both set for a team event.</summary>
    public int? MinTeamSize { get; set; }
    public int? MaxTeamSize { get; set; }

    public ICollection<EventRegistration> Registrations { get; } = [];
    public ICollection<EventTeam> Teams { get; } = [];

    public bool IsTeamEvent => MaxTeamSize is not null;
    public bool AcceptsRegistrations(DateOnly today) => Status == ModerationStatus.Approved && today <= RegistrationClosesOn;
    public bool HasStarted(DateOnly today) => today >= StartDate;
    public bool HasEnded(DateOnly today) => today > EndDate;
    public bool IsFull => Registrations.Count >= Capacity;

    public EventRegistration? RegistrationOf(int studentId) => Registrations.FirstOrDefault(r => r.StudentId == studentId);
    public int MemberCount(int teamId) => Registrations.Count(r => r.TeamId == teamId);

    public EventRegistration Register(int studentId, DateTime at)
    {
        EnsureOpen(DateOnly.FromDateTime(at));
        if (RegistrationOf(studentId) is not null) throw new DomainRuleException("You are already registered for this event.");
        if (IsFull) throw new DomainRuleException("This event is full.");

        var registration = new EventRegistration { StudentId = studentId, RegisteredAt = at };
        Registrations.Add(registration);
        return registration;
    }

    /// <summary>Removes the registration; leaving a team on the way out. Returns the registration so it can be deleted.</summary>
    public EventRegistration CancelRegistration(int studentId, DateOnly today)
    {
        EnsureOpen(today);
        var registration = RequireRegistration(studentId);
        LeaveTeamInternal(registration);
        Registrations.Remove(registration);
        return registration;
    }

    public EventTeam CreateTeam(int studentId, string name, DateTime at)
    {
        var registration = RequireTeamAction(studentId, DateOnly.FromDateTime(at));
        if (Teams.Any(t => string.Equals(t.Name, name, StringComparison.OrdinalIgnoreCase)))
            throw new DomainRuleException("Another team already uses this name.");

        string code;
        do code = ShareCodeRules.NewTeamJoinCode();
        while (Teams.Any(t => t.JoinCode == code));

        var team = new EventTeam { Name = name, JoinCode = code, CaptainStudentId = studentId, CreatedAt = at };
        Teams.Add(team);
        registration.Team = team;
        return team;
    }

    public EventTeam JoinTeam(int studentId, string joinCode, DateOnly today)
    {
        var registration = RequireTeamAction(studentId, today);
        var team = Teams.FirstOrDefault(t => t.JoinCode == ShareCodeRules.Normalise(joinCode))
                   ?? throw new DomainRuleException("No team in this event has that join code.");
        if (MemberCount(team.Id) >= MaxTeamSize) throw new DomainRuleException("This team is already full.");

        registration.TeamId = team.Id;
        registration.Team = team;
        return team;
    }

    /// <summary>Returns the team if it is now empty, so the caller can delete it.</summary>
    public EventTeam? LeaveTeam(int studentId, DateOnly today)
    {
        EnsureOpen(today);
        var registration = RequireRegistration(studentId);
        if (registration.TeamId is null) throw new DomainRuleException("You are not in a team.");
        return LeaveTeamInternal(registration);
    }

    private EventTeam? LeaveTeamInternal(EventRegistration registration)
    {
        if (registration.TeamId is not { } teamId) return null;
        var team = Teams.First(t => t.Id == teamId);
        registration.TeamId = null;
        registration.Team = null;

        // The captaincy passes to the longest-registered remaining member; an empty team is removed.
        var next = Registrations.Where(r => r.TeamId == teamId).OrderBy(r => r.RegisteredAt).FirstOrDefault();
        if (next is not null)
        {
            if (team.CaptainStudentId == registration.StudentId) team.CaptainStudentId = next.StudentId;
            return null;
        }
        Teams.Remove(team);
        return team;
    }

    private EventRegistration RequireTeamAction(int studentId, DateOnly today)
    {
        if (!IsTeamEvent) throw new DomainRuleException("This event is for individuals, not teams.");
        EnsureOpen(today);
        var registration = RequireRegistration(studentId);
        if (registration.TeamId is not null) throw new DomainRuleException("Leave your current team first.");
        return registration;
    }

    private EventRegistration RequireRegistration(int studentId) =>
        RegistrationOf(studentId) ?? throw new DomainRuleException("Register for the event first.");

    private void EnsureOpen(DateOnly today)
    {
        if (!AcceptsRegistrations(today)) throw new DomainRuleException("Registration for this event is closed.");
    }
}
