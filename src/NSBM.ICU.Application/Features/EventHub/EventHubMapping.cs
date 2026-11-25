using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.EventHub.Dtos;
using NSBM.ICU.Domain.Entities.Events;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.EventHub;

/// <summary>Student-facing loaders and mappings. Students only ever see approved events.</summary>
internal static class EventHubMapping
{
    public static EventCardDto ToCard(this IndustryEvent e, int studentId, DateOnly today)
    {
        var mine = e.RegistrationOf(studentId);
        var open = e.AcceptsRegistrations(today);
        return new EventCardDto(e.Id, e.Title, e.Company!.Name, e.Format, e.StartDate, e.EndDate, e.Location, e.Capacity,
            e.Registrations.Count, e.RegistrationClosesOn, e.MinTeamSize, e.MaxTeamSize, mine is not null,
            open, open && !e.IsFull, e.HasEnded(today), mine?.HasCertificate ?? false);
    }

    /// <summary>An approved event with registrations and teams loaded and tracked, for student commands.</summary>
    public static async Task<IndustryEvent> LoadApprovedEventAsync(this IApplicationDbContext db, int id, CancellationToken ct) =>
        await db.IndustryEvents
            .Include(e => e.Registrations)
            .Include(e => e.Teams)
            .FirstOrDefaultAsync(e => e.Id == id && e.Status == ModerationStatus.Approved, ct)
        ?? throw NotFoundException.For("Event", id);

    public static IQueryable<CertificateDto> ToCertificates(this IQueryable<EventRegistration> source, IApplicationDbContext db) =>
        source.Where(r => r.CertificateCode != null)
            .Join(db.IndustryEvents, r => r.EventId, e => e.Id, (r, e) => new { r, e })
            .OrderByDescending(x => x.r.CertificateIssuedAt)
            .Select(x => new CertificateDto(x.r.CertificateCode!, x.r.Student!.FirstName + " " + x.r.Student.LastName,
                x.e.Title, x.e.Company!.Name, x.e.Format, x.e.StartDate, x.e.EndDate,
                x.r.Team != null ? x.r.Team.Name : null, x.r.CertificateIssuedAt!.Value));
}
