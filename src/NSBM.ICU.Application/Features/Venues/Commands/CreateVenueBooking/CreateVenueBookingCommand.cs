using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.Venues.Models;
using NSBM.ICU.Domain.Entities.Venues;
using NSBM.ICU.Domain.Enums;
using NSBM.ICU.Domain.Rules;

namespace NSBM.ICU.Application.Features.Venues.Commands.CreateVenueBooking;

/// <summary>
/// The registrar allocates a venue to a cohort's lecture (optionally every week) or to an event. Every
/// occurrence is checked for clashes before anything is saved, and the people affected are notified.
/// </summary>
public sealed record CreateVenueBookingCommand(VenueBookingInput Booking) : IRequest<IReadOnlyList<int>>;

public sealed class CreateVenueBookingCommandValidator : AbstractValidator<CreateVenueBookingCommand>
{
    public CreateVenueBookingCommandValidator() => RuleFor(x => x.Booking).NotNull().SetValidator(new VenueBookingInputValidator());
}

public sealed class CreateVenueBookingCommandHandler(IApplicationDbContext db, ICurrentUser currentUser, IDateTime clock)
    : IRequestHandler<CreateVenueBookingCommand, IReadOnlyList<int>>
{
    private sealed record Subject(string Title, int Attendees, DateOnly? From = null, DateOnly? To = null);

    public async Task<IReadOnlyList<int>> Handle(CreateVenueBookingCommand request, CancellationToken ct)
    {
        var input = request.Booking;
        var registrar = await db.GetCurrentRegistrarAsync(currentUser, ct);
        var venue = await db.Venues.FirstOrDefaultAsync(v => v.Id == input.VenueId, ct) ?? throw NotFoundException.For("Venue", input.VenueId);

        if (input.Date < DateOnly.FromDateTime(clock.UtcNow))
            throw new BusinessRuleException("Choose today or a later date.");

        var subject = await ResolveSubjectAsync(input, ct);
        var dates = Enumerable.Range(0, input.RepeatWeeks).Select(week => input.Date.AddDays(7 * week)).ToList();
        if (subject.From is { } from && subject.To is { } to && dates.Any(d => d < from || d > to))
            throw new BusinessRuleException($"The event runs from {from:d MMM yyyy} to {to:d MMM yyyy}; book a date in that range.");

        var bookings = dates.Select(date => new VenueBooking
        {
            VenueId = venue.Id,
            Purpose = input.Purpose,
            Title = subject.Title,
            Date = date,
            StartTime = input.StartTime,
            EndTime = input.EndTime,
            Programme = input.Programme?.Trim(),
            AcademicYear = input.AcademicYear,
            IndustryEventId = input.IndustryEventId,
            CalendarEventId = input.CalendarEventId,
            Attendees = subject.Attendees,
            Note = string.IsNullOrWhiteSpace(input.Note) ? null : input.Note.Trim(),
            BookedByName = registrar.FullName,
        }).ToList();

        await EnsureNoClashesAsync(venue, bookings, ct);

        db.VenueBookings.AddRange(bookings);
        var first = bookings[0];
        var when = $"{first.Date:ddd d MMM yyyy}, {first.StartTime:HH\\:mm}–{first.EndTime:HH\\:mm}";
        var repeat = bookings.Count > 1 ? $" Repeats weekly for {bookings.Count} weeks." : string.Empty;
        db.NotifyAll(await db.ForAsync(first, currentUser.UserId, ct), NotificationKind.HallAssigned,
            $"Hall assigned: {first.Title}", $"{venue.DisplayName} ({venue.Building}), {when}.{repeat}", clock.UtcNow);

        await db.SaveChangesAsync(ct);
        return bookings.Select(b => b.Id).ToList();
    }

    private async Task<Subject> ResolveSubjectAsync(VenueBookingInput input, CancellationToken ct)
    {
        var expected = input.ExpectedAttendees ?? 0;
        switch (input.Purpose)
        {
            case VenueBookingPurpose.Lecture:
                var programme = input.Programme!.Trim();
                var cohortSize = await db.Students.CountAsync(s => s.Programme == programme && s.AcademicYear == input.AcademicYear, ct);
                if (cohortSize == 0)
                    throw new BusinessRuleException($"No students are enrolled in {programme}, year {input.AcademicYear}.");
                return new Subject(input.Title!.Trim(), Math.Max(cohortSize, expected));

            case VenueBookingPurpose.IndustryEvent:
                var hosted = await db.IndustryEvents.AsNoTracking().FirstOrDefaultAsync(e => e.Id == input.IndustryEventId, ct)
                             ?? throw NotFoundException.For("Event", input.IndustryEventId!);
                if (hosted.Status != ModerationStatus.Approved)
                    throw new BusinessRuleException("Only approved partner events can be given a hall.");
                return new Subject(TitleOr(input, hosted.Title), Math.Max(hosted.Capacity, expected), hosted.StartDate, hosted.EndDate);

            default:
                var planned = await db.CalendarEvents.AsNoTracking().FirstOrDefaultAsync(e => e.Id == input.CalendarEventId, ct)
                              ?? throw NotFoundException.For("Calendar event", input.CalendarEventId!);
                return new Subject(TitleOr(input, planned.Title), expected, planned.StartDate, planned.EndDate);
        }
    }

    private static string TitleOr(VenueBookingInput input, string fallback) =>
        string.IsNullOrWhiteSpace(input.Title) ? fallback : input.Title.Trim();

    private async Task EnsureNoClashesAsync(Venue venue, IReadOnlyList<VenueBooking> bookings, CancellationToken ct)
    {
        var dates = bookings.Select(b => b.Date).ToList();
        var venueBookings = await db.VenueBookings.AsNoTracking()
            .Where(b => b.VenueId == venue.Id && dates.Contains(b.Date)).ToListAsync(ct);

        var first = bookings[0];
        var cohortLectures = first.Purpose == VenueBookingPurpose.Lecture
            ? await db.VenueBookings.AsNoTracking()
                .Where(b => b.Purpose == VenueBookingPurpose.Lecture && b.Programme == first.Programme
                            && b.AcademicYear == first.AcademicYear && dates.Contains(b.Date))
                .ToListAsync(ct)
            : [];

        foreach (var booking in bookings)
        {
            VenueBookingRules.EnsureBookable(venue, booking, venueBookings);
            if (booking.Purpose == VenueBookingPurpose.Lecture)
                VenueBookingRules.EnsureCohortFree(booking, cohortLectures);
        }
    }
}
