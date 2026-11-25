using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Common.Reports;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.Reports.Queries.GetHallTimetableReport;

/// <summary>Hall allocations in a date range, optionally for one venue or one cohort: the timetable to print or share.</summary>
public sealed record GetHallTimetableReportQuery(DateOnly From, DateOnly To, int? VenueId, string? Programme, int? AcademicYear) : IRequest<ReportTable>;

public sealed class GetHallTimetableReportQueryValidator : AbstractValidator<GetHallTimetableReportQuery>
{
    public const int MaxRangeDays = 186;

    public GetHallTimetableReportQueryValidator()
    {
        RuleFor(x => x.To)
            .GreaterThanOrEqualTo(x => x.From).WithMessage("The end date cannot be before the start date.")
            .Must((x, to) => to.DayNumber - x.From.DayNumber <= MaxRangeDays).WithMessage("Report on at most six months at a time.");
        RuleFor(x => x.AcademicYear).NotNull().When(x => !string.IsNullOrEmpty(x.Programme)).WithMessage("Choose the cohort's year.");
    }
}

public sealed class GetHallTimetableReportQueryHandler(IApplicationDbContext db) : IRequestHandler<GetHallTimetableReportQuery, ReportTable>
{
    private static readonly ReportColumn[] Columns =
    [
        new("Date", ReportColumnKind.Date), new("Day"), new("From", ReportColumnKind.Time), new("To", ReportColumnKind.Time),
        new("Venue"), new("Building"), new("Purpose"), new("Title"), new("Cohort"), new("Attendees", ReportColumnKind.Number), new("Allocated by"),
    ];

    public async Task<ReportTable> Handle(GetHallTimetableReportQuery request, CancellationToken ct)
    {
        var cohortOnly = !string.IsNullOrEmpty(request.Programme);
        var bookings = await db.VenueBookings.AsNoTracking()
            .Where(b => b.Date >= request.From && b.Date <= request.To)
            .Where(b => request.VenueId == null || b.VenueId == request.VenueId)
            .Where(b => !cohortOnly || (b.Programme == request.Programme && b.AcademicYear == request.AcademicYear))
            .OrderBy(b => b.Date).ThenBy(b => b.StartTime).ThenBy(b => b.Venue!.Code)
            .Select(b => new
            {
                b.Date, b.StartTime, b.EndTime, Venue = b.Venue!.Code + " · " + b.Venue.Name, b.Venue.Building,
                b.Purpose, b.Title, b.Programme, b.AcademicYear, b.Attendees, b.BookedByName,
            })
            .ToListAsync(ct);

        var rows = bookings.Select(b => new object?[]
        {
            b.Date, b.Date.DayOfWeek.ToString(), b.StartTime, b.EndTime, b.Venue, b.Building, ReportText.Humanize(b.Purpose), b.Title,
            b.Purpose == VenueBookingPurpose.Lecture ? $"{b.Programme} · Year {b.AcademicYear}" : null, b.Attendees, b.BookedByName,
        }).ToList();

        var scope = new List<string> { ReportText.Range(request.From, request.To) };
        if (request.VenueId is { } venueId)
            scope.Add(await db.Venues.Where(v => v.Id == venueId).Select(v => v.Code + " · " + v.Name).FirstOrDefaultAsync(ct) ?? "Unknown venue");
        if (cohortOnly)
            scope.Add($"{request.Programme} · Year {request.AcademicYear}");

        return new ReportTable("Hall timetable", string.Join(" · ", scope), Columns, rows) { FileStem = "hall-timetable" };
    }
}
