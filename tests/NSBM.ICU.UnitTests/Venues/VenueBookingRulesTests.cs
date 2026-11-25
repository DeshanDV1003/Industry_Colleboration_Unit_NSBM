using NSBM.ICU.Domain.Common;
using NSBM.ICU.Domain.Entities.Venues;
using NSBM.ICU.Domain.Enums;
using NSBM.ICU.Domain.Rules;

namespace NSBM.ICU.UnitTests.Venues;

public sealed class VenueBookingRulesTests
{
    private static readonly DateOnly Day = new(2026, 11, 30);
    private static readonly Venue Hall = new() { Code = "LH-101", Name = "Lecture Hall 101", Capacity = 100, IsActive = true };

    private static VenueBooking Booking(int startHour, int endHour, int attendees = 50, DateOnly? date = null) => new()
    {
        Purpose = VenueBookingPurpose.Lecture,
        Title = $"Lecture {startHour}",
        Date = date ?? Day,
        StartTime = new TimeOnly(startHour, 0),
        EndTime = new TimeOnly(endHour, 0),
        Attendees = attendees,
    };

    [Theory]
    [InlineData(9, 11, true)]   // same slot
    [InlineData(10, 12, true)]  // overlaps the end
    [InlineData(8, 10, true)]   // overlaps the start
    [InlineData(9, 10, true)]   // inside
    [InlineData(11, 13, false)] // starts as the other ends
    [InlineData(7, 9, false)]   // ends as the other starts
    public void Slots_clash_only_when_times_overlap(int start, int end, bool clashes) =>
        Assert.Equal(clashes, Booking(9, 11).ClashesWith(Day, new TimeOnly(start, 0), new TimeOnly(end, 0)));

    [Fact]
    public void Different_days_never_clash() =>
        Assert.False(Booking(9, 11).ClashesWith(Day.AddDays(1), new TimeOnly(9, 0), new TimeOnly(11, 0)));

    [Fact]
    public void Free_slot_is_bookable() =>
        VenueBookingRules.EnsureBookable(Hall, Booking(11, 13), [Booking(9, 11)]);

    [Fact]
    public void Clashing_slot_names_the_existing_booking()
    {
        var error = Assert.Throws<DomainRuleException>(() => VenueBookingRules.EnsureBookable(Hall, Booking(10, 12), [Booking(9, 11)]));
        Assert.Contains("Lecture 9", error.Message);
    }

    [Fact]
    public void A_booking_does_not_clash_with_itself()
    {
        var booking = Booking(9, 11);
        VenueBookingRules.EnsureBookable(Hall, booking, [booking]);
    }

    [Fact]
    public void Over_capacity_is_rejected() =>
        Assert.Throws<DomainRuleException>(() => VenueBookingRules.EnsureBookable(Hall, Booking(9, 11, attendees: 101), []));

    [Fact]
    public void Outside_teaching_hours_is_rejected() =>
        Assert.Throws<DomainRuleException>(() => VenueBookingRules.EnsureBookable(Hall, Booking(20, 22), []));

    [Fact]
    public void Inactive_venue_takes_no_bookings()
    {
        var closed = new Venue { Code = "LH-999", Name = "Closed", Capacity = 100, IsActive = false };
        Assert.Throws<DomainRuleException>(() => VenueBookingRules.EnsureBookable(closed, Booking(9, 11), []));
    }

    [Fact]
    public void A_cohort_cannot_be_in_two_places_at_once()
    {
        var elsewhere = Booking(9, 11);
        Assert.Throws<DomainRuleException>(() => VenueBookingRules.EnsureCohortFree(Booking(10, 11), [elsewhere]));
        VenueBookingRules.EnsureCohortFree(Booking(11, 12), [elsewhere]);
    }
}
