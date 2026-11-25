using FluentValidation;
using MediatR;
using NSBM.ICU.Application.Features.Calendar.Dtos;

namespace NSBM.ICU.Application.Features.Calendar.Queries.GetMyCalendar;

/// <summary>
/// The signed-in user's year: the Unit events meant for them plus their own dates. Students also get
/// planner deadlines, vacancy closing dates on their path, assigned tasks and interview invitations;
/// partners get their company's vacancy closings; the Unit admin gets every approved vacancy closing.
/// </summary>
public sealed record GetMyCalendarQuery(int Year) : IRequest<IReadOnlyList<CalendarEntryDto>>;

public sealed class GetMyCalendarQueryValidator : AbstractValidator<GetMyCalendarQuery>
{
    public GetMyCalendarQueryValidator() => RuleFor(x => x.Year).InclusiveBetween(2000, 2100);
}

public sealed class GetMyCalendarQueryHandler(CalendarEntryReader reader) : IRequestHandler<GetMyCalendarQuery, IReadOnlyList<CalendarEntryDto>>
{
    public Task<IReadOnlyList<CalendarEntryDto>> Handle(GetMyCalendarQuery request, CancellationToken ct) =>
        reader.ReadAsync(new DateOnly(request.Year, 1, 1), new DateOnly(request.Year, 12, 31), ct);
}
