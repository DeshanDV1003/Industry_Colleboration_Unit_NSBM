using NSBM.ICU.Domain.Common;
using NSBM.ICU.Domain.Entities.Venues;

namespace NSBM.ICU.Domain.Rules;

/// <summary>What makes an allocation valid: an active venue that seats everyone and is free for the slot.</summary>
public static class VenueBookingRules
{
    public static readonly TimeOnly DayStarts = new(7, 0);
    public static readonly TimeOnly DayEnds = new(21, 0);

    /// <param name="venueBookings">The venue's other bookings, at least those on the candidate's day.</param>
    public static void EnsureBookable(Venue venue, VenueBooking candidate, IEnumerable<VenueBooking> venueBookings)
    {
        if (!venue.IsActive)
            throw new DomainRuleException($"{venue.DisplayName} is not taking bookings.");
        if (candidate.EndTime <= candidate.StartTime)
            throw new DomainRuleException("The end time must be after the start time.");
        if (candidate.StartTime < DayStarts || candidate.EndTime > DayEnds)
            throw new DomainRuleException($"Bookings must fall between {Clock(DayStarts)} and {Clock(DayEnds)}.");
        if (candidate.Attendees > venue.Capacity)
            throw new DomainRuleException($"{venue.DisplayName} seats {venue.Capacity}, but {candidate.Attendees} people are expected.");

        var clash = FirstClash(candidate, venueBookings);
        if (clash is not null)
            throw new DomainRuleException($"{venue.DisplayName} is already booked {Slot(clash)} for {clash.Title}.");
    }

    /// <summary>A cohort cannot be in two lectures at once, wherever they are held.</summary>
    public static void EnsureCohortFree(VenueBooking candidate, IEnumerable<VenueBooking> cohortLectures)
    {
        var clash = FirstClash(candidate, cohortLectures);
        if (clash is not null)
            throw new DomainRuleException($"This cohort already has {clash.Title} {Slot(clash)}.");
    }

    private static VenueBooking? FirstClash(VenueBooking candidate, IEnumerable<VenueBooking> others) =>
        others.FirstOrDefault(b => !ReferenceEquals(b, candidate) && (candidate.Id == 0 || b.Id != candidate.Id)
                                   && b.ClashesWith(candidate.Date, candidate.StartTime, candidate.EndTime));

    private static string Slot(VenueBooking b) => $"on {b.Date:d MMM yyyy} from {Clock(b.StartTime)} to {Clock(b.EndTime)}";
    private static string Clock(TimeOnly t) => t.ToString("HH:mm");
}
