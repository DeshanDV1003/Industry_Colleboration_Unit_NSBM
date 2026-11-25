using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Infrastructure.Persistence.Seed.Data;

/// <summary>
/// A sample Unit year plan. Dates are month/day in the seeding year (month 13+ rolls into the next year),
/// so the demo calendar always looks current. A task release points at the first approved task on its path. Pure data.
/// </summary>
internal static class CalendarData
{
    public sealed record EventSeed(
        int Month, int Day, int Days, CalendarEventType Type, CalendarAudience Audience, string Title, string Description,
        string? Location = null, string? CareerPathSlug = null, bool ReleasesTask = false);

    public static readonly EventSeed[] Events =
    [
        new(1, 20, 1, CalendarEventType.Programme, CalendarAudience.Everyone, "Industry Collaboration Unit year launch",
            "Opening session for the year's industry programme, partner introductions and the year plan.", "NSBM Main Auditorium"),
        new(2, 10, 1, CalendarEventType.CareerPlanMilestone, CalendarAudience.Students, "Choose your career path",
            "Pick a career path and finish the baseline check so your roadmap is ready for the year."),
        new(3, 5, 1, CalendarEventType.Workshop, CalendarAudience.Students, "CV and LinkedIn workshop",
            "Hands-on session with the CV Studio. Bring your current CV.", "Faculty of Computing, Lab 3"),
        new(3, 25, 1, CalendarEventType.IndustryTalk, CalendarAudience.Everyone, "Tech talk: life as a junior engineer",
            "Partner engineers share what their first year in industry looked like.", "Online"),
        new(4, 7, 14, CalendarEventType.TaskRelease, CalendarAudience.Students, "Industry task sprint: software engineering",
            "Two weeks to complete the released industry task and its knowledge check.",
            CareerPathSlug: "software-engineering", ReleasesTask: true),
        new(5, 15, 1, CalendarEventType.Workshop, CalendarAudience.Lecturers, "Lecturer briefing: supporting students on the platform",
            "How to read readiness insights, leave notes and assign tasks.", "Board Room"),
        new(6, 2, 3, CalendarEventType.Hackathon, CalendarAudience.Everyone, "NSBM Green Hack",
            "Three-day hackathon with partner mentors. Teams of three to five.", "NSBM Innovation Hub"),
        new(6, 20, 1, CalendarEventType.CareerPlanMilestone, CalendarAudience.Students, "Mid-year roadmap check-in",
            "Aim to finish at least half of your roadmap steps by today."),
        new(7, 1, 30, CalendarEventType.ApplicationWindow, CalendarAudience.Everyone, "Internship application window",
            "Partner vacancies open for applications. Prepare your CV and practise interviews first."),
        new(7, 10, 1, CalendarEventType.Workshop, CalendarAudience.Partners, "Partner onboarding: posting tasks and vacancies",
            "Walkthrough of the Industry portal for new partner companies.", "Online"),
        new(8, 12, 5, CalendarEventType.InterviewDrive, CalendarAudience.Students, "Internship interview week",
            "Partner companies interview shortlisted students. Check your applications for invitations.", "Faculty of Computing"),
        new(9, 18, 1, CalendarEventType.CareerFair, CalendarAudience.Everyone, "NSBM Career Fair",
            "Meet partner companies, hear about openings and get CV feedback on the spot.", "NSBM Main Auditorium"),
        new(10, 6, 14, CalendarEventType.TaskRelease, CalendarAudience.Students, "Industry task sprint: open to all paths",
            "Two weeks to try the released task and get company feedback.", ReleasesTask: true),
        new(11, 26, 1, CalendarEventType.IndustryTalk, CalendarAudience.Everyone, "Tech talk: cloud careers in Sri Lanka",
            "Partner cloud engineers on skills, certifications and first roles.", "Online"),
        new(12, 3, 1, CalendarEventType.Programme, CalendarAudience.Everyone, "Year-end showcase and awards",
            "Students present their best industry task work. Partners and lecturers attend.", "NSBM Main Auditorium"),
        new(13, 21, 1, CalendarEventType.Programme, CalendarAudience.Everyone, "Next year's programme launch",
            "Kick-off for the next year of industry collaboration.", "NSBM Main Auditorium"),
    ];
}
