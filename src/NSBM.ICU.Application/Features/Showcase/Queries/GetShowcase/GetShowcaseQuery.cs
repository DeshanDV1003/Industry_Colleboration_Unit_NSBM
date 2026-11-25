using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.Showcase.Dtos;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.Showcase.Queries.GetShowcase;

/// <summary>
/// Everything the public homepage shows, built from live platform data. Only approved partner content, public
/// ("Everyone") Unit events, aggregate counts, and profile cards students chose to share.
/// </summary>
public sealed record GetShowcaseQuery : IRequest<ShowcaseDto>;

public sealed class GetShowcaseQueryHandler(IApplicationDbContext db, IDateTime clock) : IRequestHandler<GetShowcaseQuery, ShowcaseDto>
{
    private const int InternshipsShown = 6;
    private const int EventsShown = 6;
    private const int EventWindowDays = 120;
    private const int ProfilesShown = 6;

    public async Task<ShowcaseDto> Handle(GetShowcaseQuery request, CancellationToken ct)
    {
        var today = DateOnly.FromDateTime(clock.UtcNow);

        var partners = await db.Companies.AsNoTracking()
            .Select(c => new ShowcasePartnerDto(
                c.Name,
                db.IndustryTasks.Count(t => t.CompanyId == c.Id && t.Status == ModerationStatus.Approved),
                db.Vacancies.Count(v => v.CompanyId == c.Id && v.Status == ModerationStatus.Approved && v.ClosingDate >= today),
                db.IndustryEvents.Count(e => e.CompanyId == c.Id && e.Status == ModerationStatus.Approved)))
            .ToListAsync(ct);
        var activePartners = partners.Where(p => p.Tasks + p.Internships + p.Events > 0).OrderBy(p => p.Name).ToList();

        var openVacancies = db.Vacancies.AsNoTracking().Where(v => v.Status == ModerationStatus.Approved && v.ClosingDate >= today);
        var internships = await openVacancies
            .OrderBy(v => v.ClosingDate)
            .Take(InternshipsShown)
            .Select(v => new ShowcaseInternshipDto(v.Title, v.Company!.Name, v.CareerPath!.Name, v.WorkMode, v.Location, v.ClosingDate))
            .ToListAsync(ct);

        var stats = new ShowcaseStatsDto(
            activePartners.Count,
            await openVacancies.CountAsync(ct),
            await db.IndustryTasks.CountAsync(t => t.Status == ModerationStatus.Approved, ct),
            await db.Students.CountAsync(s => s.CareerPathId != null, ct),
            await db.EventRegistrations.CountAsync(r => r.CertificateCode != null, ct),
            await db.SkillBadges.CountAsync(b => b.Status == ModerationStatus.Approved, ct));

        var careerPaths = await db.CareerPaths.AsNoTracking().OrderBy(p => p.SortOrder)
            .Select(p => new ShowcaseCareerPathDto(p.Slug, p.Name, p.Tagline, p.Skills.Count))
            .ToListAsync(ct);

        return new ShowcaseDto(stats, careerPaths, activePartners, internships,
            await UpcomingEventsAsync(today, ct), await FeaturedProfilesAsync(ct));
    }

    private async Task<List<ShowcaseEventDto>> UpcomingEventsAsync(DateOnly today, CancellationToken ct)
    {
        var until = today.AddDays(EventWindowDays);
        var unitEvents = await db.CalendarEvents.AsNoTracking()
            .Where(e => e.Audience == CalendarAudience.Everyone && e.EndDate >= today && e.StartDate <= until)
            .Select(e => new ShowcaseEventDto(e.Title, e.Type.ToString(), null, e.StartDate, e.EndDate, e.Location))
            .ToListAsync(ct);
        var partnerEvents = await db.IndustryEvents.AsNoTracking()
            .Where(e => e.Status == ModerationStatus.Approved && e.EndDate >= today && e.StartDate <= until)
            .Select(e => new ShowcaseEventDto(e.Title, e.Format.ToString(), e.Company!.Name, e.StartDate, e.EndDate, e.Location))
            .ToListAsync(ct);
        return unitEvents.Concat(partnerEvents).OrderBy(e => e.StartDate).Take(EventsShown).ToList();
    }

    /// <summary>Only students who turned on their public profile card, most badges first.</summary>
    private async Task<List<ShowcaseProfileDto>> FeaturedProfilesAsync(CancellationToken ct)
    {
        var rows = await db.Students.AsNoTracking()
            .Where(s => s.ShareProfileCard && s.ProfileCardSlug != null)
            .Select(s => new
            {
                s.FirstName, s.LastName, s.Programme, CareerPath = s.CareerPath!.Name, s.ProfileCardSlug,
                Badges = db.SkillBadges.Where(b => b.StudentId == s.Id && b.Status == ModerationStatus.Approved).Select(b => b.Skill!.Name).ToList(),
            })
            .ToListAsync(ct);

        return rows.OrderByDescending(r => r.Badges.Count).ThenBy(r => r.LastName).Take(ProfilesShown)
            .Select(r => new ShowcaseProfileDto($"{r.FirstName} {r.LastName}", r.Programme, r.CareerPath, r.Badges, r.ProfileCardSlug!))
            .ToList();
    }
}
